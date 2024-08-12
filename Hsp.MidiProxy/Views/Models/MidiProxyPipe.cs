using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
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

  private MidiPipe _pipe;

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
    if (!Configuration.Instance.EnableLogging) return;
    lock (_syncRoot)
      WriteMidiLog(e);
  }

  private static void WriteMidiLog(IMidiMessage midiMessage)
  {
    if (midiMessage is ChannelMessage cm && (cm.Channel > 0 || cm.Command == ChannelCommand.Controller)) return;
    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MidiLog.txt");
    var line = new[]
    {
      DateTime.Now.ToLongTimeString(),
      midiMessage.ToString()
    };
    File.AppendAllText(path, string.Join("\t", line) + Environment.NewLine);
  }

  public void Test()
  {
    ToggleRecord(true);
  }

  private void ToggleRecord(bool overdub)
  {
    if (overdub)
      SetShift(true);
    _pipe.OutputMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, 92, 127));
    _pipe.OutputMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, 92));
    if (overdub)
      SetShift(false);
  }

  private void SetShift(bool b)
  {
    if (b)
      _pipe.OutputMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, 120, 127));
    else
    {
      _pipe.OutputMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, 120));
      _pipe.OutputMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, 75, 1));
    }
  }

  public bool Connect()
  {
    try
    {
      if (IsOpen) return true;
      if (SelectedInputDevice == null || SelectedOutputDevice == null) return false;
      _pipe = new MidiPipe(SelectedInputDevice.Name, SelectedOutputDevice.Name);
      _pipe.MessageReceived += PipeOnMessageReceived;
      IsOpen = true;
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
    _pipe.MessageReceived -= PipeOnMessageReceived;
    _pipe.Close();
    _pipe = null;
    IsOpen = false;
  }


  public override string ToString()
  {
    return _pipe?.ToString() ?? "Closed";
  }
}