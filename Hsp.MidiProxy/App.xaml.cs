using System;
using System.Linq;
using System.Windows;
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
    Configuration.Configuration.Instance.Load();
    foreach (var pipe in Configuration.Configuration.Instance.Items)
    {
      try
      {
        MidiProxy.Main.Instance.Items.Add(new MidiPipeModel(pipe.InputDeviceName, pipe.OutputDeviceName));
      }
      catch (Exception ex)
      {
        // ignore
      }
    }
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