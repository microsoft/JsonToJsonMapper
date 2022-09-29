namespace JsonToJsonMapper;

using Newtonsoft.Json.Linq;

public class ValueMappingHandler : ITransformationHandler
{
  /// <summary>
  /// Returns value if matches the rule else returns default value.
  /// </summary>
  /// <param name="transform"></param>
  /// <param name="input"></param>
  /// <returns></returns>
  public dynamic Run(JObject transform, JObject input)
  {
    var valueMapping = transform["ValueMapping"].ToObject<List<ConvertValue>>();
    var defaultValue = transform["DefaultValue"].Value<string>();
    var value = input["value"].Value<string>();

    if (transform != null && valueMapping != null && value != null)
    {
      value = (from item in valueMapping where item.ExistingValue.Equals(value, StringComparison.OrdinalIgnoreCase) select item.NewValue).FirstOrDefault();
    }

    if (transform != null && string.IsNullOrWhiteSpace(value) && defaultValue != null)
    {
      if (defaultValue.Equals("UTCNOW", StringComparison.OrdinalIgnoreCase))
      {
        return DateTime.UtcNow.ToString();
      }

      if (defaultValue.Equals("NEWGUID", StringComparison.OrdinalIgnoreCase))
      {
        return Guid.NewGuid().ToString();
      }

      return defaultValue;
    }

    return value;
  }
}
