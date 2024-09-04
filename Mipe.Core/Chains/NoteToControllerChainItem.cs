using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class NoteToControllerChainItem : IMidiChainItem
{
  /// <summary>
  /// Specifies the channel to use. If null, the channel of the incoming message is used.
  /// </summary>
  public int? Channel { get; set; }

  /// <summary>
  /// Specifies the controller number to use.
  /// </summary>
  public int ControllerNumber { get; set; }

  /// <summary>
  /// Specifies how the controller value should be calculated.
  /// </summary>
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