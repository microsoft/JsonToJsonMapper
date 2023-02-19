namespace JsonToJsonMapper;

internal class TransformationFactory
{
  private readonly Dictionary<string, ITransformationHandler> _registeredHandlers = new();

  /// <summary>
  /// Add handler to the dictionary
  /// </summary>
  /// <param name="handler"></param>
  public void AddHandlers(ITransformationHandler handler)
  {
    _registeredHandlers[handler.GetType().Name] = handler;
  }

  /// <summary>
  /// Returns a handler based on the type
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public ITransformationHandler GetHandler<T>()
  {
    return !_registeredHandlers.ContainsKey(typeof(T).Name) ? null : _registeredHandlers[typeof(T).Name];
  }
}
