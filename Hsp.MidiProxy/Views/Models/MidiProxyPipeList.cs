using System.Threading.Tasks;
using Eos.Mvvm;
using Eos.Mvvm.Commands;
using Hsp.Midi;
using Hsp.MidiProxy.Storage;

namespace Hsp.MidiProxy.Views;

public sealed class MidiProxyPipeList : MappedAsyncItemsViewModelBase<StorageMidiProxyPipe, MidiProxyPipe>
{
  public static MidiProxyPipeList Instance { get; } = new();

  public VirtualMidiPort VirtualMidiOutput { get; }

  public VirtualMidiPort VirtualMidiInput { get; }


  private MidiProxyPipeList()
    : base(Configuration.Instance.Items, ConvertFrom, ConvertTo)
  {
    VirtualMidiInput = VirtualMidiPort.Create("MidiProxy Input");
    VirtualMidiOutput = VirtualMidiPort.Create("MidiProxy Output");
  }


  private static StorageMidiProxyPipe ConvertTo(MidiProxyPipe arg)
  {
    return arg.StorageItem;
  }

  private static MidiProxyPipe ConvertFrom(StorageMidiProxyPipe arg)
  {
    var vm = new MidiProxyPipe(arg);
    return vm;
  }


  public async Task Add()
  {
    var item = new MidiProxyPipe();
    await DispatchAsync(() => Items.Add(item));
  }

  public async Task Remove(MidiProxyPipe item = null)
  {
    item ??= SelectedItem;
    if (item is null) return;

    if (item.IsOpen) return;
    await DispatchAsync(() => Items.Remove(item));
  }


  public async Task ConnectAll()
  {
    foreach (var item in Items)
      item.Connect();
    await Task.CompletedTask;
  }

  public async Task DisconnectAll()
  {
    foreach (var item in Items)
      item.Disconnect();
    await Task.CompletedTask;
  }

  public override void Dispose()
  {
    VirtualMidiInput?.Dispose();
    VirtualMidiOutput?.Dispose();
  }

  public async Task Initialize(Arguments args)
  {
    await Task.WhenAll(
      MidiDeviceList.OutputDevices.Refresh(),
      MidiDeviceList.InputDevices.Refresh()
    );

    Configuration.Instance.Load();
    ItemsAsMappedCollection?.UpdateFromSource();

    if (args.AutoEnable)
      await ConnectAll();
  }
}