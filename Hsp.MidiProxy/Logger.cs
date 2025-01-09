using System;
using System.IO;
using Hsp.Midi;
using Hsp.Midi.Messages;
using Hsp.MidiProxy.Storage;

namespace Hsp.MidiProxy;

internal static class Logger
{
  private static readonly object SyncRoot = new();

  public static void WriteLog(MidiDevice device, IMidiMessage midiMessage)
  {
    if (midiMessage is ChannelMessage cm && (cm.Channel > 0 || cm.Command == ChannelCommand.Controller)) return;
    var line = new object[]
    {
      DateTime.Now.ToLongTimeString(),
      device.Name,
      midiMessage
    };
    WriteLog(line);
  }

  public static void WriteLog(params object[] parts)
  {
    WriteLog(string.Join("\t", parts));
  }

  public static void WriteLog(string line)
  {
    if (!Configuration.Instance.EnableLogging) return;
    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MidiLog.txt");
    lock (SyncRoot)
      File.AppendAllText(path, line + Environment.NewLine);
  }
}