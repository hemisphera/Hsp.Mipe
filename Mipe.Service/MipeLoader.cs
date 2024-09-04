using Microsoft.Extensions.Options;
using Mipe.Core;

namespace Mipe.Service;

public sealed class MipeLoader : IAsyncDisposable
{
  private readonly IConfiguration _root;
  private readonly ILoggerFactory _loggerFactory;
  private readonly ILogger<MipeLoader> _logger;
  private readonly IOptionsMonitor<MipeServiceSettings> _optionsMonitor;
  private MipeInstance? _instance;


  public MipeLoader(
    IConfiguration root,
    ILoggerFactory loggerFactory,
    IOptionsMonitor<MipeServiceSettings> optionsMonitor
  )
  {
    _root = root;
    _logger = loggerFactory.CreateLogger<MipeLoader>();
    _loggerFactory = loggerFactory;
    _optionsMonitor = optionsMonitor;
  }


  public void Listen()
  {
    var token = _root.GetReloadToken();
    token.RegisterChangeCallback(_ =>
    {
      _logger.LogInformation("Configuration change detected");
      Task.Run(async () =>
      {
        await Task.Delay(TimeSpan.FromSeconds(1));
        await LoadConfiguration();
        Listen();
      });
    }, null);
  }

  public async Task LoadConfiguration()
  {
    var val = _optionsMonitor.CurrentValue;
    _logger.LogInformation("Current file is {filePath}", val.ConfigurationFilePath);

    if (_instance != null)
    {
      _logger.LogInformation("Stopping active configuration");
      await _instance.Stop();
      _instance = null;
    }

    try
    {
      _logger.LogInformation("Loading configuration from {path}", val.ConfigurationFilePath);
      _instance = MipeInstance.Load(val.ConfigurationFilePath);
      _instance.LoggerFactory = _loggerFactory;
      _logger.LogInformation("Starting configuration");
      await _instance.Start();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load Mipe configuration");
    }

    _logger.LogInformation("Done");
  }

  public async ValueTask DisposeAsync()
  {
    if (_instance != null)
    {
      await _instance.Stop();
    }
  }
} 