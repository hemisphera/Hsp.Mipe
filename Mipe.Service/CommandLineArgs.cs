using CommandLine;

namespace Mipe.Service;

public class CommandLineArgs
{
  [Option('f', "file", Required = true, HelpText = "Path to the configuration file.")]
  public string File { get; set; }
}