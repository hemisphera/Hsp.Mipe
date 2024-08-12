using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Hsp.Midi;
using Hsp.Midi.Messages;

namespace Hsp.MidiProxy.Views;

/// <summary>
/// Interaction logic for MidiPipeModelView.xaml
/// </summary>
public partial class MidiProxyPipeView : UserControl
{
  public MidiProxyPipeList ViewModel => (MidiProxyPipeList)DataContext;


  public MidiProxyPipeView()
  {
    InitializeComponent();
    this.DataContextChanged += MidiPipeModelView_DataContextChanged;
  }

  private void MidiPipeModelView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue is MidiProxyPipe nm) nm.MessageReceived += Nm_MessageReceived;
    if (e.OldValue is MidiProxyPipe om) om.MessageReceived -= Nm_MessageReceived;
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