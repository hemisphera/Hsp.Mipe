using CommandLine;

namespace Hsp.MidiProxy;

public class Arguments
{
  public static Arguments Instance { get; set; }


  [Option('e', "enable", Required = false, HelpText = "Enable the MIDI proxy.")]
  public bool AutoEnable { get; set; }
}