using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace JsonToJsonMapper.ScriptTester.Core.Pages;

public sealed partial class Index
{
  private string _code { get; set; }
  private string _assemblies { get; set; }
  private string _namespaces { get; set; }
  private string _input { get; set; }
  private string _output { get; set; }
  private bool _canRun => !string.IsNullOrWhiteSpace(_code) && !string.IsNullOrWhiteSpace(_input);

  private async Task OnRun()
  {
    _output = "Running...";

    var options = (!string.IsNullOrWhiteSpace(_assemblies) && !string.IsNullOrWhiteSpace(_namespaces)) ? ScriptOptions.Default.AddReferences(_assemblies).WithImports(_namespaces) : null;
    var script = CSharpScript.Create<string>(_code, options, globalsType: typeof(ScriptHost));
    var host = new ScriptHost() {Args = _input};
    var result = await script.RunAsync(host);
    _output = result.ReturnValue;
  }
}
