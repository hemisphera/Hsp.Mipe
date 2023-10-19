using System.Windows;

namespace Hsp.MidiProxy;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

  public Main ViewModel => (Main)DataContext;

  public MainWindow()
  {
    InitializeComponent();
    DataContext = Main.Instance;
  }

}