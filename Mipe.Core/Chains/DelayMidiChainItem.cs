using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class DelayMidiChainItem : IMidiChainItem
{
  public int Milliseconds { get; set; }


  public async Task<IMidiMessage[]> ProcessAsync(IMidiMessage message)
  {
    await Task.Delay(TimeSpan.FromMilliseconds(Milliseconds));
    return [message];
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