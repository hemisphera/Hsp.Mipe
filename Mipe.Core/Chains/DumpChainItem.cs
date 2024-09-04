using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class DumpChainItem : IMidiChainItem
{
  private ILogger? _logger;

  public async Task<IMidiMessage[]> ProcessAsync(IMidiMessage message)
  {
    _logger?.LogMidi(null, message, LogLevel.Information);
    return await Task.FromResult(new[] { message });
  }

  public Task Initialize(ILogger? logger = null)
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