using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SS = DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using WP = DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace EngineeringFort.OpenXml;

/// <summary>
///     <para>
///         Provides bidirectional conversion between data objects and Open XML documents (Excel and Word).
///     </para>
///     <para>
///         Supports
///         <list type="bullet">
///             <item>extraction of structured data from existing documents, and</item>
///             <item>document generation from templates with data population via tagged content controls.</item>
///         </list>
///     </para>
/// </summary>
public class Converter
{
    #region Fields
    private readonly IFileProvider? _fileProvider;
    private readonly ILogger? _logger;
    private readonly UnitSystemService? _unitSystemService;
    #endregion

    #region Constructors
    public Converter() { }
    public Converter(ILogger logger, IFileProvider fileProvider, UnitSystemService unitSystemService)
    {
        _logger = logger;
        _fileProvider = fileProvider;
        _unitSystemService = unitSystemService;
    }
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
    public Dictionary<string, Dictionary<string, string>[]> ExtractStringData(Stream stream)
    {
        throw new NotImplementedException();
    }

    private Dictionary<string, string>[] ExtractStringData(Worksheet worksheet, SS.Table table)
    {
        throw new NotImplementedException();
    }

    private Dictionary<string, string> ExtractStringData(Row row, IEnumerable<TableColumn> tableColumns)
    {
        throw new NotImplementedException();
    }

    private string ExtractStringData(Cell cell)
    {
        throw new NotImplementedException();
    }

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

        if (_fileProvider is not null)
        {
            var fileInfo = _fileProvider.GetFileInfo(path);
            if (fileInfo.Exists)
            {
                using var stream = fileInfo.CreateReadStream();
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return extension switch
                {
                    ".docx" or ".dotx" => WordprocessingDocument.Open(memoryStream, isEditable: false),
                    ".xlsx" or ".xltx" => SpreadsheetDocument.Open(memoryStream, isEditable: false),
                    _ => throw new NotSupportedException(),
                };
            }
        }

        return extension switch
        {
            ".docx" or ".dotx" => WordprocessingDocument.Open(path, isEditable: false),
            ".xlsx" or ".xltx" => SpreadsheetDocument.Open(path, isEditable: false),
            _ => throw new NotSupportedException(),
        };
    }

    protected TElement[] GenerateElements<TElement>(TElement template, IEnumerable<object> objects)
        where TElement : OpenXmlCompositeElement
    {
        var elements = objects.Select(_ => (TElement)template.Clone()).ToArray();
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

    protected TElement[] GenerateElements<TElement>(TElement[] templates, IEnumerable<object> objects)
        where TElement : OpenXmlCompositeElement
    {
        var elements = objects.Select(obj =>
        {
            var template = templates.OfType<SdtElement>().FirstOrDefault(sdt =>
                sdt.SdtProperties?.GetFirstChild<Tag>()?.Val == obj.GetType().Name) as OpenXmlCompositeElement ??
                templates.First();
            return (TElement)template.Clone();
        }).ToArray();
        var previousElement = templates.Last();
        foreach (var (element, obj) in elements.Zip(objects))
        {
            previousElement.InsertAfterSelf(element);
            if (element is SdtElement sdtElement) Set(sdtElement, obj);
            previousElement = element;
        }
        templates.ToList().ForEach(t => t.Remove());
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

    protected void Set(SdtBlock sdtBlock, Array array)
    {
        var templates = sdtBlock.SdtContentBlock!.Elements<SdtElement>().ToArray();
        var objects = array.OfType<object>(); // TODO: use array.Cast<object>() instead to include null elements (sparse array)
        GenerateElements(templates, objects);
    }

    protected void Set(SdtElement sdtElement, object? obj, string? format = null)
    {
        switch (obj)
        {
            case bool b:
                throw new NotImplementedException();
            case int or double or Enum or IQuantity:
                Set(sdtElement, Format(obj, format));
                break;
            case string s:
                Set(sdtElement, s);
                break;
            case null:
                throw new NotImplementedException();
            case Array a:
                if (sdtElement is SdtBlock sdtBlock) Set(sdtBlock, a);
                break;
            case object when obj.GetType().IsClass:
                Populate(sdtElement, obj);
                break;
            default:
                throw new NotSupportedException();
        }
    }

    private static string Format(object obj, string? format = null) => obj switch
    {
        int i => i.ToString(format ?? "G"),
        double d => d.ToString(format ?? "0.0##"),
        Enum e => e.ToString(format ?? "G"),
        IQuantity q =>
            format is null ? q.ToString("0.0##", formatProvider: null) :
            // TODO: refactor
            format.EndsWith(" omit") ? ((Func<string>)(() =>
            {
                var formatParts = format.Split(' ');
                switch (formatParts.Length)
                {
                    case 2:
                        return q.Value.ToString(formatParts[0], formatProvider: null);
                    case 3:
                        var unit = UnitParser.Default.Parse(formatParts[1], q.Unit.GetType());
                        var value = q.As(unit);
                        return value.ToString(formatParts[0]);
                    default:
                        throw new InvalidOperationException();
                }
            }))() :
            q.ToString(format, formatProvider: null),
        _ => obj.ToString() ?? string.Empty,
    };

    protected void Populate(SdtElement container, object dataModel)
    {
        if (container is not (SdtBlock or SdtRun or SdtRow)) throw new NotSupportedException();
        var sdtElements = container.Descendants<SdtElement>();
        var properties = dataModel.GetType().GetProperties();
        foreach (var sdtElement in sdtElements)
        {
            var tagValue = sdtElement.SdtProperties?.GetFirstChild<Tag>()?.Val?.Value;
            if (tagValue is null) continue;

            var property = properties.FirstOrDefault(prop =>
            {
                var name = prop.Name;
                var displayName = prop.GetCustomAttribute<DisplayAttribute>()?.Name;
                var localizedName = DisplayStrings.ResourceManager.GetString(name);
                var localizedDisplayName = displayName is not null ? DisplayStrings.ResourceManager.GetString(displayName) : null;
                return tagValue == name || tagValue == displayName || tagValue == localizedName || tagValue == localizedDisplayName;
            });
            if (property is null) continue;

            var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
            var displayFormatAttribute = property.GetCustomAttribute<DisplayFormatAttribute>();
            var format = displayFormatAttribute?.DataFormatString ?? _unitSystemService?.GetFormats(property)?.FirstOrDefault();
            var propertyValue = property.GetValue(dataModel);
            try
            {
                Set(sdtElement, propertyValue, format);
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
