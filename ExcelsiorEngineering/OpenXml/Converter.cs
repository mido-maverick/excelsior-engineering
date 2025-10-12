using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ExcelsiorEngineering.OpenXml;

public class Converter
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

    protected void Set(SdtElement sdtElement, string text)
    {
        switch (sdtElement)
        {
            case SdtRun sdtRun:
                Set(sdtRun, text);
                break;
            case SdtCell sdtCell:
                Set(sdtCell, text);
                break;
            default:
                throw new NotSupportedException();
        }
    }

    protected void Set(SdtRun sdtRun, string text)
    {
        var sdtContentRun = sdtRun.SdtContentRun ?? throw new InvalidOperationException();

        var runs = sdtContentRun.Elements<Run>();
        if (!runs.Any()) throw new InvalidOperationException();
        foreach (var run in runs.Skip(1).ToList()) run.Remove();

        var textElements = runs.First().Elements<Text>();
        if (!textElements.Any()) throw new InvalidOperationException();
        foreach (var textElement in textElements.Skip(1).ToList()) textElement.Remove();

        textElements.First().Text = text;
    }

    protected void Set(SdtCell sdtCell, string text)
    {
        var sdtContentCell = sdtCell.SdtContentCell ?? throw new InvalidOperationException();
        var tableCell = sdtContentCell.GetFirstChild<TableCell>() ?? throw new InvalidOperationException();

        var paragraphs = tableCell.Elements<Paragraph>();
        if (!paragraphs.Any()) throw new InvalidOperationException();
        foreach (var paragraph in paragraphs.Skip(1).ToList()) paragraph.Remove();

        var runs = paragraphs.First().Elements<Run>();
        if (!runs.Any()) throw new InvalidOperationException();
        foreach (var run in runs.Skip(1).ToList()) run.Remove();

        var textElements = runs.First().Elements<Text>();
        if (!textElements.Any()) throw new InvalidOperationException();
        foreach (var textElement in textElements.Skip(1).ToList()) textElement.Remove();

        textElements.First().Text = text;
    }

    protected void Set(SdtElement sdtElement, object? obj, string? format = null)
    {
        switch (obj)
        {
            case bool b:
                throw new NotImplementedException();
            case int i:
                Set(sdtElement, i.ToString(format ?? "G"));
                break;
            case double d:
                Set(sdtElement, d.ToString(format ?? "0.0##"));
                break;
            case Enum e:
                Set(sdtElement, e.ToString(format ?? "G"));
                break;
            case string s:
                Set(sdtElement, s);
                break;
            case null:
                throw new NotImplementedException();
            case object when obj.GetType().IsClass:
                Populate(sdtElement, obj);
                break;
            default:
                throw new NotSupportedException();
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
