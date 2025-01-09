using CommandLine;

namespace Hsp.MidiProxy;

public class Arguments
{
  [Option('e', "enable", Required = false, HelpText = "Enable the MIDI proxy.")]
  public bool AutoEnable { get; set; }
}