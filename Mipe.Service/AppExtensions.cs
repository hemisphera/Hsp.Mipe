using Microsoft.Extensions.Options;

namespace Mipe.Service;

internal static class AppExtensions
{
  public static async Task Initialize(this WebApplication app)
  {
    var loader = app.Services.GetRequiredService<MipeLoader>();
    loader.Listen();

    var om = app.Services.GetRequiredService<IOptionsMonitor<MipeServiceSettings>>();
    await LoadInitialConfiguration(loader, om.CurrentValue);
  }

  private static async Task LoadInitialConfiguration(MipeLoader loader, MipeServiceSettings om)
  {
    if (string.IsNullOrEmpty(om.ConfigurationFilePath)) return;
    await loader.LoadConfiguration(om.ConfigurationFilePath);
  }
}