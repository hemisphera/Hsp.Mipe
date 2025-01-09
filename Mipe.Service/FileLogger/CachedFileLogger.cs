using System.Text;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.FileLogger;

public class CachedFileLogger : ILogger
{
  private readonly LogQueue _queue;
  private readonly string _category;

  public CachedFileLogger(string category, LogQueue queue)
  {
    _queue = queue;
    _category = category;
  }

  private static string GetShortLogLevel(LogLevel logLevel)
  {
    switch (logLevel)
    {
      case LogLevel.Trace:
        return "TRCE";
      case LogLevel.Debug:
        return "DBUG";
      case LogLevel.Information:
        return "INFO";
      case LogLevel.Warning:
        return "WARN";
      case LogLevel.Error:
        return "FAIL";
      case LogLevel.Critical:
        return "CRIT";
    }

    return logLevel.ToString().ToUpper();
  }

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    var message = formatter(state, exception);
    var logBuilder = new StringBuilder();
    if (!string.IsNullOrEmpty(message))
    {
      var timeStamp = DateTime.Now;
      logBuilder.Append(timeStamp.ToString("o"));
      logBuilder.Append('\t');
      logBuilder.Append(GetShortLogLevel(logLevel));
      logBuilder.Append("\t[");
      logBuilder.Append(_category);
      logBuilder.Append("]\t[");
      logBuilder.Append(eventId);
      logBuilder.Append("]\t");
      logBuilder.Append(message);
    }

    if (exception != null)
    {
      logBuilder.AppendLine(exception.ToString());
    }

    _queue.Add(logBuilder.ToString());
  }


  public bool IsEnabled(LogLevel logLevel)
  {
    return true;
  }

  public IDisposable? BeginScope<TState>(TState state) where TState : notnull
  {
    return null;
  }
}