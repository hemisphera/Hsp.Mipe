using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class MessageMidiChainItem : IMidiChainItem
{
  public ChannelMessage[] Messages { get; set; }


  public async Task<IMidiMessage[]> ProcessAsync(IMidiMessage message)
  {
    return await Task.FromResult(Messages);
  }

  public Task Initialize(ILogger? logger = null)
  {
    return Task.CompletedTask;
  }

  public Task Deinitialize()
  {
    return Task.CompletedTask;
  }
}