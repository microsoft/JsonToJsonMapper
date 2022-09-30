namespace JsonToJsonMapper;

using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

public class FunctionHandler : ITransformationHandler
{
  public dynamic? Run(JObject transform, JObject input)
  {
    var inputParam = new List<string>();
    string nullString = null;
    var parameters = transform["Params"].ToObject<List<dynamic>>();
    var function = transform["Function"].Value<string>();
    var ignoreEmptyValue = transform["IgnoreEmptyParams"] != null ? transform["IgnoreEmptyParams"].Value<string>() : string.Empty;
    if (!string.IsNullOrWhiteSpace(function) && function.Equals("URIESCAPEDATASTRING", StringComparison.OrdinalIgnoreCase) == false)
    {
      if (parameters != null)
      {
        foreach (var item in parameters)
        {
          if (item is not JToken)
          {
            if (item.StartsWith("$"))
            {
              if (!item.ToUpperInvariant().Contains("[{PARENT}]"))
              {
                var tokens = input.SelectTokens((string)item);
                if (tokens != null && tokens.Any())
                {
                  foreach (var i in tokens)
                  {
                    if (i.Type == JTokenType.Null)
                    {
                      inputParam.Add(nullString);
                    }
                    else if (string.IsNullOrWhiteSpace(i.ToString()))
                    {
                      if (Convert.ToBoolean(ignoreEmptyValue))
                      {
                        inputParam.Add(nullString);
                      }
                      else
                      {
                        inputParam.Add(i.ToString());
                      }
                    }
                    else
                    {
                      inputParam.Add(i.ToString());
                    }
                  }
                }
                else
                {
                  inputParam.Add(nullString);
                }
              }
              else
              {
                JContainer json;
                json = input.Parent;
                for (var i = 2; i < item.Split(new string[] { "[{parent}]" }, StringSplitOptions.None).Length; i++)
                {
                  json = json.Parent;
                }

                JToken valueToken = json.SelectToken(item.Replace("[{parent}].", "").Replace("$.", ""));
                if (valueToken != null)
                {
                  if (valueToken.Type == JTokenType.Array || valueToken.Type == JTokenType.Object)
                  {
                    inputParam.Add(valueToken.ToString().Replace("\r", "").Replace("\n", "").Replace("\t", ""));
                  }
                  else if (valueToken.Value<string>() != null)
                  {
                    if (string.IsNullOrWhiteSpace(valueToken.ToString()))
                    {
                      if (Convert.ToBoolean(ignoreEmptyValue))
                      {
                        inputParam.Add(nullString);
                      }
                      else
                      {
                        inputParam.Add(valueToken.ToString());
                      }
                    }
                    else
                    {
                      {
                        inputParam.Add(valueToken.ToString());
                      }
                    }
                  }
                  else
                  {
                    inputParam.Add(nullString);
                  }
                }
                else
                {
                  inputParam.Add(nullString);
                }
              }
            }
            else
            {
              inputParam.Add(item);
            }
          }
        }
      }
    }

    switch (function.ToUpperInvariant())
    {
      case "CONCAT":
      {
        return ConCat(inputParam, transform["Delimeter"].Value<string>());
      }

      case "REPLACEVALUE":
      {
        var compareToValue = transform["CompareToValue"].Value<string>();
        var returnValue = transform["ReturnValue"].Value<string>();
        var defaultValue = transform["DefaultValue"].Value<string>();

        compareToValue = GetCompareValue(input, nullString, compareToValue);
        returnValue = GetTokenValue(input, nullString, returnValue);
        defaultValue = GetTokenValue(input, nullString, defaultValue);
        return ReplaceValue(inputParam, compareToValue, returnValue, defaultValue);
      }

      case "REPLACEVALUEWITHREGEXCOMPARISON":
      {
        var compareToValue = transform["CompareToValue"].Value<string>();
        var returnValue = transform["ReturnValue"].Value<string>();
        var defaultValue = transform["DefaultValue"].Value<string>();
        compareToValue = GetCompareValue(input, nullString, compareToValue);

        returnValue = GetTokenValue(input, nullString, returnValue);
        defaultValue = GetTokenValue(input, nullString, defaultValue);

        return ReplaceValueWithRegexComparison(inputParam, compareToValue, returnValue, defaultValue);
      }

      case "SPLIT":
      {
        var delimeter = transform["Delimeter"].Value<char>();
        var index = transform["Index"].Value<int>();
        var positionToken = transform["Position"];
        var position = positionToken != null ? positionToken.ToString() : string.Empty;
        return Split(inputParam, delimeter, index, position);
      }

      case "TOUPPERCASE":
      {
        return inputParam[0] != null ? inputParam[0].ToUpperInvariant() : string.Empty;
      }

      case "TOLOWERCASE":
      {
        return inputParam[0] != null ? inputParam[0].ToLowerInvariant() : string.Empty;
      }

      case "RANGEMAPPING":
      {
        return MapRange((JArray)transform.SelectToken("$.Params"), (string)input.SelectToken(transform.SelectToken("$.DefaultValue").ToString()));
      }

      case "ONETOONEMAPPING":
      {
        return MapOneToOne((JArray)transform.SelectToken("$.Params"), (string)input.SelectToken(transform.SelectToken("$.DefaultValue").ToString()));
      }

      case "URIESCAPEDATASTRING":
      {
        UriEscapeDataString(input, parameters);
        break;
      }
    }

    return null;
  }

