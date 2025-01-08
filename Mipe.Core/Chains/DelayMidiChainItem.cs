using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class DelayMidiChainItem : IMidiChainItem
{
  public int Milliseconds { get; set; }


  public async Task ProcessAsync(IMidiMessage message, Func<IMidiMessage, Task> next)
  {
    var ms = Milliseconds;
    if (ms > 0)
      await Task.Delay(TimeSpan.FromMilliseconds(ms));
    await next(message);
  }

  public Task Initialize(Connection connection, ILogger? logger = null)
  {
    return Task.CompletedTask;
  }

  public Task Deinitialize()
  {
    return Task.CompletedTask;
  }
}