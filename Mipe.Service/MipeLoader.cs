using Microsoft.Extensions.Options;
using Mipe.Core;

namespace Mipe.Service;

public sealed class MipeLoader : IAsyncDisposable
{
  private readonly IConfiguration _root;
  private readonly ILoggerFactory _loggerFactory;
  private readonly ILogger<MipeLoader> _logger;
  private readonly IOptionsMonitor<MipeServiceSettings> _optionsMonitor;

  public MipeInstance? Instance { get; private set; }

  public string? CurrentFilePath { get; private set; }


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
        //await LoadConfiguration();
        Listen();
      });
    }, null);
  }

  public Task LoadConfiguration()
  {
    return LoadConfiguration(CurrentFilePath);
  }

  public async Task LoadConfiguration(string? filePath)
  {
    _logger.LogInformation("Loading file from {filePath}", filePath);

    if (Instance != null)
    {
      _logger.LogInformation("Stopping active configuration");
      await Instance.Stop();
      Instance = null;
    }

    if (!string.IsNullOrEmpty(filePath))
    {
      try
      {
        _logger.LogInformation("Loading configuration from {path}", filePath);
        Instance = MipeInstance.Load(filePath);
        Instance.LoggerFactory = _loggerFactory;
        _logger.LogInformation("Starting configuration");
        await Instance.Start();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to load Mipe configuration");
      }
    }

    _logger.LogInformation("Done");
  }

  public async ValueTask DisposeAsync()
  {
    if (Instance != null)
    {
      await Instance.Stop();
    }
  }
}