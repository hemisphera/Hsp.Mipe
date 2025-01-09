namespace Mipe.Service.FileLogger;

public sealed class CachedFileLoggerProvider : ILoggerProvider
{
  private readonly LogQueue _queue;


  public CachedFileLoggerProvider(string path)
  {
    _queue = new LogQueue(path);
  }


  public ILogger CreateLogger(string categoryName)
  {
    return new CachedFileLogger(categoryName, _queue);
  }

  public void Dispose()
  {
    _queue.Dispose();
  }
}