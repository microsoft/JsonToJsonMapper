using Microsoft.CodeAnalysis.Scripting;

namespace JsonToJsonMapper
{
  public class ScriptDefinitions
  {
    public string Name { get; set; }
    public string Code { get; set; }
    public Script ScriptIL { get; set; }
    public Reference Reference { get; set; }
  }
}
