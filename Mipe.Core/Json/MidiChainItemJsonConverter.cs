using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Mipe.Core.Chains;

namespace Mipe.Core;

public class MidiChainItemJsonConverter : JsonConverter<IMidiChainItem[]>
{
  public override IMidiChainItem[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (JsonNode.Parse(ref reader) is not JsonArray arr) throw new NotSupportedException();
    var result = new List<IMidiChainItem?>();
    foreach (var obj in arr.OfType<JsonObject>())
    {
      if (!Enum.TryParse<ChainItemType>(obj["Type"]?.GetValue<string>(), true, out var type))
      {
        continue;
      }

      switch (type)
      {
        case ChainItemType.Output:
          result.Add(obj.Deserialize<OutputMidiChainItem>(options));
          break;
        case ChainItemType.NoteToCc:
          result.Add(obj.Deserialize<NoteToControllerMidiChainItem>(options));
          break;
        case ChainItemType.NoteToProgramChange:
          result.Add(obj.Deserialize<NoteToProgramChangeMidiChainItem>(options));
          break;
        case ChainItemType.Filter:
          result.Add(obj.Deserialize<FilterMidiChainItem>(options));
          break;
        case ChainItemType.Velocity:
          result.Add(obj.Deserialize<VelocityChainItem>(options));
          break;
        case ChainItemType.Fork:
          result.Add(obj.Deserialize<ForkChainItem>(options));
          break;
        default:
          continue;
      }
    }

    return result.OfType<IMidiChainItem>().ToArray();
  }

  public override void Write(Utf8JsonWriter writer, IMidiChainItem[] value, JsonSerializerOptions options)
  {
    throw new NotSupportedException();
  }
}