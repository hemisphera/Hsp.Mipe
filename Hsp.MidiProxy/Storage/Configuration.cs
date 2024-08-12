using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Hsp.MidiProxy.Storage;

public class Configuration
{
  private static readonly string ConfigurationFilePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "Hsp.MidiProxy.json");

  public static Configuration Instance { get; } = new();


  public List<StorageMidiProxyPipe> Items { get; set; } = [];

  public bool EnableLogging { get; set; }


  private Configuration()
  {
  }


  public void Load()
  {
    if (!File.Exists(ConfigurationFilePath)) return;
    var json = File.ReadAllText(ConfigurationFilePath);
    JsonConvert.PopulateObject(json, this);
  }

  public void Save()
  {
    File.WriteAllText(ConfigurationFilePath, JsonConvert.SerializeObject(this));
  }
}