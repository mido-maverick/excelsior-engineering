namespace EngineeringFort;

public class ConversionHelper(IServiceProvider serviceProvider)
{
    //private readonly Dictionary<Type, Action<object, Stream>> _mappings = [];
    private readonly Dictionary<Type, MethodInfo> _mappings = [];

    public void Register(Type modelType, MethodInfo convertMethodInfo) => _mappings[modelType] = convertMethodInfo;

    public bool CanConvert(Type modelType) => _mappings.ContainsKey(modelType);

    public async Task ConvertAsync(object model, Stream outputStream)
    {
        var modelType = model.GetType();
        if (!_mappings.TryGetValue(modelType, out var convertMethod))
            throw new InvalidOperationException($"Conversion for {modelType} type is not registered.");

        var conversionServiceType = convertMethod.DeclaringType ??
            throw new InvalidOperationException("Declaring type of the registered convert method is not found.");

        var conversionService = serviceProvider.GetService(conversionServiceType) ??
            throw new InvalidOperationException("Conversion service not found.");

        var result = convertMethod.Invoke(conversionService, [model, outputStream]);
        if (result is Task task) await task;
    }
}