  private static string GetTokenValue(JObject input, string nullString, string value)
  {
    if (value != null && value.StartsWith("$."))
    {
      var returnValueToken = input.SelectToken(value);
      value = returnValueToken == null ? nullString : returnValueToken.Type == JTokenType.Null ? nullString : returnValueToken.ToString();
    }

    return value;
  }

  private static string GetCompareValue(JObject input, string nullString, string compareToValue)
  {
    if (compareToValue != null && compareToValue.StartsWith("$."))
    {
      var compareToValueToken = input.SelectToken(compareToValue);
      compareToValue = compareToValueToken == null ? nullString : compareToValueToken.ToString();
    }

    return compareToValue;
  }

  private static void UriEscapeDataString(JObject input, IList<dynamic> parameters)
  {
    if (parameters == null)
    {
      return;
    }

    foreach (var parameter in parameters)
    {
      if (parameter == null)
      {
        continue;
      }

      string theParameter = parameter.ToString();
      if (!theParameter.StartsWith("$"))
      {
        continue;
      }

      var parameterToken = input.SelectTokens(theParameter);
      if (parameterToken == null || !parameterToken.Any())
      {
        continue;
      }

      foreach (var token in parameterToken)
      {
        if (token.Type == JTokenType.Null)
        {
          continue;
        }

        var data = token.ToString();
        var uriEscapedData = Uri.EscapeDataString(data);
        token.Replace(uriEscapedData);
      }
    }
  }

  private static string? ConCat(List<string> args, string delimiter)
  {
    return string.Join(delimiter, args.Where(value => value != null).ToList());
  }

  private static string? ReplaceValue(List<string> args, string compareToValue, string? returnValue, string? defaultValue)
  {
    if (args[0] == null && compareToValue == null)
    {
      return returnValue;
    }

    if (args[0] != null && args[0].Equals(compareToValue, StringComparison.OrdinalIgnoreCase))
    {
      return returnValue;
    }

    return defaultValue;
  }

  private static string? ReplaceValueWithRegexComparison(List<string> args, string compareToValue, string? returnValue, string? defaultValue)
  {
    if (args[0] == null && compareToValue == null)
    {
      return returnValue;
    }

    if (args[0] != null && compareToValue != null && Regex.IsMatch(args[0], compareToValue, RegexOptions.IgnoreCase))
    {
      return returnValue;
    }

    return defaultValue;
  }

  private static string? Split(List<string> args, char delimeter, int index, string position = "")
  {
    if (string.IsNullOrEmpty(position))
    {
      var delimiters = new char[] { delimeter };
      return args[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries)[index];
    }

    if (position.Equals("FIRST", StringComparison.OrdinalIgnoreCase))
    {
      var delimiters = new char[] { delimeter };
      return args[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
    }

    if (position.Equals("LAST", StringComparison.OrdinalIgnoreCase))
    {
      var delimiters = new char[] { delimeter };
      return args[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
    }

    return string.Empty;
  }

  private static string? MapRange(JArray truthTable, string value)
  {
    return Convert.ToString(truthTable.SelectToken($"$.[?(@.min<'{Convert.ToInt64(value)}' && @.max>'{Convert.ToInt64(value)}')].value"));
  }

  private static string? MapOneToOne(JArray truthTable, string value)
  {
    return Convert.ToString(truthTable.SelectToken($"$.[?(@.key=='{value}')].value"));
  }
}
