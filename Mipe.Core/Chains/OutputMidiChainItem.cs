using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class OutputMidiChainItem : IMidiChainItem
{
  public string PortName { get; set; } = string.Empty;

  private ILogger? _logger;

  public bool PassThrough { get; set; }

  private OutputMidiDevice? _device;

  public Task<IMidiMessage[]> ProcessAsync(IMidiMessage message)
  {
    if (_device != null && _logger?.IsEnabled(LogLevel.Debug) == true)
    {
      _logger.LogMidi(_device, message);
    }

    _device?.Send(message);
    return Task.FromResult(PassThrough ? new[] { message } : []);
  }

  public Task Initialize(ILogger? logger = null)
  {
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