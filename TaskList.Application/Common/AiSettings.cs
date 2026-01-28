namespace TaskList.Application.Common;

public class AiSettings
{
    public string ServiceType { get; set; } = "OpenAI";
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = "gpt-4o";
    public string ModelId { get; set; } = "gpt-4o";
    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
    
    // Document Intelligence Settings
    public string DocumentIntelligenceEndpoint { get; set; } = string.Empty;
    public string DocumentIntelligenceApiKey { get; set; } = string.Empty;
}
