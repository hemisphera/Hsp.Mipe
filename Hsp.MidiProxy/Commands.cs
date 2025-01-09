using System;
using System.Threading.Tasks;
using Eos.Mvvm.Commands;
using Hsp.MidiProxy.Views;

namespace Hsp.MidiProxy;

internal static class Commands
{
  private static async Task RunWithCatch(Func<Task> func)
  {
    try
    {
      await func();
    }
    catch (Exception ex)
    {
      Logger.WriteLog(ex.Message);
    }
  }

  public static readonly UiCommand RemovePipe = new()
  {
    Caption = "Remove Pipe",
    ExecuteFunction = async parameter => await RunWithCatch(async () =>
    {
      if (parameter is not MidiProxyPipe pipe) return;
      await MidiProxyPipeList.Instance.Remove(pipe);
    }),
    CanExecuteCallback = parameter => parameter is MidiProxyPipe { IsOpen: false }
  };

  public static readonly UiCommand AddPipe = new()
  {
    Caption = "Add Pipe",
    ExecuteFunction = async _ => await RunWithCatch(async () => { await MidiProxyPipeList.Instance.Add(); })
  };

  public static readonly UiCommand ConnectPipe = new()
  {
    Caption = "Connect Pipe",
    ExecuteFunction = async parameter => await RunWithCatch(async () =>
    {
      if (parameter is not MidiProxyPipe pipe) return;
      pipe.Connect();
      await Task.CompletedTask;
    }),
    CanExecuteCallback = parameter => parameter is MidiProxyPipe { IsOpen: false }
  };

  public static readonly UiCommand DisconnectPipe = new()
  {
    Caption = "Disconnect Pipe",
    ExecuteFunction = async parameter => await RunWithCatch(async () =>
    {
      if (parameter is not MidiProxyPipe pipe) return;
      pipe.Disconnect();
      await Task.CompletedTask;
    }),
    CanExecuteCallback = parameter => parameter is MidiProxyPipe { IsOpen: true }
  };

  public static readonly UiCommand ConnectAllPipes = new()
  {
    Caption = "Connect Pipe",
    ExecuteFunction = async _ => await RunWithCatch(async () => { await MidiProxyPipeList.Instance.ConnectAll(); })
  };

  public static readonly UiCommand DisconnectAllPipes = new()
  {
    Caption = "Disconnect Pipe",
    ExecuteFunction = async _ => await RunWithCatch(async () => { await MidiProxyPipeList.Instance.DisconnectAll(); })
  };

  public static readonly UiCommand SelectTrackCommand = new()
  {
    Caption = "SelectTrack",
    ExecuteFunction = async parameter => await RunWithCatch(async () =>
    {
      var trackNo = int.Parse((string)parameter);
      await MidiProxyPipeList.Instance.SelectTrack(trackNo);
    })
  };
}