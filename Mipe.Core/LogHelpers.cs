using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core;

internal static class LogHelpers
{
  public static void LogMidi(this ILogger logger, IInputMidiDevice device, IMidiMessage message)
  {
    var msgType = (message as ChannelMessage)?.Command.ToString() ?? message.GetType().Name;
    logger.LogDebug("RX {port} => {message} {data}", device.Name, msgType, message.ToHexString());
  }

  public static void LogMidi(this ILogger logger, IOutputMidiDevice device, IMidiMessage message)
  {
    var msgType = (message as ChannelMessage)?.Command.ToString() ?? message.GetType().Name;
    logger.LogDebug("TX {port} => {message} {data}", device.Name, msgType, message.ToHexString());
  }
}