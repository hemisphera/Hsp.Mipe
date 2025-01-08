using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class DumpChainItem : IMidiChainItem
{
  private ILogger? _logger;

  public async Task ProcessAsync(IMidiMessage message, Func<IMidiMessage, Task> next)
  {
    _logger?.LogMidi(null, message, LogLevel.Information);
    await next(message);
  }

  public Task Initialize(Connection connection, ILogger? logger = null)
  {
    _logger = logger;
    return Task.CompletedTask;
  }

  public Task Deinitialize()
  {
    _logger = null;
    return Task.CompletedTask;
  }
}