using Hsp.Midi;
using Hsp.Midi.Messages;

namespace Mipe.Core.Inputs;

public class MidiInputPort : IInputPort
{
  private InputMidiDevice? _device;
  public string PortName { get; }


  public event EventHandler<IMidiMessage>? MessageReceived;


  public MidiInputPort(string portName)
  {
    PortName = portName;
  }


  public async Task Connect()
  {
    ArgumentException.ThrowIfNullOrEmpty(PortName, nameof(PortName));
    _device = InputMidiDevicePool.Instance.Open(PortName);
    _device.MessageReceived += DeviceOnMessageReceived;
    await Task.CompletedTask;
  }

  private void DeviceOnMessageReceived(object? sender, IMidiMessage e)
  {
    MessageReceived?.Invoke(this, e);
  }

  public async Task Disconnect()
  {
    if (_device == null) return;
    InputMidiDevicePool.Instance.Close(_device);
    await Task.CompletedTask;
  }
}