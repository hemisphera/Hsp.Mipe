using Hsp.Midi.Messages;

namespace Mipe.Service.Models;

public class HttpMidiMessage
{
  public int? Message { get; set; }
  
  public IMidiMessage? ToMidiMessage()
  {
    if (Message != null) return new ChannelMessage(Message.Value);
    return null;
  }
}