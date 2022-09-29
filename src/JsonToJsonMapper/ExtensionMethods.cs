namespace JsonToJsonMapper;

public static class ExtensionMethods
{
  public static void AddRange(this Dictionary<string, object> dictA, Dictionary<string, object> dictB)
  {
    foreach (var pair in dictB)
    {
      dictA[pair.Key] = pair.Value;
    }
  }
}
