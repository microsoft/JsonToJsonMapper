namespace JsonToJsonMapper;

using System.Text;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json.Linq;

internal class RoslynScriptHandler : ITransformationHandler
{
  public Dictionary<string, Script> Scripts { get; set; }

  public RoslynScriptHandler(Dictionary<string, Script> scripts)
  {
    Scripts = scripts;
  }

  /// <summary>
  /// Executes the script using Roslyn and returns the value.
  /// </summary>
  /// <param name="transform"></param>
  /// <param name="input"></param>
  /// <returns></returns>
  public dynamic Run(JObject transform, JObject input)
  {
    var inputParam = new StringBuilder();
    var parameters = transform["Params"].ToObject<List<string>>();
    var scriptName = transform["ScriptName"].Value<string>();

    foreach (var item in parameters)
    {
      if (item.StartsWith("$"))
      {
        if (input.SelectTokens(item) != null)
        {
          var tokens = input.SelectTokens(item);
          foreach (var i in tokens)
          {
            inputParam.Append(i.ToString());
            if (tokens.Count() > 1)
              inputParam.Append("[tokenDelimiter]");
          }
        }

        if (parameters.Count > 1)
          inputParam.Append(string.Empty + "[delimiter]");
      }
      else
        inputParam.Append(item + "[delimiter]");
    }

    var result = Scripts[scriptName].RunAsync(new ScriptHost { Args = inputParam.ToString() });
    return result.Result.ReturnValue.ToString();
  }
}
