using System.Text.Json;
using System.Text.Json.Serialization;
using Mipe.Core;
using Range = Mipe.Core.Range;

namespace Mipe.Core.Tests;

public class UnitTest1
{
  [Fact]
  public void Test1()
  {
    const string filename = @"C:\Repos\dotNet\Hsp.MidiProxy\default.json";
    var s = MipeInstance.Load(filename);
  }

  [Theory]
  [InlineData("1..10", 1, 10)]
  [InlineData("2..10", 2, 10)]
  [InlineData("2", 2, null)]
  public void ValidRanges(string str, int min, int? max)
  {
    Range.Parse(str, out var r);
    Assert.NotNull(r);
    Assert.Equal(min, r.Minimum);
    Assert.Equal(max, r.Maximum);
  }

  [Theory]
  [InlineData("a..10")]
  [InlineData("2 .. 10")]
  [InlineData("2 ..5")]
  [InlineData("1..")]
  public void FailedRanges(string str)
  {
    Range.Parse(str, out var r);
    Assert.Null(r);
  }

  [Fact]
  public void ReadRange()
  {
    var json = @"
      [
        ""0..200"",
        ""5""
      ]";

    var r = JsonSerializer.Deserialize<Range[]>(json, new JsonSerializerOptions
    {
      Converters =
      {
        new RangeJsonConverter()
      }
    });

    Assert.NotNull(r);
    Assert.Equal(2, r.Length);
    Assert.Equal(0, r[0].Minimum);
    Assert.Equal(200, r[0].Maximum);
    Assert.Equal(5, r[1].Minimum);
    Assert.Null(r[1].Maximum);
  }
}