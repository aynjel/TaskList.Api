using System.Text;
using System.Text.Json;
using Mscc.GenerativeAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskList.Application.Common;
using TaskList.Application.DTOs.AI;
using TaskList.Application.DTOs.Tasks;
using TaskList.Application.Interfaces;
using TaskList.Domain.Enums;

namespace TaskList.Infrastucture.Services;

public class GoogleGeminiAiService : IAiService
{
    private readonly GoogleAI _googleAI;
    private readonly GenerativeModel _model;
    private readonly ITaskRepository _taskRepository;
    private readonly IDocumentParserService _documentParser;
    private readonly ILogger<GoogleGeminiAiService> _logger;
    private readonly AiSettings _aiSettings;

    public GoogleGeminiAiService(
        IOptions<AiSettings> aiSettings,
        ITaskRepository taskRepository,
        IDocumentParserService documentParser,
        ILogger<GoogleGeminiAiService> logger)
    {
        _aiSettings = aiSettings.Value;
        _taskRepository = taskRepository;
        _documentParser = documentParser;
        _logger = logger;

        // Initialize Gemini model - use gemini-1.5-flash as default (fast and reliable)
        var modelId = _aiSettings.ModelId ?? "gemini-1.5-flash";
        _googleAI = new GoogleAI(_aiSettings.ApiKey);
        _model = _googleAI.GenerativeModel(modelId);
        
        _logger.LogInformation("Google Gemini AI Service initialized with model: {ModelId}", modelId);
    }

