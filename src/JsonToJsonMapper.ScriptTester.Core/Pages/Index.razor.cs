namespace JsonToJsonMapper.ScriptTester.Core.Pages;

using BlazorMonaco;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

public sealed partial class Index
{
  private MonacoEditor _code { get; set; }
  private string _assemblies { get; set; }
  private string _namespaces { get; set; }
  private string _input { get; set; }
  private string _output { get; set; }

  private StandaloneEditorConstructionOptions EditorOptions(MonacoEditor editor)
  {
    return new StandaloneEditorConstructionOptions
    {
      Language = "csharp",
      AutomaticLayout = true,
      RenderIndentGuides = false,
      RenderFinalNewline = false,
      ColorDecorators = true,
      OccurrencesHighlight = true,
    };
  }

  private async Task OnRun()
  {
    _output = "Running...";
    try
    {
      var options = (!string.IsNullOrWhiteSpace(_assemblies) && !string.IsNullOrWhiteSpace(_namespaces)) ? ScriptOptions.Default.AddReferences(_assemblies).WithImports(_namespaces) : null;
      var src = await _code.GetValue();
      var script = CSharpScript.Create<string>(src, options, globalsType: typeof(ScriptHost));
      var host = new ScriptHost() {Args = _input};
      var result = await script.RunAsync(host);
      _output = result.ReturnValue;
    }
    catch (Exception ex)
    {
      _output = ex.Message;
    }
  }
}
