using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Mipe.Core;

public class MidiClock
{
  private readonly ILogger _logger;
  private const byte TicksPerQuarterNote = 24;


  private byte _remainingTicks = TicksPerQuarterNote;
  private int _counter;
  private long _lastTick;

  public double Bpm { get; private set; }

  public int Counter
  {
    get => _counter;
    private set
    {
      var newTick = Stopwatch.GetTimestamp();
      var diff = newTick - _lastTick;
      Bpm = 30 / (diff / (double)Stopwatch.Frequency);
      _lastTick = newTick;
      _counter = value;
    }
  }


  public event EventHandler? OnQuarterNote;


  public MidiClock(ILogger<MidiClock> logger)
  {
    _logger = logger;
  }

  public void Start()
  {
    _remainingTicks = TicksPerQuarterNote;
    Counter = 0;
    Bpm = 0;
    Task.Run(() => { _logger.LogInformation("start"); });
  }

  public void Stop()
  {
    _remainingTicks = TicksPerQuarterNote;
    Counter = 0;
    Bpm = 0;
    Task.Run(() => { _logger.LogInformation("stop"); });
  }

  public void Tick()
  {
    _remainingTicks -= 1;
    if (_remainingTicks > 0) return;

    _remainingTicks = TicksPerQuarterNote;
    Counter++;
    Task.Run(() =>
    {
      _logger.LogInformation("tick: {ctr} @ {bpm}bpm", Counter, Bpm);
      OnQuarterNote?.Invoke(this, EventArgs.Empty);
    });
  }
}