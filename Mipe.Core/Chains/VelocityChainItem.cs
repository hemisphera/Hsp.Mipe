using Hsp.Midi;
using Hsp.Midi.Messages;
using Microsoft.Extensions.Logging;

namespace Mipe.Core.Chains;

public class VelocityChainItem : IMidiChainItem
{
  private ILogger? _logger;

  /// <summary>
  /// Specifies the output velocity range.
  /// </summary>
  public Range? Range { get; set; }

  /// <summary>
  /// Specifies whether to apply the range to NoteOn messages.
  /// </summary>
  public bool NoteOn { get; set; } = true;

  /// <summary>
  /// Specifies whether to apply the range to NoteOff messages.
  /// </summary>
  public bool NoteOff { get; set; } = false;

  /// <summary>
  /// Specifies the method how the range is applied.
  /// </summary>
  public VelocityApplicationMethod Method { get; set; } = VelocityApplicationMethod.Limit;


  public async Task<IMidiMessage[]> ProcessAsync(IMidiMessage message)
  {
    if (message is not ChannelMessage cm) return [message];
    if (cm.Command != ChannelCommand.NoteOn && cm.Command != ChannelCommand.NoteOff) return [message];
    var cm2 = new ChannelMessage(cm.Command, cm.Channel, cm.Data1, GetNewValue(cm.Data2) ?? cm.Data2);
    _logger?.LogDebug("Modified velocity from {v1} to {v2}", cm.Data2, cm2.Data2);
    return await Task.FromResult<IMidiMessage[]>([cm2]);
  }

  private int? GetNewValue(int inputVelocity)
  {
    return Method switch
    {
      VelocityApplicationMethod.Limit => Range?.Limit(inputVelocity),
      VelocityApplicationMethod.Translate => TranslateToRange(inputVelocity),
      _ => null
    };
  }

  private int? TranslateToRange(double inputVelocity)
  {
    if (Range == null) return null;
    var inputPercentage = inputVelocity / 127.0;
    var outputRange = Range.Maximum ?? 127 - Range.Minimum;
    return (int)(Range.Minimum + outputRange * inputPercentage);
  }

  public Task Initialize(ILogger? logger = null)
  {
    _logger = logger;
    return Task.CompletedTask;
  }

  public Task Deinitialize()
  {
    return Task.CompletedTask;
  }
}