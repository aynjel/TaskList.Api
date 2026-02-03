# AI Configuration Examples

This document provides configuration examples for switching between Azure OpenAI and Google Gemini AI providers.

## Overview

The TaskList API supports two AI providers:
1. **Azure OpenAI** - Microsoft's Azure-hosted OpenAI service
2. **Google Gemini** - Google's Gemini AI models

You can easily switch between them by changing the `ServiceType` in your `appsettings.json` or `appsettings.Development.json` file.

---

## Azure OpenAI Configuration

### Prerequisites
- Azure subscription
- Azure OpenAI resource created in Azure Portal
- Deployed model (e.g., gpt-4o-mini, gpt-4, gpt-3.5-turbo)
- API key and endpoint from Azure Portal

### Configuration

```json
{
  "AiSettings": {
    "ServiceType": "AzureOpenAI",
    "ApiKey": "your-azure-openai-api-key",
    "Endpoint": "https://your-resource-name.openai.azure.com/",
    "DeploymentName": "gpt-4o-mini",
    "ModelId": "gpt-4o-mini",
    "MaxTokens": 2000,
    "Temperature": 0.7
  }
}
```

### Parameters Explained

| Parameter | Required | Description | Example |
|-----------|----------|-------------|---------|
| `ServiceType` | ? Yes | Must be "AzureOpenAI" | `"AzureOpenAI"` |
| `ApiKey` | ? Yes | Your Azure OpenAI API key | `"abc123..."` |
| `Endpoint` | ? Yes | Your Azure OpenAI endpoint URL | `"https://myresource.openai.azure.com/"` |
| `DeploymentName` | ? Yes | The deployment name in Azure (not the model name) | `"gpt-4o-mini"` |
| `ModelId` | ?? Optional | The model identifier (recommended) | `"gpt-4o-mini"` |
| `MaxTokens` | ?? Optional | Maximum tokens in response (default: 2000) | `2000` |
| `Temperature` | ?? Optional | Creativity level 0.0-1.0 (default: 0.7) | `0.7` |

### How to Get Your Azure OpenAI Credentials

