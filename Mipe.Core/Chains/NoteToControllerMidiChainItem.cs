using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class NoteToControllerMidiChainItem : IMidiChainItem
{
  public int? Channel { get; set; }

  public int ControllerNumber { get; set; }

  public NoteToCcValueType Value { get; set; }


  public Task<IMidiMessage[]> ProcessAsync(IMidiMessage message)
  {
    if (message is not ChannelMessage cm)
    {
      return Task.FromResult(Array.Empty<IMidiMessage>());
    }

    var value = 0;
    switch (Value)
    {
      case NoteToCcValueType.NoteNumber:
        value = cm.Data1;
        break;
      case NoteToCcValueType.Velocity:
        value = cm.Data2;
        break;
    }

    var resultMessage = new ChannelMessage(ChannelCommand.Controller, Channel ?? cm.Channel, ControllerNumber, value);
    return Task.FromResult(new IMidiMessage[] { resultMessage });
  }

  public Task Initialize(ILogger? logger = null)
  {
    return Task.CompletedTask;
  }

  public Task Deinitialize()
  {
    return Task.CompletedTask;
  }
}