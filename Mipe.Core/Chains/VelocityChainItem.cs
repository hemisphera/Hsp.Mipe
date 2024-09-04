using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class VelocityChainItem : IMidiChainItem
{
  private ILogger? _logger;
  public Range? Range { get; set; }

  public async Task<IMidiMessage[]> ProcessAsync(IMidiMessage message)
  {
    if (message is not ChannelMessage cm) return [message];
    if (cm.Command != ChannelCommand.NoteOn && cm.Command != ChannelCommand.NoteOff) return [message];
    var cm2 = new ChannelMessage(cm.Command, cm.Channel, cm.Data1, Range?.Limit(cm.Data2) ?? cm.Data2);
    _logger?.LogDebug("Modified velocity from {v1} to {v2}", cm.Data2, cm2.Data2);
    return await Task.FromResult<IMidiMessage[]>([cm2]);
  }

  public Task Initialize(ILogger? logger = null)
  {
    _logger = logger;
    return Task.CompletedTask;
  }

  public Task Deinitialize()
  {
    return Task.CompletedTask;
  }
}