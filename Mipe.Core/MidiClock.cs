using System.Diagnostics;
using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;
using Mipe.Core.Inputs;

namespace Mipe.Core;

public sealed class MidiClock : IDisposable
{
  private readonly InputMidiDevice _input;
  private readonly ILogger _logger;
  private const byte TicksPerQuarterNote = 24;

  private byte _remainingTicks = TicksPerQuarterNote;
  private readonly Stopwatch _sw = new();

  public double Bpm
  {
    get
    {
      if (Counter == 0) return 0;
      var secPerBeat = _sw.ElapsedMilliseconds / 1000.0 / Counter;
      return 30.0 / secPerBeat;
    }
  }

  public int Counter { get; private set; }


  public event EventHandler? OnQuarterNote;


  public MidiClock(string input, ILogger<MidiClock> logger)
    : this(InputMidiDevicePool.Instance.Open(input), logger)
  {
  }

  public MidiClock(InputMidiDevice input, ILogger<MidiClock> logger)
  {
    _input = input;
    _logger = logger;
    _input.MessageReceived += OnMessageReceived;
  }


  private void OnMessageReceived(object? sender, IMidiMessage e)
  {
    if (e is SysRealtimeMessage { SysRealtimeType: SysRealtimeType.Start })
      Start();
    if (e is SysRealtimeMessage { SysRealtimeType: SysRealtimeType.Stop })
      Stop();
    if (e is SysRealtimeMessage { SysRealtimeType: SysRealtimeType.Clock })
      Tick();
  }

  public void Start()
  {
    _remainingTicks = TicksPerQuarterNote;
    _sw.Reset();
    _sw.Start();
    Counter = 0;
    Task.Run(() => { _logger.LogInformation("start"); });
  }

  public void Stop()
  {
    _remainingTicks = TicksPerQuarterNote;
    _sw.Stop();
    Counter = 0;
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

  public void Dispose()
  {
    _input.MessageReceived -= OnMessageReceived;
    InputMidiDevicePool.Instance.Close(_input);
  }
}