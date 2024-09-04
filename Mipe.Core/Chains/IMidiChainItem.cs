using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public interface IMidiChainItem
{
  Task<IMidiMessage[]> ProcessAsync(IMidiMessage message);
  Task Initialize(ILogger? logger = null);
  Task Deinitialize();
}