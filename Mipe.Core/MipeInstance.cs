using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Hsp.Midi;
using Microsoft.Extensions.Logging;

namespace Mipe.Core;

public sealed class MipeInstance : IAsyncDisposable
{
  private static readonly JsonSerializerOptions SerializerOptions = new()
  {
    Converters =
    {
      new MidiChainItemJsonConverter(),
      new ChannelMessageJsonConverter(),
      new RangeJsonConverter(),
      new JsonStringEnumConverter()
    }
  };

  public MidiClock? Clock { get; }

  public string? CurrentFilePath { get; private set; }

  public bool Started { get; private set; }


  private readonly List<VirtualMidiPort> _virtualPorts = [];
  private readonly ILoggerFactory? _loggerFactory;
  private readonly ILogger _logger;


  /// <summary>
  /// Specifies a list of virtual MIDI ports to create.
  /// </summary>
  public string[]? VirtualPorts { get; set; }

  /// <summary>
  /// Specifies a list of MIDI connections to create.
  /// </summary>
  public Connection[]? Connections { get; set; }


  public MipeInstance(MidiClock clock, ILoggerFactory loggerFactory)
  {
    _loggerFactory = loggerFactory;
    Clock = clock;
    _logger = _loggerFactory.CreateLogger<MipeInstance>();
  }


  public async Task Load(string? path)
  {
    if (Started) throw new InvalidOperationException("Instance is already started.");
    _logger.LogInformation("Loading configuration from '{path}'.", path);
    ArgumentException.ThrowIfNullOrEmpty(path);
    await using var s = File.OpenRead(path);

    VirtualPorts = [];
    Connections = [];

    var jo = await JsonNode.ParseAsync(s) as JsonObject;
    VirtualPorts = jo?[nameof(VirtualPorts)].Deserialize<string[]>(SerializerOptions);
    Connections = jo?[nameof(Connections)].Deserialize<Connection[]>(SerializerOptions);
    foreach (var connection in Connections ?? [])
    {
      connection.Owner = this;
    }

    CurrentFilePath = path;
    _logger.LogInformation("Configuration loaded.");
  }


  public async Task Start()
  {
    if (Started) throw new InvalidOperationException();
    try
    {
      foreach (var portName in VirtualPorts ?? [])
      {
        _virtualPorts.Add(VirtualMidiPort.Create(portName));
        _logger?.LogInformation("Created virtual port '{name}'.", portName);
      }

      await Task.WhenAll((Connections ?? []).Select(a => a.TryConnect(_loggerFactory)));

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
  }

  public async ValueTask DisposeAsync()
  {
    await Stop();
  }
}