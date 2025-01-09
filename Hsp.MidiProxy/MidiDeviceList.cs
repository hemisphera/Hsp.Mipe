using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Eos.Mvvm;
using Hsp.Midi;

namespace Hsp.MidiProxy;

public class MidiDeviceList : AsyncItemsViewModelBase<MidiDeviceInfo>
{
  public bool Input { get; }


  public static readonly MidiDeviceList InputDevices = new(InputMidiDevicePool.Instance.Enumerate().ToList());

  public static readonly MidiDeviceList OutputDevices = new(OutputMidiDevicePool.Instance.Enumerate().ToList());


  private MidiDeviceList(List<MidiDeviceInfo> list)
    : base(list)
  {
  }


  protected override async Task<IEnumerable<MidiDeviceInfo>> GetItems()
  {
    return await Task.FromResult(Items);
  }
}