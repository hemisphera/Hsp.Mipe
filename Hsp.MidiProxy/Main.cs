using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;
using Eos.Mvvm;
using Eos.Mvvm.Commands;

namespace Hsp.MidiProxy;

public class Main : AsyncItemsViewModelBase<MidiPipeModel>
{

  public static Main Instance { get; } = new();


  public UiCommand AddCommand => GetAutoFieldValue(() => new UiCommand
  {
    ExecuteFunction = async parameter => Add()
  });

  public UiCommand RemoveCommand => GetAutoFieldValue(() => new UiCommand
  {
    ExecuteFunction = async parameter => Remove(parameter as MidiPipeModel)
  });

  public UiCommand ConnectAllCommand => GetAutoFieldValue(() => new UiCommand
  {
    ExecuteFunction = async parameter => ConnectAll()
  });

  public UiCommand DisconnectAllCommand => GetAutoFieldValue(() => new UiCommand
  {
    ExecuteFunction = async parameter => DisconnectAll()
  });



  private Main()
  {
  }



  protected override async Task<IEnumerable<MidiPipeModel>> GetItems()
  {
    return await Task.FromResult(Items);
  }


  public async Task Add()
  {
    await DispatchAsync(() => Items.Add(new MidiPipeModel()));
    await Refresh();
  }

  public async Task Remove(MidiPipeModel item = null)
  {
    if (item == null) item = SelectedItem;
    if (item is null) return;

    if (item.IsOpen) return;
    await DispatchAsync(() => Items.Remove(item));
    await Refresh();
  }


  private void ConnectAll()
  {
    foreach (var item in Items)
      item.Connect();
  }

  private void DisconnectAll()
  {
    foreach (var item in Items)
      item.Disconnect();
  }

}