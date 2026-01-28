namespace TaskList.Application.Interfaces;

public interface IDocumentParserService
{
    /// <summary>
    /// Extract text content from various document formats
    /// </summary>
    Task<string> ExtractTextAsync(Stream documentStream, string fileName, string contentType);
    
    /// <summary>
    /// Check if the file type is supported
    /// </summary>
    bool IsSupported(string contentType);
}
