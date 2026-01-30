using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Logging;
using MsgReader.Outlook;
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
            // Check if it's a .msg file by extension, as content type can be unreliable
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            if (fileExtension == ".msg")
            {
                return await ExtractFromMsgAsync(documentStream, fileName);
            }

            return contentType.ToLowerInvariant() switch
            {
                "application/pdf" => await ExtractFromPdfAsync(documentStream),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => await ExtractFromDocxAsync(documentStream),
                "text/plain" => await ExtractFromTextFileAsync(documentStream),
                "application/vnd.ms-outlook" => await ExtractFromMsgAsync(documentStream, fileName),
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

    private async Task<string> ExtractFromMsgAsync(Stream stream, string fileName)
    {
        var sb = new StringBuilder();

        try
        {
            // Save stream to a temporary file as MsgReader requires a file path
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{Path.GetExtension(fileName)}");
            
            try
            {
                await using (var fileStream = File.Create(tempPath))
                {
                    stream.Position = 0;
                    await stream.CopyToAsync(fileStream);
                }

                using var msg = new Storage.Message(tempPath);
                
                // Extract basic email information
                sb.AppendLine($"From: {msg.Sender?.Email ?? "Unknown"}");
                sb.AppendLine($"Subject: {msg.Subject ?? "No Subject"}");
                sb.AppendLine($"Date: {msg.SentOn}");
                
                // Recipients
                if (msg.Recipients != null && msg.Recipients.Count > 0)
                {
                    var recipients = new List<string>();
                    foreach (Storage.Recipient recipient in msg.Recipients)
                    {
                        if (!string.IsNullOrEmpty(recipient.Email))
                        {
                            recipients.Add(recipient.Email);
                        }
                    }
                    if (recipients.Count != 0)
                    {
                        sb.AppendLine($"Recipients: {string.Join(", ", recipients)}");
                    }
                }
                
                sb.AppendLine();
                
                // Extract body text
                if (!string.IsNullOrEmpty(msg.BodyText))
                {
                    sb.AppendLine("Body:");
                    sb.AppendLine(msg.BodyText);
                }
                else if (!string.IsNullOrEmpty(msg.BodyHtml))
                {
                    sb.AppendLine("Body (HTML):");
                    // Simple HTML tag removal for better text extraction
                    var bodyText = Regex.Replace(msg.BodyHtml, "<.*?>", string.Empty);
                    sb.AppendLine(bodyText);
                }

                // Extract attachments information
                if (msg.Attachments != null && msg.Attachments.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine($"Attachments ({msg.Attachments.Count}):");
                    foreach (var attachment in msg.Attachments)
                    {
                        if (attachment is Storage.Attachment att)
                        {
                            sb.AppendLine($"- {att.FileName}");
                        }
                    }
                }
            }
            finally
            {
                // Clean up temporary file
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting text from MSG file");
            throw;
        }

        return await Task.FromResult(sb.ToString());
    }
}
