using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ExcelsiorEngineering.OpenXml;

internal class Converter
{
    protected List<TElement> GenerateElements<TElement>(TElement template, IEnumerable<object> objects)
        where TElement : OpenXmlCompositeElement
    {
        var elements = objects.Select(_ => (TElement)template.Clone()).ToList();
        var previousElement = template;
        foreach (var (element, obj) in elements.Zip(objects))
        {
            previousElement.InsertAfterSelf(element);
            if (element is SdtElement sdtElement) Set(sdtElement, obj);
            previousElement = element;
        }
        template.Remove();
        return elements;
    }

    protected void Set(SdtElement sdtElement, object? obj, string? format = null)
    {
        switch (obj)
        {
            case bool b:
                break;
            case int i:
                break;
            case double d:
                break;
            case string s:
                break;
            case null:
                break;
            case object when obj.GetType().IsClass:
                Populate(sdtElement, obj);
                break;
            default:
                break;
        }
    }

    protected void Populate(SdtElement container, object obj)
    {
        if (container is not (SdtBlock or SdtRow)) throw new NotSupportedException();
        var sdtElements = container.Descendants<SdtElement>();
        var properties = obj.GetType().GetProperties();
        foreach (var sdtElement in sdtElements)
        {
            var tagValue = sdtElement.SdtProperties?.GetFirstChild<Tag>()?.Val?.Value;
            if (tagValue is null) continue;

            var property = properties.FirstOrDefault(p => p.Name == tagValue);
            if (property is null) continue;

            var propertyValue = property.GetValue(obj);
            Set(sdtElement, propertyValue);
        }
    }
}
