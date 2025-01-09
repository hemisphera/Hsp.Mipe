using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public interface IMidiChainItem
{
  Task ProcessAsync(IMidiMessage message, Func<IMidiMessage, Task> next);
  Task Initialize(Connection connection, ILogger? logger = null);
  Task Deinitialize();
}