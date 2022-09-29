namespace JsonToJsonMapper;

using Newtonsoft.Json.Linq;

public interface ITransformationHandler
{
  dynamic Run(JObject config, JObject input);
}
