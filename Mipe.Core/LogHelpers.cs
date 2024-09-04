using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core;

internal static class LogHelpers
{
  public static void LogMidi(this ILogger logger, IMidiDevice? device, IMidiMessage message, LogLevel level = LogLevel.Debug)
  {
    var direction = device is IInputMidiDevice ? "RX" : "TX";
    var msgType = (message as ChannelMessage)?.Command.ToString() ?? message.GetType().Name;
    logger.Log(level, "{direction} {port} => {message} {data}", direction, device?.Name, msgType, message.ToHexString());
  }
}