1. Go to [Azure Portal](https://portal.azure.com/)
2. Navigate to your Azure OpenAI resource
3. Click on **"Keys and Endpoint"** in the left menu
4. Copy:
   - **KEY 1** or **KEY 2** ? Use as `ApiKey`
   - **Endpoint** ? Use as `Endpoint`
5. Go to **"Model deployments"** or **"Go to Azure OpenAI Studio"**
6. Copy the **Deployment name** ? Use as `DeploymentName`

---

## Google Gemini Configuration

### Prerequisites
- Google account
- API key from Google AI Studio

### Configuration

```json
{
  "AiSettings": {
    "ServiceType": "GoogleGemini",
    "ApiKey": "your-google-gemini-api-key",
    "ModelId": "gemini-3-flash-preview",
    "MaxTokens": 2000,
    "Temperature": 0.7
  }
}
```

### Parameters Explained

| Parameter | Required | Description | Example |
|-----------|----------|-------------|---------|
| `ServiceType` | ? Yes | Must be "GoogleGemini" | `"GoogleGemini"` |
| `ApiKey` | ? Yes | Your Google AI API key | `"AIza..."` |
| `ModelId` | ?? Optional | Gemini model to use (default: gemini-2.0-flash-exp) | `"gemini-2.0-flash-exp"` |
| `MaxTokens` | ?? Optional | Maximum tokens in response (default: 2000) | `2000` |
| `Temperature` | ?? Optional | Creativity level 0.0-1.0 (default: 0.7) | `0.7` |

**Note:** When using Google Gemini, the `Endpoint` and `DeploymentName` fields are **not required** and will be ignored.

### How to Get Your Google Gemini API Key

1. Go to [Google AI Studio](https://makersuite.google.com/app/apikey)
2. Sign in with your Google account
3. Click **"Create API Key"**
4. Select a Google Cloud project (or create one)
5. Copy the API key
6. Use it as the `ApiKey` in your configuration

### Available Gemini Models

| Model ID | Description | Use Case |
|----------|-------------|----------|
| `gemini-1.5-flash` | **Recommended** - Fast, efficient, and reliable | General use, fast responses |
| `gemini-1.5-pro` | Advanced reasoning and longer context | Complex tasks, detailed analysis |
| `gemini-pro` | Balanced performance | General purpose tasks |

**Note:** The experimental `gemini-2.0-flash-exp` model may not be available in all regions. Use `gemini-1.5-flash` for best reliability.

---

## Switching Between Providers

### Development Environment

Edit `appsettings.Development.json`:

**For Azure OpenAI:**
```json
{
  "AiSettings": {
    "ServiceType": "AzureOpenAI",
    "ApiKey": "api-key-here",
    "Endpoint": "endpoint-here",
    "DeploymentName": "gpt-4o-mini",
    "ModelId": "gpt-4o-mini",
    "MaxTokens": 2000,
    "Temperature": 0.7
  }
}
```

**For Google Gemini:**
```json
{
  "AiSettings": {
    "ServiceType": "GoogleGemini",
    "ApiKey": "api-key-here",
    "ModelId": "gemini-3-flash-preview",
    "MaxTokens": 2000,
    "Temperature": 0.7
  }
}
```

### Production Environment

Edit `appsettings.json` or use environment variables:

```bash
# Azure OpenAI
export AiSettings__ServiceType="AzureOpenAI"
export AiSettings__ApiKey="your-azure-key"
export AiSettings__Endpoint="https://your-resource.openai.azure.com/"
export AiSettings__DeploymentName="gpt-4o-mini"

# OR Google Gemini
export AiSettings__ServiceType="GoogleGemini"
export AiSettings__ApiKey="your-gemini-key"
export AiSettings__ModelId="gemini-2.0-flash-exp"
```

---

## Feature Compatibility

Both AI providers support the same features in the TaskList API:

| Feature | Azure OpenAI | Google Gemini |
|---------|:------------:|:-------------:|
| Task Summary Generation | ? | ? |
| Document Task Extraction | ? | ? |
| PDF Processing | ? | ? |
| Word Document Processing | ? | ? |
| Email (MSG) Processing | ? | ? |
| Text File Processing | ? | ? |
| Confidence Scoring | ? | ? |
| Auto-categorization | ? | ? |
| Priority Detection | ? | ? |

---

## Configuration Best Practices

### Security
- ?? **Never commit API keys to version control**
- Use environment variables for production
- Consider using Azure Key Vault or Google Secret Manager
- Rotate API keys regularly

### Performance
- **MaxTokens**: Start with 2000, adjust based on needs
  - Lower values = faster responses, potentially truncated
  - Higher values = slower responses, more detailed
  
- **Temperature**: 
  - `0.0-0.3` = More deterministic, consistent results
  - `0.4-0.7` = Balanced creativity and consistency
  - `0.8-1.0` = More creative, varied responses

### Cost Optimization
- **Azure OpenAI**: Pay per token (input + output)
- **Google Gemini**: Free tier available, then pay per request
- Monitor usage in respective dashboards
- Use appropriate MaxTokens to avoid unnecessary costs

### Testing Both Providers

```http
### Test with Azure OpenAI
POST https://localhost:7191/api/v1/ai/summary
Authorization: Bearer {{accessToken}}
# Ensure appsettings has ServiceType: "AzureOpenAI"

### Test with Google Gemini
POST https://localhost:7191/api/v1/ai/summary
Authorization: Bearer {{accessToken}}
# Change appsettings to ServiceType: "GoogleGemini"
```

---

## Troubleshooting

### Azure OpenAI Issues

**Error: "The API deployment for this resource does not exist"**
- Check that `DeploymentName` matches exactly with Azure Portal
- Verify the deployment is active in Azure OpenAI Studio

**Error: "Access denied"**
- Verify API key is correct
- Check endpoint URL format includes trailing slash
- Ensure your Azure subscription is active

### Google Gemini Issues

**Error: "API key not valid"**
- Verify the API key is copied correctly
- Check that the API key hasn't been restricted or revoked
- Ensure Google AI API is enabled for your project

**Error: "Model not found"**
- Check the `ModelId` is correct
- Try using `gemini-2.0-flash-exp` (recommended default)
- Some models may not be available in all regions

### General Issues

**Error: "Service type not recognized"**
- Ensure `ServiceType` is either "AzureOpenAI" or "GoogleGemini" (case-insensitive)
- Check for typos in configuration

**Slow responses**
- Reduce `MaxTokens` value
- Consider using faster models (e.g., gemini-1.5-flash)
- Check network connectivity

---

## Example Responses

Both providers return identical response structures:

```json
{
  "summary": "Good morning! You have a productive day ahead with 5 tasks due today...",
  "metrics": {
    "totalTasks": 15,
    "dueToday": 5,
    "dueThisWeek": 8,
    "overdue": 2
  },
  "tasksToday": [...],
  "upcomingTasks": [...]
}
```

The application abstracts the AI provider differences, so your API consumers don't need to know which provider is being used.

---

## Support

For issues specific to:
- **Azure OpenAI**: [Azure OpenAI Documentation](https://learn.microsoft.com/en-us/azure/ai-services/openai/)
- **Google Gemini**: [Google AI Documentation](https://ai.google.dev/docs)
- **TaskList API**: Check the [main README](README.md) or create an issue on GitHub
