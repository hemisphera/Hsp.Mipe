using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommandLine;
using Eos.Mvvm;
using Eos.Mvvm.DataTemplates;
using Eos.Mvvm.EventArgs;
using Hsp.MidiProxy.Storage;
using Hsp.MidiProxy.Views;

namespace Hsp.MidiProxy;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
  public App()
  {
    UiSettings.ViewLocator = new ViewLocator(GetType().Assembly);
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    var args = Parser.Default.ParseArguments<Arguments>(e.Args).Value;
    Task.Run(async () =>
    {
      await MidiProxyPipeList.Instance.Initialize(args);
      await UiSettings.GetDispatcher().InvokeAsync(() => { ContentHost.Instance.Content = MidiProxyPipeList.Instance; });
    });
  }

  private void InverseBooleanConverter_OnOnConvert(object sender, ConverterEventArgs e)
  {
    e.Result = e.Value is false;
  }

  protected override void OnExit(ExitEventArgs e)
  {
    Configuration.Instance.Save();
  }
}