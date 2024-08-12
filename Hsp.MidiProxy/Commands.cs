using System.Threading.Tasks;
using Eos.Mvvm.Commands;
using Hsp.MidiProxy.Views;

namespace Hsp.MidiProxy;

internal static class Commands
{
  public static readonly UiCommand RemovePipe = new UiCommand
  {
    Caption = "Remove Pipe",
    ExecuteFunction = async parameter =>
    {
      if (parameter is not MidiProxyPipe pipe) return;
      await MidiProxyPipeList.Instance.Remove(pipe);
    },
    CanExecuteCallback = parameter => parameter is MidiProxyPipe { IsOpen: false }
  };

  public static readonly UiCommand AddPipe = new UiCommand
  {
    Caption = "Add Pipe",
    ExecuteFunction = async parameter => { await MidiProxyPipeList.Instance.Add(); }
  };

  public static readonly UiCommand ConnectPipe = new UiCommand
  {
    Caption = "Connect Pipe",
    ExecuteFunction = async parameter =>
    {
      if (parameter is not MidiProxyPipe pipe) return;
      pipe.Connect();
      await Task.CompletedTask;
    },
    CanExecuteCallback = parameter => parameter is MidiProxyPipe { IsOpen: false }
  };

  public static readonly UiCommand DisconnectPipe = new UiCommand
  {
    Caption = "Disconnect Pipe",
    ExecuteFunction = async parameter =>
    {
      if (parameter is not MidiProxyPipe pipe) return;
      pipe.Disconnect();
      await Task.CompletedTask;
    },
    CanExecuteCallback = parameter => parameter is MidiProxyPipe { IsOpen: true }
  };

  public static readonly UiCommand ConnectAllPipes = new UiCommand
  {
    Caption = "Connect Pipe",
    ExecuteFunction = async _ => { await MidiProxyPipeList.Instance.ConnectAll(); }
  };

  public static readonly UiCommand DisconnectAllPipes = new UiCommand
  {
    Caption = "Disconnect Pipe",
    ExecuteFunction = async _ => { await MidiProxyPipeList.Instance.DisconnectAll(); }
  };
}