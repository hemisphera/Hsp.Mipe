using Hsp.Midi;
using Hsp.Midi.Messages;

namespace Mipe.Core.Tests;

public class MidiEncoder
{
  [Theory]
  [InlineData(0, ChannelCommand.NoteOn, 60, 100)]
  //[InlineData(0, ChannelCommand.NoteOff, 60, 100)]
  public void MidiEncoderNoteOn(int channel, ChannelCommand command, int data1, int data2)
  {
    var msg = new ChannelMessage(command, channel, data1, data2);
    var builtMessage = msg.Message;

    var statusByte = ((int)command & 0xF0) | (channel & 0x0F);
    var calcMessage = data2 << 16;
    calcMessage |= data1 << 8;
    calcMessage |= statusByte;

    var af = new ChannelMessage(calcMessage);
    var af2 = new ChannelMessage(builtMessage);

    Assert.Equal(builtMessage, calcMessage);
  }
}