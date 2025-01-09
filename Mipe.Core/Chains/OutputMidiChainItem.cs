﻿using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class OutputMidiChainItem : IMidiChainItem
{
  private OutputMidiDevice? _device;

  /// <summary>
  /// Specifies the name of the MIDI port to use.
  /// </summary>
  public string? PortName { get; set; } = string.Empty;


  public async Task ProcessAsync(IMidiMessage message, Func<IMidiMessage, Task> next)
  {
    _device?.Send(message);
    await next(message);
  }

  public Task Initialize(Connection connection, ILogger? logger = null)
  {
    if (string.IsNullOrEmpty(PortName) && !string.IsNullOrEmpty(connection.DefaultOutputPort))
      PortName = connection.DefaultOutputPort;
    _device = OutputMidiDevicePool.Instance.Open(PortName);
    return Task.CompletedTask;
  }

  public async Task Deinitialize()
  {
    if (_device == null) return;
    OutputMidiDevicePool.Instance.Close(_device);
    await Task.CompletedTask;
  }
}