    public async Task<TaskSummaryResponse> GenerateTaskSummaryAsync(string userId)
    {
        try
        {
            _logger.LogInformation("Generating AI task summary for user {UserId}", userId);

            // Get all user tasks
            var allTasks = await _taskRepository.GetAllByUserIdAsync(userId);

            // Calculate metrics
            var now = DateTime.UtcNow;
            var today = now.Date;
            var endOfWeek = today.AddDays(7);

            var metrics = new TaskMetrics
            {
                TotalTasks = allTasks.Count,
                DueToday = allTasks.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date == today),
                DueThisWeek = allTasks.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date > today && t.DueDate.Value.Date <= endOfWeek),
                Overdue = allTasks.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date < today && t.Status != TaskItemStatus.Completed),
                ByStatus = allTasks.GroupBy(t => t.Status.ToString()).ToDictionary(g => g.Key, g => g.Count()),
                ByPriority = allTasks.GroupBy(t => t.Priority.ToString()).ToDictionary(g => g.Key, g => g.Count()),
                ByCategory = allTasks.GroupBy(t => t.Category.ToString()).ToDictionary(g => g.Key, g => g.Count())
            };

            var tasksToday = allTasks
                .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == today)
                .OrderBy(t => t.Priority)
                .ToList();

            var upcomingTasks = allTasks
                .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date > today && t.DueDate.Value.Date <= endOfWeek)
                .OrderBy(t => t.DueDate)
                .ThenBy(t => t.Priority)
                .Take(10)
                .ToList();

            // Generate AI summary
            var prompt = CreateSummaryPrompt(metrics, tasksToday, upcomingTasks);
            var aiSummary = await GenerateAiTextAsync(prompt);

            return new TaskSummaryResponse
            {
                Summary = aiSummary,
                Metrics = metrics,
                TasksToday = [.. tasksToday.Select(MapToTaskResponse)],
                UpcomingTasks = [.. upcomingTasks.Select(MapToTaskResponse)]
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI task summary for user {UserId}", userId);
            throw;
        }
    }

    public async Task<DocumentExtractionResponse> ExtractTasksFromDocumentAsync(
        Stream documentStream, 
        string fileName, 
        string contentType)
    {
        try
        {
            _logger.LogInformation("Extracting tasks from document: {FileName}", fileName);

            var startTime = DateTime.UtcNow;

            // Extract text from document
            var documentText = await _documentParser.ExtractTextAsync(documentStream, fileName, contentType);

            if (string.IsNullOrWhiteSpace(documentText))
            {
                return new DocumentExtractionResponse
                {
                    AiInsights = "Unable to extract text from the document.",
                    Metadata = new ExtractionMetadata
                    {
                        ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds
                    }
                };
            }

            // Create prompt for task extraction
            var prompt = CreateExtractionPrompt(documentText);
            var aiResponse = await GenerateAiTextAsync(prompt);

            // Parse AI response
            var extraction = ParseExtractionResponse(aiResponse);

            // Calculate and populate metadata
            extraction.Metadata = CalculateExtractionMetadata(extraction, startTime, fileName);

            _logger.LogInformation("Successfully extracted {TaskCount} tasks from document", extraction.ExtractedTasks.Count);

            return extraction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting tasks from document: {FileName}", fileName);
            throw;
        }
    }

    private static string CreateSummaryPrompt(TaskMetrics metrics, List<Domain.Entities.TaskItem> tasksToday, List<Domain.Entities.TaskItem> upcomingTasks)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are a friendly and helpful AI assistant for a task management application.");
        sb.AppendLine("Generate a natural, conversational summary of the user's tasks.");
        sb.AppendLine("Be encouraging and provide actionable insights.");
        sb.AppendLine();
        sb.AppendLine("Task Metrics:");
        sb.AppendLine($"- Total tasks: {metrics.TotalTasks}");
        sb.AppendLine($"- Due today: {metrics.DueToday}");
        sb.AppendLine($"- Due this week: {metrics.DueThisWeek}");
        sb.AppendLine($"- Overdue: {metrics.Overdue}");
        sb.AppendLine();

        if (tasksToday.Count != 0)
        {
            sb.AppendLine("Tasks due today:");
            foreach (var task in tasksToday.Take(5))
            {
                sb.AppendLine($"- {task.Title} (Priority: {task.Priority}, Status: {task.Status})");
            }
            sb.AppendLine();
        }

        if (upcomingTasks.Count != 0)
        {
            sb.AppendLine("Upcoming tasks (next 7 days):");
            foreach (var task in upcomingTasks.Take(5))
            {
                sb.AppendLine($"- {task.Title} (Due: {task.DueDate:MMM dd}, Priority: {task.Priority})");
            }
            sb.AppendLine();
        }

        sb.AppendLine("Generate a friendly 2-3 paragraph summary that:");
        sb.AppendLine("1. Greets the user and provides an overview");
        sb.AppendLine("2. Highlights important tasks and deadlines");
        sb.AppendLine("3. Offers encouragement and productivity tips");
        sb.AppendLine();
        sb.AppendLine("Keep the tone conversational and supportive. Make it personal and helpful.");

        return sb.ToString();
    }

    private static string CreateExtractionPrompt(string documentText)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are an AI assistant that extracts task-related information from documents.");
        sb.AppendLine("Analyze the following document and extract:");
        sb.AppendLine("1. Action items/tasks (with titles, descriptions, due dates if mentioned)");
        sb.AppendLine("2. People/contacts mentioned (names, emails, roles)");
        sb.AppendLine("3. A brief summary of the document");
        sb.AppendLine("4. Key insights about urgency and priorities");
        sb.AppendLine();
        sb.AppendLine("Return the response in the following JSON format:");
        sb.AppendLine(@"{
          ""extractedTasks"": [
            {
              ""title"": ""string"",
              ""description"": ""string"",
              ""dueDate"": ""YYYY-MM-DD or null"",
              ""priority"": 1-3 (1=Low, 2=Medium, 3=High),
              ""suggestedCategory"": 1-3 (1=Work, 2=Personal, 3=Shopping),
              ""confidence"": 0.0-1.0,
              ""sourceText"": ""relevant excerpt from document""
            }
          ],
          ""extractedContacts"": [
            {
              ""name"": ""string"",
              ""email"": ""string or empty"",
              ""role"": ""string or empty""
            }
          ],
          ""documentSummary"": ""string"",
          ""aiInsights"": ""string - conversational insights about the tasks""
        }");
        sb.AppendLine();
        sb.AppendLine("Document content:");
        sb.AppendLine("---");
        sb.AppendLine(documentText.Length > 8000 ? string.Concat(documentText.AsSpan(0, 8000), "...") : documentText);
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("Return ONLY the JSON response, no additional text.");

        return sb.ToString();
    }

    private async Task<string> GenerateAiTextAsync(string prompt)
    {
        try
        {
            var generationConfig = new GenerationConfig
            {
                MaxOutputTokens = _aiSettings.MaxTokens,
                Temperature = (float)_aiSettings.Temperature
            };

            var response = await _model.GenerateContent(prompt, generationConfig);

            return response?.Text ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating text with Gemini");
            throw;
        }
    }

    private DocumentExtractionResponse ParseExtractionResponse(string aiResponse)
    {
        try
        {
            // Try to extract JSON from the response
            var jsonStart = aiResponse.IndexOf('{');
            var jsonEnd = aiResponse.LastIndexOf('}');

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonString = aiResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<DocumentExtractionResponse>(jsonString, options);
                return result ?? new DocumentExtractionResponse();
            }

            return new DocumentExtractionResponse
            {
                AiInsights = aiResponse,
                Metadata = new ExtractionMetadata()
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse AI extraction response");
            return new DocumentExtractionResponse
            {
                AiInsights = aiResponse,
                Metadata = new ExtractionMetadata()
            };
        }
    }

    private static ExtractionMetadata CalculateExtractionMetadata(
        DocumentExtractionResponse extraction, 
        DateTime startTime, 
        string fileName)
    {
        var tasks = extraction.ExtractedTasks;
        var now = DateTime.UtcNow;
        var urgentThreshold = now.AddDays(7);

        var metadata = new ExtractionMetadata
        {
            TotalTasksFound = tasks.Count,
            TotalContactsFound = extraction.ExtractedContacts.Count,
            TasksWithDueDates = tasks.Count(t => t.DueDate.HasValue),
            HighPriorityTasks = tasks.Count(t => t.Priority == 3),
            MediumPriorityTasks = tasks.Count(t => t.Priority == 2),
            LowPriorityTasks = tasks.Count(t => t.Priority == 1),
            UrgentTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value <= urgentThreshold),
            AverageConfidence = tasks.Any() 
                ? Math.Round(tasks.Average(t => t.Confidence), 2) 
                : 0.0,
            ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds,
            DocumentType = DetectDocumentType(fileName, extraction),
            TasksByCategory = tasks
                .GroupBy(t => GetCategoryName(t.SuggestedCategory))
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return metadata;
    }

    private static string DetectDocumentType(string fileName, DocumentExtractionResponse extraction)
    {
        var lowerFileName = fileName.ToLowerInvariant();
        var textContent = extraction.DocumentSummary.ToLowerInvariant();

        // Simple heuristic detection
        if (lowerFileName.Contains("meeting") || textContent.Contains("meeting notes") || textContent.Contains("attendees"))
            return "Meeting Notes";
        
        if (lowerFileName.Contains("email") || textContent.Contains("from:") || textContent.Contains("subject:"))
            return "Email";
        
        if (lowerFileName.Contains("project") || lowerFileName.Contains("plan") || textContent.Contains("deliverable"))
            return "Project Plan";
        
        if (lowerFileName.Contains("sprint") || textContent.Contains("sprint") || textContent.Contains("story"))
            return "Sprint Planning";
        
        if (lowerFileName.Contains("requirement") || textContent.Contains("requirement"))
            return "Requirements Document";
        
        if (lowerFileName.Contains("todo") || lowerFileName.Contains("task"))
            return "Task List";

        return "General Document";
    }

    private static string GetCategoryName(int category)
    {
        return category switch
        {
            1 => "Work",
            2 => "Personal",
            3 => "Shopping",
            _ => "Other"
        };
    }

    private TaskResponse MapToTaskResponse(Domain.Entities.TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            Category = task.Category,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            LastModifiedAt = task.LastModifiedAt
        };
    }
}
