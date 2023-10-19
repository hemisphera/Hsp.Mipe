using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eos.Mvvm;
using Hsp.Midi;

namespace Hsp.MidiProxy;

public class MidiDeviceList : AsyncItemsViewModelBase<MidiDeviceInfo>
{

  public bool Input { get; }

  public MidiDeviceList(bool input)
  {
    Input = input;
  }


  protected override async Task<IEnumerable<MidiDeviceInfo>> GetItems()
  {
    if (Input)
      return await Task.FromResult(InputMidiDevice.Enumerate());
    else
      return await Task.FromResult(OutputMidiDevice.Enumerate());
  }

}