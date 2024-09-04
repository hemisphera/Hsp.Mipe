using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class ForkChainItem : IMidiChainItem
{
  public IMidiChainItem[][]? SubChains { get; set; }

  public async Task<IMidiMessage[]> ProcessAsync(IMidiMessage message)
  {
    if (SubChains == null || SubChains.Length == 0) return [];
    await Task.WhenAll(SubChains.Select(subChain => subChain.Process(message)));
    return [];
  }

  public async Task Initialize(ILogger? logger = null)
  {
    if (SubChains == null || SubChains.Length == 0) return;
    var subchainItems = SubChains.SelectMany(subChain => subChain).ToArray();
    await Task.WhenAll(subchainItems.Select(subChain => subChain.Initialize(logger)));
  }

  public async Task Deinitialize()
  {
    if (SubChains == null || SubChains.Length == 0) return;
    var subchainItems = SubChains.SelectMany(subChain => subChain).ToArray();
    await Task.WhenAll(subchainItems.Select(subChain => subChain.Deinitialize()));
  }
}