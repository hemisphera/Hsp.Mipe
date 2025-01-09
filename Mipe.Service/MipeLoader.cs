using Mipe.Core;

namespace Mipe.Service;

public sealed class MipeLoader : IAsyncDisposable
{
  private readonly ILoggerFactory _loggerFactory;
  private readonly ILogger<MipeLoader> _logger;

  public MipeInstance? Instance { get; private set; }
  public string? CurrentFilePath { get; private set; }


  public MipeLoader(
    ILoggerFactory loggerFactory
  )
  {
    _logger = loggerFactory.CreateLogger<MipeLoader>();
    _loggerFactory = loggerFactory;
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

    if (string.IsNullOrEmpty(filePath)) return;

    try
    {
      _logger.LogInformation("Loading configuration from {path}", filePath);
      Instance = MipeInstance.Load(filePath);
      CurrentFilePath = filePath;
      Instance.LoggerFactory = _loggerFactory;
      _logger.LogInformation("Starting configuration");
      await Instance.Start();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load Mipe configuration");
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