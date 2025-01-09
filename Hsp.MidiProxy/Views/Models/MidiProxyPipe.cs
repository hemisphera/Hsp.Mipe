using System;
using System.Linq;
using Eos.Mvvm;
using Hsp.Midi;
using Hsp.Midi.Messages;
using Hsp.MidiProxy.Storage;

namespace Hsp.MidiProxy.Views;

public class MidiProxyPipe : ViewModelBase
{
  public StorageMidiProxyPipe StorageItem { get; }

  public MidiDeviceInfo SelectedInputDevice
  {
    get => GetAutoFieldValue<MidiDeviceInfo>();
    set
    {
      SetAutoFieldValue(value);
      if (value != null)
        StorageItem.InputDeviceName = value.Name;
    }
  }

  public MidiDeviceInfo SelectedOutputDevice
  {
    get => GetAutoFieldValue<MidiDeviceInfo>();
    set
    {
      SetAutoFieldValue(value);
      if (value != null)
        StorageItem.OutputDeviceName = value.Name;
    }
  }

  public MidiPipe Pipe { get; private set; }

  private readonly object _syncRoot = new();


  public bool IsOpen
  {
    get => GetAutoFieldValue<bool>();
    private set
    {
      SetAutoFieldValue(value);
      //CommandManager.InvalidateRequerySuggested();
      Dispatch(() =>
      {
        Commands.ConnectPipe.RaiseCanExecuteChanged();
        Commands.DisconnectPipe.RaiseCanExecuteChanged();
        Commands.RemovePipe.RaiseCanExecuteChanged();
      });
    }
  }


  public event EventHandler<IMidiMessage> MessageReceived;


  public MidiProxyPipe()
    : this(new StorageMidiProxyPipe())
  {
  }

  public MidiProxyPipe(StorageMidiProxyPipe storageItem)
  {
    StorageItem = storageItem;
    SelectedInputDevice = MidiDeviceList.InputDevices.FirstOrDefault(d => d.Name.Equals(StorageItem.InputDeviceName));
    SelectedOutputDevice = MidiDeviceList.OutputDevices.FirstOrDefault(d => d.Name.Equals(StorageItem.OutputDeviceName));
  }


  private void PipeOnMessageReceived(object sender, IMidiMessage e)
  {
    MessageReceived?.Invoke(this, e);
    Logger.WriteLog(e);
  }

  public void Test()
  {
    ToggleRecord(true);
  }

  private void ToggleRecord(bool overdub)
  {
    if (overdub)
      SetShift(true);
    Pipe.OutputMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, 92, 127));
    Pipe.OutputMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, 92));
    if (overdub)
      SetShift(false);
  }

  private void SetShift(bool b)
  {
    if (b)
      Pipe.OutputMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, 120, 127));
    else
    {
      Pipe.OutputMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, 120));
      Pipe.OutputMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, 75, 1));
    }
  }

  public bool Connect()
  {
    try
    {
      if (IsOpen) return true;
      if (SelectedInputDevice == null || SelectedOutputDevice == null) return false;
      Pipe = new MidiPipe(SelectedInputDevice.Name, SelectedOutputDevice.Name);
      Pipe.MessageReceived += PipeOnMessageReceived;
      IsOpen = true;
      Logger.WriteLog($"Connected pipe '{Pipe}'");
      return true;
    }
    catch
    {
      return false;
    }
  }

  public void Disconnect()
  {
    if (!IsOpen) return;
    Pipe.MessageReceived -= PipeOnMessageReceived;
    Pipe.Close();
    Logger.WriteLog($"Disconnected pipe '{Pipe}'");
    Pipe = null;
    IsOpen = false;
  }


  public override string ToString()
  {
    return Pipe?.ToString() ?? "Closed";
  }
}