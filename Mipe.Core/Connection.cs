using System.Text.Json.Serialization;
using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;
using Mipe.Core.Chains;

namespace Mipe.Core;

public class Connection
{
  private InputMidiDevice? _device;
  private string _name = string.Empty;
  private ILogger? _logger;

  [JsonIgnore]
  public bool Connected { get; private set; }

  /// <summary>
  /// Specifies if the connection is enabled. Default is true.
  /// Disabled connections will not be connected.
  /// </summary>
  public bool Enabled { get; set; } = true;

  /// <summary>
  /// An optional name for the connection.
  /// If no name is given, the input port name will be used.
  /// </summary>
  public string Name
  {
    get => string.IsNullOrEmpty(_name) ? InputPort : _name;
    set => _name = value;
  }

  /// <summary>
  /// The name of the input port to connect to.
  /// </summary>
  public string InputPort { get; set; } = string.Empty;

  /// <summary>
  /// The chain of items to process the incoming messages.
  /// </summary>
  public IMidiChainItem[]? Chain { get; set; }


  public async Task<bool> TryConnect(ILoggerFactory? loggerFactory)
  {
    try
    {
      await Connect(loggerFactory);
      return true;
    }
    catch (Exception e)
    {
      _logger?.LogError(e, "Failed to connect connection {name}.", Name);
      return false;
    }
  }

  public async Task Connect(ILoggerFactory? loggerFactory)
  {
    if (!Enabled) return;
    if (Connected) return;
    
    _logger = loggerFactory?.CreateLogger<Connection>();

    try
    {
      ArgumentException.ThrowIfNullOrEmpty(InputPort, nameof(InputPort));
      _device = InputMidiDevicePool.Instance.Open(InputPort);
      _device.MessageReceived += DeviceOnMessageReceived;
      await Task.WhenAll((Chain ?? []).Select(a =>
      {
        try
        {
          return a.Initialize(loggerFactory?.CreateLogger(a.GetType()));
        }
        catch (Exception ex)
        {
          _logger?.LogError(ex, "Failed to initialize chain item {name}: {message}", a.GetType().Name, ex.Message);
          throw;
        }
      }));
      Connected = true;
    }
    catch
    {
      await Disconnect();
      throw;
    }
  }

  public async Task Disconnect()
  {
    if (!Enabled) return;
    await Task.WhenAll((Chain ?? []).Select(c => c.Deinitialize()));
    if (_device != null)
    {
      _device.MessageReceived -= DeviceOnMessageReceived;
      InputMidiDevicePool.Instance.Close(_device);
    }

    Connected = false;
  }


  private void DeviceOnMessageReceived(object? sender, IMidiMessage e)
  {
    Dispatch(e);
  }

  public void Dispatch(IMidiMessage midiMessage)
  {
    if (_device != null && _logger?.IsEnabled(LogLevel.Debug) == true)
    {
      _logger.LogMidi(_device, midiMessage);
    }

    _ = Chain?.Process(midiMessage, _logger);
  }
}