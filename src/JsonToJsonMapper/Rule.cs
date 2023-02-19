namespace JsonToJsonMapper;

public class Rule
{
  public string SourceColumn { get; set; }
  public string DestinationColumn { get; set; }
  public string DataType { get; set; }
  public string Format { get; set; }
  public MappingRule ComplexType { get; set; }
  public Transform TransformValue { get; set; }

  public Rule(string sourceColumn, string destinationColumn, string dataType)
  {
    SourceColumn = sourceColumn;
    DestinationColumn = destinationColumn;
    DataType = dataType;
  }

  public Rule(string sourceColumn, string destinationColumn, string dataType, string format)
  {
    SourceColumn = sourceColumn;
    DestinationColumn = destinationColumn;
    DataType = dataType;
    Format = format;
  }

  public Rule(string sourceColumn, string destinationColumn, string dataType, MappingRule mappingRule)
  {
    SourceColumn = sourceColumn;
    DestinationColumn = destinationColumn;
    DataType = dataType;
    ComplexType = mappingRule;
  }

  public Rule(string sourceColumn, string destinationColumn, string dataType, MappingRule mappingRule, Transform transform)
  {
    SourceColumn = sourceColumn;
    DestinationColumn = destinationColumn;
    DataType = dataType;
    ComplexType = mappingRule;
    TransformValue = transform;
  }

  public Rule(string sourceColumn, string destinationColumn, string dataType, Transform transform)
  {
    SourceColumn = sourceColumn;
    DestinationColumn = destinationColumn;
    DataType = dataType;
    TransformValue = transform;
  }

  public Rule(string sourceColumn, string destinationColumn, string dataType, string format, Transform transform)
  {
    SourceColumn = sourceColumn;
    DestinationColumn = destinationColumn;
    DataType = dataType;
    Format = format;
    TransformValue = transform;
  }

  public Rule()
  {
    SourceColumn = string.Empty;
    DestinationColumn = string.Empty;
    DataType = string.Empty;
  }
}
