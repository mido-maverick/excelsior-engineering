using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SS = DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using WP = DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.FileProviders;

namespace ExcelsiorEngineering.OpenXml;

public class Converter
{
    #region Fields
    private readonly IFileProvider? _fileProvider;
    #endregion

    #region Constructors
    public Converter() { }
    public Converter(IFileProvider fileProvider) { _fileProvider = fileProvider; }
    #endregion

    #region Properties
    private SpreadsheetDocument? SpreadsheetDocument { get; set; }
    private WorkbookPart? WorkbookPart => SpreadsheetDocument?.WorkbookPart;
    private Workbook? Workbook => WorkbookPart?.Workbook;
    private Sheets? Sheets => Workbook?.Sheets;
    private SharedStringTablePart? SharedStringTablePart => WorkbookPart?.SharedStringTablePart;
    private SharedStringTable? SharedStringTable => SharedStringTablePart?.SharedStringTable;
    private WorkbookStylesPart? WorkbookStylesPart => WorkbookPart?.WorkbookStylesPart;
    private Stylesheet? Stylesheet => WorkbookStylesPart?.Stylesheet;
    private NumberingFormats? NumberingFormats => Stylesheet?.NumberingFormats;
    private CellFormats? CellFormats => Stylesheet?.CellFormats;
    private IEnumerable<WorksheetPart>? WorksheetParts => WorkbookPart?.WorksheetParts;

    protected WordprocessingDocument? WordprocessingDocument { get; set; }
    protected MainDocumentPart? MainDocumentPart => WordprocessingDocument?.MainDocumentPart;
    protected Document? Document => MainDocumentPart?.Document;
    protected Body? Body => Document?.Body;
    protected IEnumerable<SdtBlock>? SdtBlocks => Body?.Elements<SdtBlock>();
    #endregion

    #region Methods
    public virtual void GenerateDocument(Stream stream, object? dataModel = null)
    {
        WordprocessingDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);
        WordprocessingDocument.AddMainDocumentPart();
        MainDocumentPart!.Document = new Document(new Body(new Paragraph(new WP.Run(new WP.Text("Hello!")))));
        WordprocessingDocument.Dispose();
        WordprocessingDocument = null;
    }

    public void GenerateDocument(string templatePath, Stream stream, IEnumerable<(string tag, IEnumerable<object> dataModels)> sections)
    {
        if (OpenTemplate(templatePath) is not WordprocessingDocument template) throw new InvalidOperationException();
        WordprocessingDocument = template.Clone(stream, isEditable: true);
        if (Body is null) throw new InvalidOperationException();
        if (SdtBlocks is null) throw new InvalidOperationException();
        foreach (var section in sections)
        {
            var templateBlock = SdtBlocks.FirstOrDefault(sdt => sdt.SdtProperties?.GetFirstChild<Tag>()?.Val == section.tag);
            if (templateBlock is null) continue;
            var generatedBlocks = GenerateElements(templateBlock, section.dataModels);
        }
        Body.Elements<OpenXmlElement>()
            .Where(e =>
                e is not SdtBlock ||
                e is SdtBlock sdt &&
                !sections.Select(s => s.tag).Contains(sdt.SdtProperties?.GetFirstChild<Tag>()?.Val?.Value))
            .ToList()
            .ForEach(e => e.Remove());
        WordprocessingDocument?.Dispose();
        WordprocessingDocument = null;
    }

    protected OpenXmlPackage OpenTemplate(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".docx" or ".dotx" => WordprocessingDocument.Open(path, isEditable: false),
            ".xlsx" or ".xltx" => SpreadsheetDocument.Open(path, isEditable: false),
            _ => throw new NotSupportedException(),
        };
    }

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

        var runs = sdtContentRun.Elements<WP.Run>();
        if (!runs.Any()) throw new InvalidOperationException();
        foreach (var run in runs.Skip(1).ToList()) run.Remove();

        var textElements = runs.First().Elements<WP.Text>();
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

        var runs = paragraphs.First().Elements<WP.Run>();
        if (!runs.Any()) throw new InvalidOperationException();
        foreach (var run in runs.Skip(1).ToList()) run.Remove();

        var textElements = runs.First().Elements<WP.Text>();
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

    protected void Populate(SdtElement container, object dataModel)
    {
        if (container is not (SdtBlock or SdtRow)) throw new NotSupportedException();
        var sdtElements = container.Descendants<SdtElement>();
        var properties = dataModel.GetType().GetProperties();
        foreach (var sdtElement in sdtElements)
        {
            var tagValue = sdtElement.SdtProperties?.GetFirstChild<Tag>()?.Val?.Value;
            if (tagValue is null) continue;

            var property = properties.FirstOrDefault(p =>
                p.Name == tagValue ||
                p.GetCustomAttribute<DisplayAttribute>()?.Name == tagValue);
            if (property is null) continue;

            var propertyValue = property.GetValue(dataModel);
            try
            {
                Set(sdtElement, propertyValue);
            }
            catch (NotImplementedException)
            {
                continue;
            }
            catch (NotSupportedException)
            {
                continue;
            }
        }
    }
    #endregion
}
