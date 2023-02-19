namespace JsonToJsonMapper;

public class MappingRule
{
  public string DestinationType { get; set; }
  public string DataType { get; set; }
  public string Node { get; set; }
  public bool IgnoreEmptyArray { get; set; }

  public bool IgnoreNullValue { get; set; } = true;
  public List<Rule> TruthTable { get; set; }

  public MappingRule()
  {
    TruthTable = new List<Rule>();
  }
}
