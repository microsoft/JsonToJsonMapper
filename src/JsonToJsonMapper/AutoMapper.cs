namespace JsonToJsonMapper;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class AutoMapper
{
  private readonly MappingRule _mapping;

  private readonly JsonSerializerSettings _jsonConvertSettings = new()
  {
    DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
    DateParseHandling = DateParseHandling.None
  };

  private readonly TransformationFactory _handler;

  public AutoMapper(string autoMapperConfig)
  {
    _handler = new TransformationFactory();
    var config = JsonConvert.DeserializeObject<AutoMapperConfig>(autoMapperConfig);
    _mapping = config.MappingRuleConfig;

    if (_mapping == null || !_mapping.TruthTable.Any())
    {
      throw new Exception("Invalid mapping json");
    }

    var scripts = new Dictionary<string, Script>();

    // Compile and Load all the scripts in memory
    if (config.Scripts != null)
    {
      foreach (var script in config.Scripts)
      {
        // With assembly import
        var options = script.Reference != null ? ScriptOptions.Default.AddReferences(script.Reference.Assembly).WithImports(script.Reference.NameSpace) : null;
        scripts.Add(script.Name, CSharpScript.Create<string>(script.Code, options, globalsType: typeof(ScriptHost)));
      }
    }

    // Load all the handlers
    _handler.AddHandlers(new TransposeHandler());
    _handler.AddHandlers(new TypeConverterHandler());
    _handler.AddHandlers(new ValueMappingHandler());
    _handler.AddHandlers(new RoslynScriptHandler(scripts));
    _handler.AddHandlers(new FunctionHandler());
  }

  /// <summary>
  /// Transforms into a type
  /// </summary>
  /// <param name="inputJson"></param>
  /// <returns></returns>
  public object Transform(string inputJson)
  {
    if (_mapping.DestinationType == null)
    {
      throw new Exception("Invalid mapping json");
    }

    return Execute((JObject)JsonConvert.DeserializeObject(inputJson, _jsonConvertSettings), _mapping);
  }

  public object Transform(JObject jObj)
  {
    if (_mapping.DestinationType == null)
    {
      throw new Exception("Invalid mapping json");
    }

    return Execute(jObj, _mapping);
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

            var finalValue = _handler.GetHandler<TypeConverterHandler>()
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
    _mapping.IgnoreNullValue = ignoreNullValue;
    return JsonConvert.SerializeObject(ExecuteToJson((JObject)JsonConvert.DeserializeObject(inputJson, _jsonConvertSettings), _mapping));
  }

  public JObject TransformIntoJson(JObject jObj, bool ignoreNullValue)
  {
    _mapping.IgnoreNullValue = ignoreNullValue;
    return ExecuteToJson(jObj, _mapping);
  }

  /// <summary>
  /// Transforms into json. Uses the IgnoreNullValue from the config, default is true.
  /// </summary>
  /// <param name="inputJson"></param>
  /// <returns></returns>
  public string TransformIntoJson(string inputJson)
  {
    return JsonConvert.SerializeObject(ExecuteToJson((JObject)JsonConvert.DeserializeObject(inputJson, _jsonConvertSettings), _mapping));
  }

  public string TransformIntoJson(JObject jObj)
  {
    return JsonConvert.SerializeObject(ExecuteToJson(jObj, _mapping));
  }

  protected JObject ExecuteToJson(JObject jsonObject, MappingRule mapping)
  {
    var jsonString = new JsonTransform();
    foreach (var rule in mapping.TruthTable)
    {
      // handle transpose
      if (rule.TransformValue != null && rule.TransformValue.Type != null && string.Equals(rule.TransformValue.Type, "promoteArrayToProperty", StringComparison.OrdinalIgnoreCase))
      {
        Dictionary<string, object> transposeResponse = _handler.GetHandler<TransposeHandler>()
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

          var finalValue = _handler.GetHandler<TypeConverterHandler>()
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
              jsonString.Json.Add(rule.DestinationColumn, _handler.GetHandler<TypeConverterHandler>()
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
            var jTok = (JToken)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ExecuteToJson((JObject)a, mapping)), _jsonConvertSettings);
            array.Add(jTok);
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
        array.Add((JToken)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ExecuteToJson((JObject)item, mapping)), _jsonConvertSettings));
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
      return _handler.GetHandler<RoslynScriptHandler>().Run(JObject.FromObject(transform), jsonObject);
    }

    if (transform != null && transform.Type != null && transform.Type.Equals("FUNCTION", StringComparison.OrdinalIgnoreCase))
    {
      return _handler.GetHandler<FunctionHandler>().Run(JObject.FromObject(transform), jsonObject);
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
        var json = jsonObject.Parent;
        for (var i = 2; i < key.Split(new[] { "[{parent}]" }, StringSplitOptions.None).Length; i++)
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
      value = _handler.GetHandler<ValueMappingHandler>()
        .Run(JObject.FromObject(transform), JObject.FromObject(new { value = value }));
    }

    return value;
  }
}
