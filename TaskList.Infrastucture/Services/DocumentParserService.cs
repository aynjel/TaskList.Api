using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Logging;
using TaskList.Application.Interfaces;

namespace TaskList.Infrastucture.Services;

public class DocumentParserService(ILogger<DocumentParserService> logger) : IDocumentParserService
{
    private static readonly HashSet<string> SupportedContentTypes =
    [
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/msword",
        "text/plain"
    ];

    public bool IsSupported(string contentType)
    {
        return SupportedContentTypes.Contains(contentType.ToLowerInvariant());
    }

    public async Task<string> ExtractTextAsync(Stream documentStream, string fileName, string contentType)
    {
        try
        {
            return contentType.ToLowerInvariant() switch
            {
                "application/pdf" => await ExtractFromPdfAsync(documentStream),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => await ExtractFromDocxAsync(documentStream),
                "text/plain" => await ExtractFromTextFileAsync(documentStream),
                _ => throw new NotSupportedException($"Content type {contentType} is not supported")
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting text from document: {FileName}", fileName);
            throw;
        }
    }

    private async Task<string> ExtractFromPdfAsync(Stream stream)
    {
        var sb = new StringBuilder();

        try
        {
            using var pdfReader = new PdfReader(stream);
            using var pdfDocument = new PdfDocument(pdfReader);

            for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                var currentText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);
                sb.AppendLine(currentText);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting text from PDF");
            throw;
        }

        return await Task.FromResult(sb.ToString());
    }

    private async Task<string> ExtractFromDocxAsync(Stream stream)
    {
        var sb = new StringBuilder();

        try
        {
            using var wordDocument = WordprocessingDocument.Open(stream, false);
            var body = wordDocument.MainDocumentPart?.Document.Body;

            if (body is not null)
            {
                foreach (var paragraph in body.Descendants<Paragraph>())
                {
                    sb.AppendLine(paragraph.InnerText);
                }

                // Extract from tables
                foreach (var table in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Table>())
                {
                    foreach (var row in table.Descendants<TableRow>())
                    {
                        var rowText = string.Join(" | ", row.Descendants<TableCell>().Select(c => c.InnerText));
                        sb.AppendLine(rowText);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting text from DOCX");
            throw;
        }

        return await Task.FromResult(sb.ToString());
    }

    private static async Task<string> ExtractFromTextFileAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}
