namespace JsonToJsonMapper;

using Microsoft.CodeAnalysis.Scripting;

public class ScriptDefinitions
{
  public string Name { get; set; }
  public string Code { get; set; }
  public Script ScriptIL { get; set; }
  public Reference Reference { get; set; }
}
