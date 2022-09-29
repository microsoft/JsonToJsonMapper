using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JsonToJsonMapper
{
  public class TransposeHandler : ITransformationHandler
  {
    /// <summary>
    /// Transposes an array into properties
    /// </summary>
    /// <param name="rule"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public dynamic Run(JObject rule, JObject input)
    {
      var sourceColumn = rule["SourceColumn"].Value<string>();
      var transformValueKey = rule["TransformValue"]["KeyLookupField"].Value<string>();
      var transformValuePrependText = rule["TransformValue"]["PrependKeyText"].Value<string>();
      var transformValue = rule["TransformValue"]["ValueLookupField"].Value<string>();

      var tokens = input.SelectTokens(sourceColumn);
      var array = new Dictionary<string, object>();
      foreach (var i in tokens)
      {
        var key = string.Empty;
        var token = i.SelectToken(transformValueKey);
        if (token != null)
          key = transformValuePrependText + token.ToString().Replace(" ", string.Empty);

        if (!string.IsNullOrWhiteSpace(key))
        {
          var valueToken = i.SelectToken(transformValue);
          if (valueToken != null)
          {
            if (valueToken.Type == JTokenType.Array || valueToken.Type == JTokenType.Object)
              array.Add(key, valueToken.ToString().Replace("\r", "").Replace("\n", "").Replace("\t", ""));
            else if (valueToken.Value<string>() != null)
              array.Add(key, valueToken);
          }
        }
      }

      return array;
    }
  }
}
