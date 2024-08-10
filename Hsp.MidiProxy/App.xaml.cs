using System;
using System.Linq;
using System.Windows;
using CommandLine;
using Eos.Mvvm;
using Eos.Mvvm.DataTemplates;
using Eos.Mvvm.EventArgs;
using Hsp.MidiProxy.Configuration;

namespace Hsp.MidiProxy;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  public App()
  {
    UiSettings.ViewLocator = new ViewLocator(GetType().Assembly);
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    Arguments.Instance = Parser.Default.ParseArguments<Arguments>(e.Args).Value;
    _ = MidiProxy.Main.Instance.Refresh();
  }

  protected override void OnExit(ExitEventArgs e)
  {
    Configuration.Configuration.Instance.Items = MidiProxy.Main.Instance
      .Select(i =>
      {
        var inputDeviceName = i.InputDevices.SelectedItem?.Name ?? i.RequestedInputDeviceName;
        var outputDeviceName = i.OutputDevices.SelectedItem?.Name ?? i.RequesteOutputDeviceName;
        if (String.IsNullOrEmpty(inputDeviceName) || String.IsNullOrEmpty(outputDeviceName)) return null;
        return new ConfiguredPipe
        {
          InputDeviceName = inputDeviceName,
          OutputDeviceName = outputDeviceName
        };
      }).Where(i => i != null).ToList();
    Configuration.Configuration.Instance.Save();
  }

  private void InverseBooleanConverter_OnOnConvert(object sender, ConverterEventArgs e)
  {
    e.Result = e.Value is false;
  }
}