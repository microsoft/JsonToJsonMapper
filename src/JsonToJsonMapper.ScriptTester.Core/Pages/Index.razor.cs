namespace JsonToJsonMapper.ScriptTester.Core.Pages;

public sealed partial class Index
{
  private string _code { get; set; }
  private string _assemblies { get; set; }
  private string _namespaces { get; set; }
  private string _input { get; set; }
  private string _output { get; set; }
  private bool _canRun => !string.IsNullOrWhiteSpace(_code) && !string.IsNullOrWhiteSpace(_input);

  private Task OnRun()
  {
    _output = $"Run:  {_code}{Environment.NewLine}+{Environment.NewLine}{_input}";
    return Task.CompletedTask;
  }
}
