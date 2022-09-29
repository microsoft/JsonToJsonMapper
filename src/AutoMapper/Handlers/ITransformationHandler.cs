using Newtonsoft.Json.Linq;

namespace JsonToJsonMapper
{
  public interface ITransformationHandler
  {
    dynamic Run(JObject config, JObject input);
  }
}
