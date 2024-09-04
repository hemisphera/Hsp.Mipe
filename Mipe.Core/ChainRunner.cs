using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;
using Mipe.Core.Chains;

namespace Mipe.Core;

internal static class ChainRunner
{
  public static async Task Process(this IMidiChainItem[] chain, IMidiMessage message, ILogger? logger = null)
  {
    if (chain.Length == 0) return;

    IMidiMessage[] messages = [message];
    foreach (var item in chain)
    {
      List<IMidiMessage> nextBatch = [];
      foreach (var m in messages)
      {
        nextBatch.AddRange(await item.ProcessAsync(m));
      }

      messages = nextBatch.ToArray();
    }
  }
}