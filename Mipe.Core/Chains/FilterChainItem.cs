using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class FilterChainItem : IMidiChainItem
{
  private ILogger? _logger;

  /// <summary>
  /// Specifies the channel(s) to allow. If not specified or empty, all channels are allowed.
  /// </summary>
  public Range[]? Channel { get; set; }

  /// <summary>
  /// Specifies the message type(s) to allow. If not specified or empty, all message types are allowed.
  /// </summary>
  public MidiMessageType[]? MessageType { get; set; }

  /// <summary>
  /// Specifies the value(s) to allow. If not specified or empty, all values are allowed.
  /// </summary>
  public Range[]? Data1 { get; set; }

  /// <summary>
  /// Specifies the value(s) to allow. If not specified or empty, all values are allowed.
  /// </summary>
  public Range[]? Data2 { get; set; }


  public Task<IMidiMessage[]> ProcessAsync(IMidiMessage message)
  {
    var messageMatches =
      ChannelMatches(message) &&
      MessageTypeMatches(message) &&
      Data1Matches(message) &&
      Data2Matches(message);
    _logger?.LogDebug("FilterMidiChainItem: {message} {matches}", message, messageMatches);
    return Task.FromResult<IMidiMessage[]>(messageMatches ? [message] : []);
  }

  private bool ChannelMatches(IMidiMessage message)
  {
    if (Channel == null || Channel.Length == 0) return true;
    if (message is not ChannelMessage cm) return false;
    return Channel.Any(r => r.Contains(cm.Channel));
  }

  private bool MessageTypeMatches(IMidiMessage message)
  {
    if (MessageType == null || MessageType.Length == 0) return true;
    var isSysex = message is SysExMessage or SysCommonMessage or SysRealtimeMessage;
    if (isSysex)
      return MessageType.Contains(MidiMessageType.SysEx);
    if (message is not ChannelMessage cm) return false;
    return cm.Command switch
    {
      ChannelCommand.NoteOff => MessageType.Contains(MidiMessageType.NoteOff),
      ChannelCommand.NoteOn => MessageType.Contains(MidiMessageType.NoteOn),
      ChannelCommand.PolyPressure => MessageType.Contains(MidiMessageType.PolyPressure),
      ChannelCommand.Controller => MessageType.Contains(MidiMessageType.Controller),
      ChannelCommand.ProgramChange => MessageType.Contains(MidiMessageType.ProgramChange),
      ChannelCommand.ChannelPressure => MessageType.Contains(MidiMessageType.ChannelPressure),
      ChannelCommand.PitchWheel => MessageType.Contains(MidiMessageType.PitchWheel),
      _ => false
    };
  }

  private bool Data1Matches(IMidiMessage message)
  {
    if (Data1 == null || Data1.Length == 0) return true;
    return message is ChannelMessage cm && Data1.Any(r => r.Contains(cm.Data1));
  }

  private bool Data2Matches(IMidiMessage message)
  {
    if (Data2 == null || Data2.Length == 0) return true;
    return message is ChannelMessage cm && Data2.Any(r => r.Contains(cm.Data2));
  }

  public Task Initialize(ILogger? logger = null)
  {
    _logger = logger;
    return Task.CompletedTask;
  }

  public Task Deinitialize()
  {
    return Task.CompletedTask;
  }
}