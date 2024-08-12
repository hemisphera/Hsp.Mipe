using Eos.Mvvm;

namespace Hsp.MidiProxy;

public class ContentHost : ObservableEntity
{
  public static ContentHost Instance { get; } = new();

  public object Content
  {
    get => GetAutoFieldValue<object>();
    set => SetAutoFieldValue(value);
  }

  private ContentHost()
  {
  }
}