using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eos.Mvvm;
using Eos.Mvvm.Commands;

namespace Hsp.MidiProxy;

public class Main : AsyncItemsViewModelBase<MidiPipeModel>
{
  public static Main Instance { get; } = new();


  public UiCommand AddCommand => GetAutoFieldValue(() => new UiCommand
  {
    ExecuteFunction = async _ => await Add()
  });

  public UiCommand RemoveCommand => GetAutoFieldValue(() => new UiCommand
  {
    ExecuteFunction = async parameter => await Remove(parameter as MidiPipeModel)
  });

  public UiCommand ConnectAllCommand => GetAutoFieldValue(() => new UiCommand
  {
    ExecuteFunction = async _ => await ConnectAll()
  });

  public UiCommand DisconnectAllCommand => GetAutoFieldValue(() => new UiCommand
  {
    ExecuteFunction = async _ => await DisconnectAll()
  });


  private Main()
  {
  }


  protected override async Task<IEnumerable<MidiPipeModel>> GetItems()
  {
    Configuration.Configuration.Instance.Load();
    var items = new List<MidiPipeModel>();
    foreach (var pipe in Configuration.Configuration.Instance.Items)
    {
      try
      {
        items.Add(await MidiPipeModel.Create(pipe.InputDeviceName, pipe.OutputDeviceName));
      }
      catch (Exception)
      {
        // ignore
      }
    }

    return await Task.FromResult(items);
  }

  public override async Task Refresh()
  {
    await base.Refresh();
    if (Arguments.Instance.AutoEnable)
      await ConnectAll();
  }


  public async Task Add()
  {
    var item = await MidiPipeModel.Create();
    await DispatchAsync(() => Items.Add(item));
    await Refresh();
  }

  public async Task Remove(MidiPipeModel item = null)
  {
    item ??= SelectedItem;
    if (item is null) return;

    if (item.IsOpen) return;
    await DispatchAsync(() => Items.Remove(item));
    await Refresh();
  }


  private async Task ConnectAll()
  {
    foreach (var item in Items)
      item.Connect();
    await Task.CompletedTask;
  }

  private async Task DisconnectAll()
  {
    foreach (var item in Items)
      item.Disconnect();
    await Task.CompletedTask;
  }
}