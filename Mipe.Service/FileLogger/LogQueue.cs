namespace Mipe.Core.FileLogger;

public sealed class LogQueue : IDisposable
{
  private readonly Queue<string> _queue = new();
  private readonly string _filename;
  private readonly CancellationTokenSource _cts;

  public LogQueue(string filename)
  {
    _filename = filename;
    _cts = new CancellationTokenSource();
    Task.Run(async () =>
    {
      var token = _cts.Token;
      while (!token.IsCancellationRequested)
      {
        await Task.Delay(TimeSpan.FromSeconds(5), token);
        Flush();
      }
    });
  }

  public void Add(string item)
  {
    lock (_queue)
    {
      _queue.Enqueue(item);
    }
  }

  public void Flush()
  {
    var lines = new List<string>();
    lock (_queue)
    {
      while (_queue.TryDequeue(out var line))
      {
        lines.Add(line);
      }
    }
    
    if (lines.Count == 0) return;

    var fi = new FileInfo(_filename);
    File.AppendAllLines(_filename, lines);
  }

  public void Dispose()
  {
    _cts.Cancel();
    Flush();
    _cts.Dispose();
  }
}