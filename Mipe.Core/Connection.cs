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

  [JsonIgnore]
  public bool Connected { get; private set; }

  private ILogger? _logger;

  public string Name
  {
    get => string.IsNullOrEmpty(_name) ? InputPort : _name;
    set => _name = value;
  }

  public string InputPort { get; set; } = string.Empty;

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
    if (Connected) return;
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
      _logger = loggerFactory?.CreateLogger<Connection>();
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
    await Task.WhenAll((Chain ?? []).Select(c => c.Deinitialize()));
    if (_device != null)
    {
      _device.MessageReceived -= DeviceOnMessageReceived;
      InputMidiDevicePool.Instance.Close(_device);
    }

    _logger = null;
    Connected = false;
  }


  private void DeviceOnMessageReceived(object? sender, IMidiMessage e)
  {
    if (_device != null && _logger?.IsEnabled(LogLevel.Debug) == true)
    {
      _logger.LogMidi(_device, e);
    }

    _ = Chain?.Process(e, _logger);
  }
}