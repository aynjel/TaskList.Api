using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Controllers.Common;
using TaskList.Application.DTOs.AI;
using TaskList.Application.Interfaces;

namespace TaskList.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize]
public class AiController(
    IAiService aiService,
    ILogger<AiController> logger,
    IConfiguration configuration)
    : BaseApiController(logger, configuration)
{
    /// <summary>
    /// Get AI-powered task summary
    /// </summary>
    /// <returns>Conversational summary of user's tasks with metrics and insights</returns>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(TaskSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TaskSummaryResponse>> GetTaskSummary()
    {
        try
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized<TaskSummaryResponse>("Authentication required.");

            var summary = await aiService.GenerateTaskSummaryAsync(userId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return HandleException<TaskSummaryResponse>(ex, "Failed to generate task summary.");
        }
    }

    /// <summary>
    /// Extract tasks from uploaded document
    /// </summary>
    /// <param name="file">Document file (PDF, Word, or text)</param>
    /// <returns>Extracted tasks, contacts, and insights from the document</returns>
    [HttpPost("extract-from-document")]
    [ProducesResponseType(typeof(DocumentExtractionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [RequestSizeLimit(10_000_000)] // 10 MB limit
    public async Task<ActionResult<DocumentExtractionResponse>> ExtractTasksFromDocument(IFormFile file)
    {
        try
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized<DocumentExtractionResponse>("Authentication required.");

            // Validate file
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            var supportedTypes = new[] { "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };
            if (!supportedTypes.Contains(file.ContentType.ToLowerInvariant()))
                return BadRequest(new { message = "Unsupported file type. Supported types: PDF, DOCX, TXT" });

            if (file.Length > 10_000_000) // 10 MB
                return BadRequest(new { message = "File size exceeds maximum limit of 10 MB." });

            using var stream = file.OpenReadStream();
            var result = await aiService.ExtractTasksFromDocumentAsync(stream, file.FileName, file.ContentType);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException<DocumentExtractionResponse>(ex, "Failed to extract tasks from document.");
        }
    }

    /// <summary>
    /// Health check for AI services
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            service = "AI Service",
            status = "Healthy",
            timestamp = DateTime.UtcNow
        });
    }
}
