namespace JsonToJsonMapper;

using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class TypeConverterHandler : ITransformationHandler
{
  /// <summary>
  /// Converts value from one type to another
  /// </summary>
  /// <param name="config"></param>
  /// <param name="input"></param>
  /// <returns></returns>
  public dynamic? Run(JObject config, JObject input)
  {
    var value = input["value"].Value<object>();
    var dataType = config["DataType"].Value<string>();
    var format = config["Format"].Value<string>();

    if (value == null || ((JToken)value).Type == JTokenType.Null)
    {
      return value;
    }

    if (string.IsNullOrWhiteSpace(dataType))
    {
      return value;
    }

    switch (dataType.ToUpperInvariant())
    {
      case "LONG":
      {
        return Convert.ToInt64(value);
      }

      case "JOBJECT":
      {
        var jtokenvalue = (JToken)value;
        if (jtokenvalue.Type == JTokenType.Object)
        {
          return (JObject)jtokenvalue;
        }

        return JsonConvert.DeserializeObject(jtokenvalue.ToString());
      }

      case "JARRAY":
      {
        var jtokenvalue = (JToken)value;
        if (jtokenvalue.Type == JTokenType.Array)
        {
          return jtokenvalue.Any() ? (JArray)jtokenvalue : null;
        }

        if (jtokenvalue.Type == JTokenType.Object)
        {
          return new JArray(jtokenvalue);
        }

        if (jtokenvalue.ToString().StartsWith("[") && jtokenvalue.ToString().EndsWith("]"))
        {
          return JArray.Parse(jtokenvalue.ToString());
        }

        return new JArray(jtokenvalue);
      }

      case "SHORT":
      {
        return Convert.ToInt16(value);
      }

      case "INT":
      case "INTEGER":
      {
        return Convert.ToInt32(value);
      }

      case "GUID":
      {
        return new Guid(value.ToString());
      }

      case "DATETIME":
      {
        var valueType = value.ToString();
        var x = new Regex(@".*[+-][0-9][0-9][:]");
        return x.IsMatch(valueType) ? DateTimeOffset.Parse(valueType, CultureInfo.InvariantCulture) : DateTime.Parse(valueType, CultureInfo.InvariantCulture);
      }

      case "CUSTOMDATETIME":
      {
        var valueType = value.ToString();
        var x = new Regex(@".[+-][0-9]{4}");
        if (!x.IsMatch(valueType))
        {
          return null;
        }

        var pos = valueType.IndexOf("Z", StringComparison.OrdinalIgnoreCase);
        var datetime = valueType.Substring(0, pos);
        return datetime;
      }

      case "BOOL":
      case "BOOLEAN":
      {
        return Convert.ToBoolean(value);
      }

      case "DECIMAL":
      {
        return Convert.ToDecimal(value);
      }

      case "DECIMAL?":
      {
        if (decimal.TryParse(value.ToString(), out var decimalValue))
        {
          return decimalValue;
        }

        return null;
      }

      case "INT?":
      {
        var dval = Convert.ToDecimal(value.ToString());
        return decimal.ToInt32(dval);
      }

      case "GUID?":
      {
        if (Guid.TryParse(value.ToString(), out var guid))
        {
          return guid;
        }

        return null;
      }

      case "DATETIME?":
      {
        if (DateTime.TryParse(value.ToString(), out var datetime))
        {
          return datetime;
        }

        return null;
      }

      case "UTCDATETIME":
      {
        if (!DateTime.TryParse(value.ToString(), out var dateTime))
        {
          return null;
        }

        var utcDateTime = dateTime.ToString("s");
        var zone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        utcDateTime = utcDateTime + "+" + zone.BaseUtcOffset.ToString();
        return utcDateTime;
      }

      case "STRINGTOUTCDATEFORMAT":
      {
        //This block converts the given date into UTC formatted string, it will not change the TimeZone offset
        //compared to previous block.
        if (!DateTime.TryParse(value.ToString(), out var dateTime))
        {
          return null;
        }

        var timeInUtcFormat = dateTime.ToString("yyyy-MM-ddTHH:mm:sszzz");
        return timeInUtcFormat;
      }

      case "REMOVEDATETIMEOFFSET":
      {
        var valueType = value.ToString();
        if (!valueType.Contains("Z"))
        {
          return valueType;
        }

        var pos = valueType.IndexOf("Z", StringComparison.OrdinalIgnoreCase);
        var datetime = valueType.Substring(0, pos + 1); //including 'Z' in the output
        return datetime;
      }

      case "FORMATTEDDATETIME":
      {
        if (!DateTime.TryParse(value.ToString(), out var dateTime))
        {
          return null;
        }

        var timeInUtcFormat = !string.IsNullOrWhiteSpace(format) ? dateTime.ToString(format) : dateTime.ToString(null, CultureInfo.InvariantCulture);
        return timeInUtcFormat;
      }

      case "STRING":
      {
        return value.GetType().Equals("JValue") ? ((JValue)value).Value<string>() : value.ToString();
      }
    }

    return value;
  }
}
