using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonToJsonMapper
{
  public class JsonTransform
  {
    [JsonExtensionData]
    public Dictionary<string, object> Json { get; set; }

    public JsonTransform()
    {
      Json = new Dictionary<string, object>();
    }
  }
}
