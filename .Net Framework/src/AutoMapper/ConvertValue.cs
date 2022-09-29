namespace JsonToJsonMapper
{
  public class ConvertValue
  {
    public string ExistingValue { get; set; }
    public string NewValue { get; set; }

    public ConvertValue(string existingValue, string newValue)
    {
      ExistingValue = existingValue;
      NewValue = newValue;
    }
  }
}
