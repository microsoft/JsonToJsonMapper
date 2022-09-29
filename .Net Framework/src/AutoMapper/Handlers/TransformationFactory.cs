using System.Collections.Generic;

namespace JsonToJsonMapper
{
  internal class TransformationFactory
  {
    private readonly Dictionary<string, ITransformationHandler> registeredHandlers = new Dictionary<string, ITransformationHandler>();

    /// <summary>
    /// Add handler to the dictionary
    /// </summary>
    /// <param name="handler"></param>
    public void AddHandlers(ITransformationHandler handler)
    {
      registeredHandlers[handler.GetType().Name] = handler;
    }

    /// <summary>
    /// Returns a handler based on the type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ITransformationHandler GetHandler<T>()
    {
      if (!registeredHandlers.ContainsKey(typeof(T).Name))
        return null;

      return registeredHandlers[typeof(T).Name];
    }
  }
}
