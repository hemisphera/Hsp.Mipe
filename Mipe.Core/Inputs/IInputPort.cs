using Hsp.Midi.Messages;

namespace Mipe.Core.Inputs;

public interface IInputPort
{
  Task Connect();

  Task Disconnect();

  event EventHandler<IMidiMessage> MessageReceived;
}