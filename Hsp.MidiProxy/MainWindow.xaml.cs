using System.Windows;
using Hsp.MidiProxy.Views;

namespace Hsp.MidiProxy;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
  public MainWindow()
  {
    InitializeComponent();
    DataContext = ContentHost.Instance;
  }
}