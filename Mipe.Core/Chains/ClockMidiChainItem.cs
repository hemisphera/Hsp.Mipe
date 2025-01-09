using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class ClockMidiChainItem : IMidiChainItem
{
  private MidiClock? _clock;

  public async Task ProcessAsync(IMidiMessage message, Func<IMidiMessage, Task> next)
  {
    if (message is SysRealtimeMessage { SysRealtimeType: SysRealtimeType.Start })
      _clock?.Start();
    if (message is SysRealtimeMessage { SysRealtimeType: SysRealtimeType.Stop })
      _clock?.Stop();
    if (message is SysRealtimeMessage { SysRealtimeType: SysRealtimeType.Clock })
      _clock?.Tick();
    await next(message);
  }

  public Task Initialize(Connection connection, ILogger? logger = null)
  {
    _clock = connection.Owner?.Clock;
    return Task.CompletedTask;
  }

  public Task Deinitialize()
  {
    return Task.CompletedTask;
  }
}