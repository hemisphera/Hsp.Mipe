using System.Text.Json;
using System.Text.Json.Serialization;
using Hsp.Midi;
using Microsoft.Extensions.Logging;

namespace Mipe.Core;

public class MipeInstance
{
  private static readonly JsonSerializerOptions SerializerOptions = new()
  {
    Converters =
    {
      new MidiChainItemJsonConverter(),
      new RangeJsonConverter(),
      new JsonStringEnumConverter()
    }
  };

  [JsonIgnore]
  public ILoggerFactory? LoggerFactory { get; set; }

  public bool Started { get; private set; }


  private readonly List<VirtualMidiPort> _virtualPorts = [];
  private ILogger? _logger;


  /// <summary>
  /// Specifies a list of virtual MIDI ports to create.
  /// </summary>
  public string[]? VirtualPorts { get; set; }

  /// <summary>
  /// Specifies a list of MIDI connections to create.
  /// </summary>
  public Connection[]? Connections { get; set; }


  public static MipeInstance Load(string? path)
  {
    ArgumentException.ThrowIfNullOrEmpty(path);
    using var s = File.OpenRead(path);
    return
      JsonSerializer.Deserialize<MipeInstance>(s, SerializerOptions)
      ?? throw new Exception($"Failed to load configuration from '{path}'.");
  }


  public async Task Start()
  {
    _logger = LoggerFactory?.CreateLogger<MipeInstance>();
    if (Started) throw new InvalidOperationException();
    try
    {
      foreach (var portName in VirtualPorts ?? [])
      {
        _virtualPorts.Add(VirtualMidiPort.Create(portName));
        _logger?.LogInformation("Created virtual port '{name}'.", portName);
      }

      var delay = TimeSpan.FromSeconds(2);
      _logger?.LogInformation("Waiting {delay}s", delay.TotalSeconds);
      await Task.Delay(delay);
      await Task.WhenAll((Connections ?? []).Select(a => a.TryConnect(LoggerFactory)));

      Started = true;
      _logger?.LogInformation("Connected");
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "Failed to connect.");
      await Stop();
    }
  }

  public async Task Stop()
  {
    await Task.WhenAll((Connections ?? []).Select(a => a.Disconnect()));

    foreach (var virtualPort in _virtualPorts.ToArray())
    {
      virtualPort.Dispose();
      _virtualPorts.Remove(virtualPort);
      _logger?.LogInformation("Removed virtual port '{name}'.", virtualPort.Name);
    }

    _logger = null;
  }
}