namespace JsonToJsonMapper;

using Newtonsoft.Json;

public class JsonTransform
{
  [JsonExtensionData]
  public Dictionary<string, object> Json { get; set; }

  public JsonTransform()
  {
    Json = new Dictionary<string, object>();
  }
}
