using System.Collections.Generic;

namespace JsonToJsonMapper
{
  public class Transform
  {
    public string Type { get; set; }
    public string PrependKeyText { get; set; }
    public List<ConvertValue> ValueMapping { get; set; }
    public string DefaultValue { get; set; }
    public string ScriptName { get; set; }
    public List<dynamic> Params { get; set; }
    public string KeyLookupField { get; set; }
    public string ValueLookupField { get; set; }
    public string Function { get; set; }
    public string Delimeter { get; set; }
    public string CompareToValue { get; set; }
    public string ReturnValue { get; set; }
    public string IgnoreEmptyParams { get; set; }
    public int Index { get; set; }
  }
}
