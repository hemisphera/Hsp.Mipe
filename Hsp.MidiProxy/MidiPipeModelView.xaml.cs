using Hsp.Midi.Messages;
using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Hsp.Midi;

namespace Hsp.MidiProxy;

/// <summary>
/// Interaction logic for MidiPipeModelView.xaml
/// </summary>
public partial class MidiPipeModelView : UserControl
{

  public Main ViewModel => (Main)DataContext;


  public MidiPipeModelView()
  {
    InitializeComponent();
    this.DataContextChanged += MidiPipeModelView_DataContextChanged;
  }

  private void MidiPipeModelView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue is MidiPipeModel nm) nm.MessageReceived += Nm_MessageReceived;
    if (e.OldValue is MidiPipeModel om) om.MessageReceived -= Nm_MessageReceived;
  }

  private void Nm_MessageReceived(object sender, IMidiMessage e)
  {
    Ellipse targetLed = null;
    if (e is SysExMessage) targetLed = SysExLed;
    if (e is ChannelMessage { Command: ChannelCommand.NoteOn }) targetLed = NoteOnOffLed;
    if (e is ChannelMessage { Command: ChannelCommand.NoteOff }) targetLed = NoteOnOffLed;
    if (e is ChannelMessage { Command: ChannelCommand.Controller }) targetLed = ControllerLed;

    if (targetLed is null) return;
    Dispatcher.Invoke(() => (FindResource("sb") as Storyboard)?.Begin(targetLed));
  }

}