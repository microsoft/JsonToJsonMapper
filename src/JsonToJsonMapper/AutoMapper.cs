namespace JsonToJsonMapper;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class AutoMapper : IDisposable
{
  public MappingRule Mapping { get; set; }
  private JsonSerializerSettings JsonConvertSettings { get; set; }
  private readonly TransformationFactory handler;

  public AutoMapper(string autoMapperConfig)
  {
    JsonConvertSettings = new JsonSerializerSettings();
    JsonConvertSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
    JsonConvertSettings.DateParseHandling = DateParseHandling.None;
    handler = new TransformationFactory();
    var config = JsonConvert.DeserializeObject<AutoMapperConfig>(autoMapperConfig);
    Mapping = config.MappingRuleConfig;

    if (Mapping == null || !Mapping.TruthTable.Any())
    {
      throw new Exception("Invalid mapping json");
    }

    var scripts = new Dictionary<string, Script>();

    // Compile and Load all the scripts in memory
    if (config.Scripts != null)
    {
      foreach (var script in config.Scripts)
      {
        if (script.Reference != null)
        {
          // With assembly import
          var options = ScriptOptions.Default.AddReferences(script.Reference.Assembly).WithImports(script.Reference.NameSpace);
          scripts.Add(script.Name, CSharpScript.Create<string>(script.Code, options, globalsType: typeof(ScriptHost)));
        }
        else
          scripts.Add(script.Name, CSharpScript.Create<string>(script.Code, globalsType: typeof(ScriptHost)));
      }
    }

    // Load all the handlers
    handler.AddHandlers(new TransposeHandler());
    handler.AddHandlers(new TypeConverterHandler());
    handler.AddHandlers(new ValueMappingHandler());
    handler.AddHandlers(new RoslynScriptHandler(scripts));
    handler.AddHandlers(new FunctionHandler());
  }

  /// <summary>
  /// Transforms into a type
  /// </summary>
  /// <param name="inputJson"></param>
  /// <returns></returns>
  public object Transform(string inputJson)
  {
    if (Mapping.DestinationType == null)
    {
      throw new Exception("Invalid mapping json");
    }

    return Execute((JObject)JsonConvert.DeserializeObject(inputJson, JsonConvertSettings), Mapping);
  }

  public object Transform(JObject jObj)
  {
    if (Mapping.DestinationType == null)
    {
      throw new Exception("Invalid mapping json");
    }

    return Execute(jObj, Mapping);
  }

  /// <summary>
  /// Creates an intance of destination type and sets the properties
  /// </summary>
  /// <param name="jsonObject"></param>
  /// <param name="mapping"></param>
  /// <returns></returns>
  private object Execute(JObject jsonObject, MappingRule mapping)
  {
    var type = Type.GetType(mapping.DestinationType, true);
    var entity = Activator.CreateInstance(type);

    // Set the value for each item in destinationType
    foreach (var rule in mapping.TruthTable)
    {
      var propertyInfo = entity.GetType().GetProperty(rule.DestinationColumn);
      if (propertyInfo != null)
      {
        if (rule.ComplexType == null)
        {
          string valueType;
          var value = GetValue(jsonObject, rule.SourceColumn, rule.TransformValue, out valueType);

          if (value != null)
          {
            if (rule.DataType == null)
            {
              rule.DataType = valueType;
            }

            var finalValue = handler.GetHandler<TypeConverterHandler>()
              .Run(JObject.FromObject(rule), JObject.FromObject(new { value = value }));
            propertyInfo.SetValue(entity, finalValue, null);
          }
          else
          {
            propertyInfo.SetValue(entity, value, null);
          }
        }
        else
        {
          propertyInfo.SetValue(entity, Execute(jsonObject, rule.ComplexType), null);
        }
      }
    }

    return entity;
  }

  /// <summary>
  /// Transforms into Json 
  /// </summary>
  /// <param name="inputJson"></param>
  /// <param name="ignoreNullVaue"></param>
  /// <returns></returns>
  public string TransformIntoJson(string inputJson, bool ignoreNullValue)
  {
    Mapping.IgnoreNullValue = ignoreNullValue;
    return JsonConvert.SerializeObject(ExecuteToJson((JObject)JsonConvert.DeserializeObject(inputJson, JsonConvertSettings), Mapping));
  }

  public JObject TransformIntoJson(JObject jObj, bool ignoreNullValue)
  {
    Mapping.IgnoreNullValue = ignoreNullValue;
    return ExecuteToJson(jObj, Mapping);
  }

  /// <summary>
  /// Transforms into json. Uses the IgnoreNullValue from the config, default is true.
  /// </summary>
  /// <param name="inputJson"></param>
  /// <returns></returns>
  public string TransformIntoJson(string inputJson)
  {
    return JsonConvert.SerializeObject(ExecuteToJson((JObject)JsonConvert.DeserializeObject(inputJson, JsonConvertSettings), Mapping));
  }

  public string TransformIntoJson(JObject jObj)
  {
    return JsonConvert.SerializeObject(ExecuteToJson(jObj, Mapping));
  }

  protected JObject ExecuteToJson(JObject jsonObject, MappingRule mapping)
  {
    var jsonString = new JsonTransform();
    foreach (var rule in mapping.TruthTable)
    {
      // handle transpose
      if (rule.TransformValue != null && rule.TransformValue.Type != null && string.Equals(rule.TransformValue.Type, "promoteArrayToProperty", StringComparison.OrdinalIgnoreCase))
      {
        Dictionary<string, object> transposeResponse = handler.GetHandler<TransposeHandler>()
          .Run(JObject.FromObject(rule), jsonObject);
        if (transposeResponse != null)
        {
          jsonString.Json.AddRange(transposeResponse);
        }
      }
      else if (!string.IsNullOrEmpty(rule.DestinationColumn))
      {
        if (rule.ComplexType == null)
        {
          // Handle Jvalue
          string valueType;
          var destinationValue = rule.DestinationColumn.StartsWith("$") ? jsonObject.SelectToken(rule.DestinationColumn).ToString() : rule.DestinationColumn;

          var value = GetValue(jsonObject, rule.SourceColumn, rule.TransformValue, out valueType);
          if (rule.DataType == null)
          {
            rule.DataType = valueType;
          }

          var finalValue = handler.GetHandler<TypeConverterHandler>()
            .Run(JObject.FromObject(rule), JObject.FromObject(new { value = value }));
          if (finalValue != null || finalValue.Type != JTokenType.Null || (finalValue == null && !mapping.IgnoreNullValue) || (finalValue.Type == JTokenType.Null && !mapping.IgnoreNullValue))
          {
            jsonString.Json.Add(destinationValue, finalValue);
          }
        }
        else if (rule.ComplexType.DataType != null && rule.ComplexType.DataType.ToUpperInvariant().Equals("JARRAY"))
        {
          var result = TransformJArray(jsonObject, rule.ComplexType, mapping.IgnoreNullValue);
          if (result != null)
          {
            jsonString.Json.Add(rule.DestinationColumn, result);
          }
        }
        else
        {
          // Recursive call to handle complex type
          var result = ExecuteToJson(jsonObject, rule.ComplexType);
          if (result != null)
          {
            if (!string.IsNullOrWhiteSpace(rule.DataType))
            {
              jsonString.Json.Add(rule.DestinationColumn, handler.GetHandler<TypeConverterHandler>()
                .Run(JObject.FromObject(rule), JObject.FromObject(new { value = result })));
            }
            else
            {
              jsonString.Json.Add(rule.DestinationColumn, result);
            }
          }
        }
      }
    }

    return JObject.FromObject(jsonString);
  }

  private dynamic TransformJArray(JObject jsonObject, MappingRule mapping, bool ignoreNullVaue)
  {
    var tokens = jsonObject.SelectTokens(mapping.Node);
    var array = new JArray();
    var hasToken = false;
    foreach (var item in tokens)
    {
      hasToken = true;
      if (string.Equals(item.GetType().Name, "jarray", StringComparison.OrdinalIgnoreCase))
      {
        var itemJArray = (JArray)item;
        if (itemJArray.Any())
        {
          foreach (var a in itemJArray)
          {
            var o = (JToken)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ExecuteToJson((JObject)a, mapping)), JsonConvertSettings);
            array.Add(o);
          }
        }
        else
        {
          itemJArray.Add(new JObject(new JProperty("temp", "")));
          array.Add((JToken)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ExecuteToJson((JObject)item.FirstOrDefault(), mapping))));
        }
      }
      else
      {
        array.Add((JToken)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ExecuteToJson((JObject)item, mapping)), JsonConvertSettings));
      }
    }

    if (!hasToken && mapping.IgnoreEmptyArray)
    {
      return null;
    }

    return array;
  }

  private string GetValue(JObject jsonObject, string key, Transform transform, out string valueType)
  {
    string value = null;
    valueType = "string";

    if (transform != null && transform.Type != null && transform.Type.Equals("SCRIPT", StringComparison.OrdinalIgnoreCase))
    {
      return handler.GetHandler<RoslynScriptHandler>().Run(JObject.FromObject(transform), jsonObject);
    }

    if (transform != null && transform.Type != null && transform.Type.Equals("FUNCTION", StringComparison.OrdinalIgnoreCase))
    {
      return handler.GetHandler<FunctionHandler>().Run(JObject.FromObject(transform), jsonObject);
    }

    if (!string.IsNullOrEmpty(key) && key.StartsWith("$"))
    {
      if (!key.ToUpperInvariant().Contains("[{PARENT}]"))
      {
        var token = jsonObject.SelectToken(key);
        if (token != null && token.Value<dynamic>() != null)
        {
          valueType = token.Type.ToString();
          var tokenValue = token.ToString();
          if (token.GetType().Name.Equals("JVALUE", StringComparison.OrdinalIgnoreCase) && token.Type == JTokenType.Null)
          {
            value = null;
          }
          else if (valueType.Equals("Date", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(tokenValue))
          {
            var val = (token.Parent.ToString().Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries).Length > 1) ? token.Parent.ToString().Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries)[1] : tokenValue;
            value = val.Replace("\"", "").Trim();
          }
          else if (valueType.Equals("Boolean", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(tokenValue))
          {
            value = tokenValue.ToLowerInvariant();
          }
          else
          {
            value = tokenValue;
          }
        }
        else
        {
          value = null;
        }
      }
      else
      {
        JContainer json;
        json = jsonObject.Parent;
        for (var i = 2; i < key.Split(new string[] { "[{parent}]" }, StringSplitOptions.None).Length; i++)
        {
          json = json.Parent;
        }

        var valueToken = json.SelectToken(key.Replace("[{parent}].", "").Replace("$.", ""));
        if (valueToken != null)
        {
          valueType = valueToken.Type.ToString();
          if (valueToken.Type == JTokenType.Array || valueToken.Type == JTokenType.Object)
          {
            value = valueToken.ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "");
          }
          else if (valueToken.Value<string>() != null)
          {
            value = valueToken.ToString();
          }
          else
          {
            value = null;
          }
        }
        else
        {
          value = null;
        }
      }
    }
    else
    {
      var jsonobjectvalue = jsonObject.GetValue(key, StringComparison.OrdinalIgnoreCase);
      if (jsonobjectvalue == null || jsonobjectvalue.Type == JTokenType.Null)
      {
        value = null;
      }
      else
      {
        valueType = jsonobjectvalue.Type.ToString();
        value = jsonobjectvalue.ToString();
        if (value.StartsWith("\""))
        {
          value = value.Substring(1);
        }

        if (value.EndsWith("\""))
        {
          value = value.Substring(0, value.Length - 1);
        }
      }
    }

    if (transform != null)
    {
      value = handler.GetHandler<ValueMappingHandler>()
        .Run(JObject.FromObject(transform), JObject.FromObject(new { value = value }));
    }

    return value;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  // Protected implementation of Dispose pattern. 
  protected virtual void Dispose(bool disposing)
  {
    Mapping = null;
  }
}
