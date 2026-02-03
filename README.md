# TaskList API with AI Integration

A modern, AI-powered task management REST API built with .NET 10, featuring intelligent task summarization and document extraction capabilities using Azure OpenAI.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Installation](#installation)
  - [Configuration](#configuration)
  - [Database Setup](#database-setup)
  - [Running the Application](#running-the-application)
- [Authentication](#authentication)
- [Database Schema](#database-schema)
- [API Endpoints](#api-endpoints)
- [AI Features](#ai-features)
  - [Task Summary](#task-summary)
  - [Document Extraction](#document-extraction)
  - [Setup Guide](#ai-setup-guide)
- [Project Structure](#project-structure)
- [Configuration](#detailed-configuration)
- [Testing](#testing)
- [Deployment](#deployment)
- [Known Limitations](#known-limitations)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

---

## 🎯 Overview

TaskList API is a comprehensive task management system that leverages artificial intelligence to enhance productivity. The application provides traditional CRUD operations for task management while incorporating cutting-edge AI features for intelligent task analysis and automated task extraction from documents.

### Key Highlights

- **RESTful API Design**: Clean, consistent API following REST principles
- **AI-Powered Insights**: Natural language task summaries powered by Azure OpenAI or Google Gemini
- **Document Intelligence**: Automatic task extraction from PDF, Word, Text, and Outlook email files
- **Clean Architecture**: Separation of concerns with Domain-Driven Design
- **Type-Safe**: Strong typing throughout the application
- **Secure**: JWT-based authentication with refresh tokens
- **Scalable**: Built for production with modern .NET 10

---

## ✨ Features

### Core Features
- ✅ **User Authentication**: JWT-based authentication with refresh tokens
- ✅ **Task Management**: Full CRUD operations for tasks
- ✅ **Advanced Filtering**: Filter by status, priority, category, and date
- ✅ **Pagination**: Header-based pagination with metadata
- ✅ **Sorting**: Flexible sorting options

### AI-Powered Features
- 🤖 **AI Task Summary**: Conversational overview of your tasks
- 📄 **Document Extraction**: Extract tasks from PDF, DOCX, TXT, and MSG files
- 📧 **Email Processing**: Extract action items directly from Outlook emails
- 🎯 **Batch Creation**: Create multiple tasks from extracted data
- 📊 **Confidence Scoring**: AI confidence levels for each extraction
- 🏷️ **Auto-Categorization**: Automatic task priority and category detection

### Technical Features
- 🔐 **Secure by Design**: JWT authentication, HTTPS, CORS configured
- 📝 **API Documentation**: OpenAPI/Swagger with Scalar UI
- 🗃️ **Entity Framework Core**: Code-first database with migrations
- 📊 **Structured Logging**: Comprehensive logging with Serilog
- ✅ **Data Validation**: FluentValidation for request validation
- 🔄 **API Versioning**: Support for multiple API versions

---

## 🛠️ Technology Stack

### Backend Framework
- **.NET 10**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **C# 13**: Latest C# language features

### Database
- **SQL Server**: Relational database
- **Entity Framework Core 10**: ORM for data access

### AI & ML
- **Google Gemini**: Primary AI provider (Default)
- **Azure OpenAI**: Alternative AI provider support
- **Semantic Kernel 1.30**: Microsoft's AI orchestration framework (for Azure OpenAI)

### Authentication & Security
- **JWT (JSON Web Tokens)**: Stateless authentication
- **ASP.NET Core Identity**: User management

### Libraries & Tools
- **FluentValidation**: Request validation
- **Serilog**: Structured logging
- **Scalar**: Modern API documentation
- **iText7**: PDF processing
- **DocumentFormat.OpenXml**: Word document processing
- **MsgReader**: Outlook MSG file processing
- **Mscc.GenerativeAI**: Google Gemini integration

---

## 🏗️ Architecture

The application follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                   Presentation Layer                    │
│                    (TaskList.Api)                       │
│  • Controllers  • Middleware  • Program.cs              │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                 Application Layer                       │
│               (TaskList.Application)                    │
│  • Interfaces  • DTOs  • Validators  • Common           │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│               Infrastructure Layer                      │
│              (TaskList.Infrastructure)                  │
│  • Services  • Repositories  • DbContext  • Config      │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                    Domain Layer                         │
│                  (TaskList.Domain)                      │
│  • Entities  • Enums  • Domain Logic                    │
└─────────────────────────────────────────────────────────┘
```

### Design Patterns Used
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Loose coupling
- **Unit of Work**: Transaction management
- **Strategy Pattern**: Document parsing for different formats
- **Factory Pattern**: Object creation

---

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- ✅ [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- ✅ [SQL Server 2019+](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- ✅ [Git](https://git-scm.com/downloads)
- ✅ Azure OpenAI Resource (for AI features)

### Optional Tools
- [Postman](https://www.postman.com/) for API testing
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for containerization)

---

## 🚀 Getting Started

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/aynjel/TaskList.Api.git
   cd TaskList.Api
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

### Configuration

1. **Update Database Connection String**

   Edit `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TaskListDb;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

   For SQL Server Authentication:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=TaskListDb;User ID=sa;Password=YourPassword;TrustServerCertificate=True"
     }
   }
   ```

2. **Configure JWT Settings**

   Edit `appsettings.json`:
   ```json
   {
     "JwtSettings": {
       "Secret": "your-secret-key-min-32-characters-long",
       "Issuer": "TaskListApi",
       "Audience": "TaskListClient",
       "ExpirationInMinutes": 30,
       "RefreshTokenExpirationInDays": 7
     }
   }
   ```

3. **Configure AI Settings** (See [AI Setup Guide](#ai-setup-guide))

**Default Configuration (Google Gemini):**
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

**Alternative (Azure OpenAI):**
```json
{
  "AiSettings": {
    "ServiceType": "AzureOpenAI",
    "ApiKey": "your-azure-openai-key",
    "Endpoint": "https://your-resource.openai.azure.com/",
    "DeploymentName": "gpt-4o-mini",
    "ModelId": "gpt-4o-mini",
    "MaxTokens": 2000,
    "Temperature": 0.7
  }
}
```

### Database Setup

**Create the database** (automatically on first run)
   ```bash
   dotnet ef database update --project TaskList.Infrastucture --startup-project TaskList.Api
   ```

   Or let the application create it automatically:
   ```csharp
   // Program.cs already includes automatic migration
   using (var scope = app.Services.CreateScope())
   {
       var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
       await dbInitializer.InitializeAsync();
   }
   ```

### Running the Application

1. **Run the API**
   ```bash
   dotnet run --project TaskList.Api
   ```

   Or with hot reload:
   ```bash
   dotnet watch run --project TaskList.Api
   ```

2. **Access the application**
   - **API**: https://localhost:7191
   - **HTTP**: http://localhost:5045
   - **Scalar UI**: https://localhost:7191/scalar/v1

3. **Test with HTTP files**
   - Open `TaskList.Api-Auth.http` for authentication examples
   - Open `TaskList.Api-Tasks.http` for task management examples
   - Open `TaskList.Api-AI.http` for AI feature examples

---

## 🔐 Authentication

The API uses **JWT (JSON Web Token)** based authentication with refresh token support.

### Authentication Flow

```
1. User Login
   POST /api/v1/auth/login
   ↓
2. Receive JWT Access Token + Refresh Token
   Access Token: Short-lived (30 minutes)
   Refresh Token: Long-lived (7 days, HTTP-only cookie)
   ↓
3. Use Access Token in Authorization Header
   Authorization: Bearer {access-token}
   ↓
4. When Access Token Expires
   POST /api/v1/auth/refresh
   ↓
5. Receive New Access Token
```

### Token Structure

**Access Token Claims:**
```json
{
  "sub": "user-id",
  "email": "user@example.com",
  "name": "User Name",
  "role": "User",
  "jti": "unique-token-id",
  "exp": 1234567890,
  "iss": "TaskListApi",
  "aud": "TaskListClient"
}
```

### Security Features

- ✅ **Password Hashing**: BCrypt with salt
- ✅ **Refresh Tokens**: Stored in database, revocable
- ✅ **HTTP-Only Cookies**: Refresh tokens not accessible via JavaScript
- ✅ **Token Expiration**: Short-lived access tokens
- ✅ **Role-Based Access**: Admin and User roles
- ✅ **CORS Protection**: Configured for specific origins

### Example Usage

**Login:**
```http
POST https://localhost:7191/api/v1/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "refresh-token-guid",
  "expiresIn": 1800
}
```

**Using the Token:**
```http
GET https://localhost:7191/api/v1/tasks
Authorization: Bearer eyJhbGc...
```

---

## 🗄️ Database Schema

The application uses **Entity Framework Core** with SQL Server.

### Entity Relationship Diagram

```
┌─────────────────────┐
│   AspNetUsers       │
│─────────────────────│
│ Id (PK)            │
│ Email              │
│ PasswordHash       │
│ UserName           │
└──────────┬──────────┘
           │ 1
           │
           │ *
┌──────────▼──────────┐
│   TaskItems         │
│─────────────────────│
│ Id (PK)            │
│ Title              │
│ Description        │
│ DueDate            │
│ Priority (enum)    │
│ Category (enum)    │
│ Status (enum)      │
│ UserId (FK)        │
│ CreatedAt          │
│ LastModifiedAt     │
└─────────────────────┘
```

### Tables

#### **AspNetUsers**
Identity framework tables for user management:
- `AspNetUsers`: User accounts
- `AspNetRoles`: Roles (User, Admin)
- `AspNetUserRoles`: User-role relationships
- `AspNetUserTokens`: Refresh tokens

#### **TaskItems**
Main tasks table:

| Column | Type | Description |
|--------|------|-------------|
| `Id` | int | Primary key (auto-increment) |
| `Title` | nvarchar(200) | Task title (required) |
| `Description` | nvarchar(max) | Task description (optional) |
| `DueDate` | datetime2 | Due date (nullable) |
| `Priority` | int | 1=Low, 2=Medium, 3=High |
| `Category` | int | 1=Work, 2=Personal, 3=Shopping |
| `Status` | int | 0=Todo, 1=InProgress, 2=Completed |
| `UserId` | nvarchar(450) | Foreign key to AspNetUsers |
| `CreatedAt` | datetime2 | Created timestamp |
| `LastModifiedAt` | datetime2 | Last modified timestamp (nullable) |

### Enums

**TaskItemStatus:**
```csharp
public enum TaskItemStatus
{
    Todo = 0,
    InProgress = 1,
    Completed = 2
}
```

**PriorityLevel:**
```csharp
public enum PriorityLevel
{
    Low = 1,
    Medium = 2,
    High = 3
}
```

**CategoryType:**
```csharp
public enum CategoryType
{
    Work = 1,
    Personal = 2,
    Shopping = 3
}
```

### Indexes

- `UserId` on `TaskItems` (for filtering by user)
- `Status` on `TaskItems` (for filtering by status)
- `DueDate` on `TaskItems` (for date-based queries)
- Unique index on `Email` in `AspNetUsers`

---

## 📡 API Endpoints

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/v1/auth/register` | Register new user | No |
| POST | `/api/v1/auth/login` | User login | No |
| POST | `/api/v1/auth/refresh` | Refresh access token | No |
| POST | `/api/v1/auth/logout` | Logout | Yes |
| GET | `/api/v1/auth/me` | Get current user info | Yes |

### Task Management Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/v1/tasks` | Get paginated tasks | Yes |
| GET | `/api/v1/tasks/{id}` | Get task by ID | Yes |
| POST | `/api/v1/tasks` | Create new task | Yes |
| PUT | `/api/v1/tasks/{id}` | Update task | Yes |
| PATCH | `/api/v1/tasks/{id}/status` | Update task status only | Yes |
| DELETE | `/api/v1/tasks/{id}` | Delete task | Yes |
| POST | `/api/v1/tasks/create-from-extraction` | Create tasks from extraction | Yes |

### AI Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/v1/ai/summary` | Get AI task summary | Yes |
| POST | `/api/v1/ai/extract-from-document` | Extract tasks from document | Yes |
| GET | `/api/v1/ai/health` | AI service health check | No |

### Query Parameters

**Task List (`GET /api/v1/tasks`):**
```
?PageNumber=1              # Page number (default: 1)
&PageSize=10               # Items per page (default: 10)
&Status=1                  # Filter by status (0,1,2)
&Priority=3                # Filter by priority (1,2,3)
&Category=1                # Filter by category (1,2,3)
&StartDate=2026-01-01      # Filter by start date
&EndDate=2026-12-31        # Filter by end date
&SortBy=DueDate            # Sort field
&SortDescending=true       # Sort order
```

### Response Headers

**Pagination Metadata:**
```
X-Pagination: {"currentPage":1,"pageSize":10,"totalCount":45,"totalPages":5,"hasPrevious":false,"hasNext":true}
```

---

## 🤖 AI Features

The application integrates with **OpenAI GPT models** using **Microsoft Semantic Kernel** for intelligent task management.

### Task Summary

**Endpoint:** `GET /api/v1/ai/summary`

**Description:** Generates a conversational, user-friendly summary of your tasks including:
- Total task count with breakdown
- Tasks due today (with list)
- Tasks due in the next 7 days (with list)
- Overdue task count
- AI-generated insights and recommendations

**Example Response:**
```json
{
  "summary": "Good morning! You have 15 tasks total. Today looks manageable with just 2 tasks due: 'Complete project documentation' and 'Review pull requests'. Looking ahead, you have 5 tasks due in the next week. I'd recommend prioritizing the high-priority items first. You're on track! Keep up the great work! 🎯",
  "metrics": {
    "totalTasks": 15,
    "dueToday": 2,
    "dueThisWeek": 5,
    "overdue": 1,
    "byStatus": {
      "Todo": 8,
      "InProgress": 5,
      "Completed": 2
    },
    "byPriority": {
      "High": 3,
      "Medium": 7,
      "Low": 5
    }
  },
  "tasksToday": [...],
  "upcomingTasks": [...]
}
```

### Document Extraction

**Endpoint:** `POST /api/v1/ai/extract-from-document`

**Description:** Extracts tasks and relevant information from uploaded documents.

**Supported Formats:**
- PDF (`.pdf`)
- Word Documents (`.docx`)
- Text Files (`.txt`)
- Outlook Email Messages (`.msg`)

**What It Extracts:**
- ✅ Action items and tasks
- ✅ Due dates (when mentioned)
- ✅ Priority levels (AI-inferred)
- ✅ People/contacts with emails
- ✅ Document summary
- ✅ AI insights about urgency
- ✅ Email metadata (sender, recipients, subject, date) from .msg files
- ✅ Email body content and attachments list from .msg files

**Example Response:**
```json
{
  "extractedTasks": [
    {
      "title": "Submit quarterly report",
      "description": "Q4 2025 financial summary",
      "dueDate": "2026-02-15",
      "priority": 3,
      "suggestedCategory": 1,
      "confidence": 0.95,
      "sourceText": "The quarterly report is due by February 15th..."
    }
  ],
  "extractedContacts": [
    {
      "name": "John Doe",
      "email": "john@example.com",
      "role": "Project Manager"
    }
  ],
  "documentSummary": "Meeting notes discussing Q1 deliverables...",
  "aiInsights": "I found 5 action items with specific due dates. The most urgent is the quarterly report due in 3 weeks.",
  "metadata": {
    "totalTasksFound": 5,
    "totalContactsFound": 3,
    "highPriorityTasks": 2,
    "averageConfidence": 0.89,
    "documentType": "Meeting Notes",
    "processingTimeMs": 2340
  }
}
```

**Example Request (using cURL):**

```bash
# Upload a PDF document
curl -X POST "https://localhost:7191/api/v1/ai/extract-from-document" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -F "file=@meeting-notes.pdf"

# Upload a Word document
curl -X POST "https://localhost:7191/api/v1/ai/extract-from-document" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -F "file=@project-plan.docx"

# Upload an Outlook email message
curl -X POST "https://localhost:7191/api/v1/ai/extract-from-document" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -F "file=@task-email.msg"
```

**Use Cases for .msg Files:**
- 📧 Extract action items from project emails
- 📅 Identify deadlines mentioned in email threads
- 👥 Capture team member assignments from email distribution
- 📎 Track referenced attachments and deliverables
- 🔄 Convert email task requests into trackable tasks

### AI Setup Guide

#### Using Azure OpenAI

1. **Create Azure OpenAI Resource:**
   - Go to [Azure Portal](https://portal.azure.com/)
   - Create "Azure OpenAI" resource
   - Deploy a model (e.g., gpt-4o-mini)
   - Get endpoint and API key from "Keys and Endpoint"

2. **Configure in `appsettings.json`:**
   ```json
   {
     "AiSettings": {
       "ServiceType": "AzureOpenAI",
       "ApiKey": "your-azure-key",
       "Endpoint": "https://your-resource.openai.azure.com/",
       "DeploymentName": "gpt-4o-mini",
       "ModelId": "gpt-4o-mini",
       "MaxTokens": 2000,
       "Temperature": 0.7
     }
   }
   ```

3. **Find Your Deployment Name:**
   - In Azure Portal, go to your OpenAI resource
   - Click "Model deployments" or "Go to Azure OpenAI Studio"
   - Copy the "Deployment name" (not the model name)

#### Using Google Gemini

1. **Get Google AI API Key:**
   - Go to [Google AI Studio](https://makersuite.google.com/app/apikey)
   - Sign in with your Google account
   - Click "Create API Key"
   - Copy your API key

2. **Configure in `appsettings.json`:**
   ```json
   {
     "AiSettings": {
       "ServiceType": "GoogleGemini",
       "ApiKey": "your-google-gemini-api-key",
       "ModelId": "gemini-2.0-flash-exp",
       "MaxTokens": 2000,
       "Temperature": 0.7
     }
   }
   ```

3. **Available Models:**
   - `gemini-2.0-flash-exp` - Latest experimental model (recommended)
   - `gemini-1.5-pro` - Advanced reasoning and code generation
   - `gemini-3-flash-preview` - Fast and efficient
   - `gemini-pro` - Balanced performance

#### Switching Between AI Providers

The application supports seamless switching between Azure OpenAI and Google Gemini. Simply change the `ServiceType` in your configuration:

**For Azure OpenAI:**
```json
{
  "AiSettings": {
    "ServiceType": "AzureOpenAI",
    "ApiKey": "your-azure-key",
    "Endpoint": "https://your-resource.openai.azure.com/",
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
    "ApiKey": "your-google-gemini-api-key",
    "ModelId": "gemini-2.0-flash-exp",
    "MaxTokens": 2000,
    "Temperature": 0.7
  }
}
```

**Note:** When using Google Gemini, the `Endpoint` and `DeploymentName` fields are not required. When using Azure OpenAI, the `ModelId` is optional but recommended.

Both providers support the same features:
- ✅ AI-powered task summaries
- ✅ Document extraction
- ✅ Task insights and recommendations
- ✅ Confidence scoring

---

## 📁 Project Structure

```
TaskList.Api/
├── TaskList.Api/                      # Presentation Layer
│   ├── Controllers/
│   │   ├── Common/
│   │   │   └── BaseApiController.cs  # Base controller with helpers
│   │   └── V1/
│   │       ├── AuthController.cs     # Authentication endpoints
│   │       ├── TasksController.cs    # Task management endpoints
│   │       └── AiController.cs       # AI feature endpoints
│   ├── Extensions/
│   │   └── ClaimsPrincipalExtensions.cs
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Configuration/
│   │   └── DependencyInjection.cs
│   ├── Program.cs                    # Application entry point
│   ├── appsettings.json             # Production settings
│   ├── appsettings.Development.json # Development settings
│   └── TaskList.Api-*.http          # HTTP test files
│
├── TaskList.Application/             # Application Layer
│   ├── Common/
│   │   ├── JwtSettings.cs           # JWT configuration
│   │   ├── AiSettings.cs            # AI configuration
│   │   └── PagedResult.cs           # Pagination helper
│   ├── DTOs/
│   │   ├── Auth/                    # Authentication DTOs
│   │   ├── Tasks/                   # Task DTOs
│   │   └── AI/                      # AI feature DTOs
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── ITaskService.cs
│   │   ├── IAiService.cs
│   │   ├── ITaskRepository.cs
│   │   └── IJwtTokenService.cs
│   └── Validators/                  # FluentValidation validators
│       ├── CreateTaskRequestValidator.cs
│       └── LoginRequestValidator.cs
│
├── TaskList.Infrastructure/          # Infrastructure Layer
│   ├── Configuration/
│   │   └── DependencyInjection.cs   # Service registration
│   ├── Identity/
│   │   └── ApplicationUser.cs       # Identity user model
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs  # EF DbContext
│   │   ├── DatabaseInitializer.cs   # DB initialization & seeding
│   │   └── Repositories/
│   │       └── TaskRepository.cs    # Task data access
│   └── Services/
│       ├── AuthService.cs           # Authentication logic
│       ├── JwtTokenService.cs       # JWT token generation
│       ├── TaskService.cs           # Task business logic
│       ├── AiSummaryService.cs      # AI features
│       └── DocumentParserService.cs # Document processing
│
├── TaskList.Domain/                  # Domain Layer
│   ├── Entities/
│   │   └── TaskItem.cs              # Task entity
│   └── Enums/
│       ├── TaskItemStatus.cs        # Task status enum
│       ├── PriorityLevel.cs         # Priority enum
│       └── CategoryType.cs          # Category enum
```

### Layer Responsibilities

**1. TaskList.Api (Presentation)**
- HTTP request/response handling
- Routing and API versioning
- Authentication/authorization
- Input validation
- API documentation

**2. TaskList.Application (Application)**
- Business logic interfaces
- DTOs (Data Transfer Objects)
- Validation rules
- Application services contracts

**3. TaskList.Infrastructure (Infrastructure)**
- Service implementations
- Database context and migrations
- External service integrations (AI, email, etc.)
- Data access (repositories)

**4. TaskList.Domain (Domain)**
- Core business entities
- Domain logic
- Enums and value objects
- Domain events (if any)

---

## ⚙️ Detailed Configuration

### Application Settings

**`appsettings.json`** (Production):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=TaskListDb;..."
  },
  "JwtSettings": {
    "Secret": "production-secret-key-min-32-chars",
    "Issuer": "TaskListApi",
    "Audience": "TaskListClient",
    "ExpirationInMinutes": 30,
    "RefreshTokenExpirationInDays": 7
  },
  "AiSettings": {
    "ServiceType": "AzureOpenAI",
    "ApiKey": "",
    "Endpoint": "https://your-resource.openai.azure.com/",
    "DeploymentName": "gpt-4o",
    "MaxTokens": 2000,
    "Temperature": 0.7
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  },
  "AllowedHosts": "*"
}
```

**`appsettings.Development.json`**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskListDb;Trusted_Connection=True;"
  },
  "AiSettings": {
    "ServiceType": "OpenAI",
    "ApiKey": "sk-your-dev-api-key",
    "ModelId": "gpt-4o-mini"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

### Environment Variables

For production, use environment variables:

```bash
# Connection String
ConnectionStrings__DefaultConnection="Server=..."

# JWT Secret
JwtSettings__Secret="production-secret"

# AI Configuration
AiSettings__ApiKey="your-api-key"
AiSettings__ServiceType="AzureOpenAI"
AiSettings__Endpoint="https://..."
AiSettings__DeploymentName="gpt-4o"
```

### CORS Configuration

Already configured in `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Pagination");
    });
});
```

For production, restrict origins:
```csharp
policy.WithOrigins("https://yourdomain.com")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .WithExposedHeaders("X-Pagination");
```

---

## 🧪 Testing

### HTTP Test Files

The project includes HTTP test files for manual testing:

1. **`TaskList.Api-Auth.http`**: Authentication tests
2. **`TaskList.Api-Tasks.http`**: Task management tests
3. **`TaskList.Api-AI.http`**: AI feature tests

**Usage:**
- Open in Visual Studio or VS Code with REST Client extension
- Update `@accessToken` variable after login
- Run requests directly from the file

### Testing Workflow

1. **Register/Login**
   ```http
   POST https://localhost:7191/api/v1/auth/login
   ```

2. **Copy Access Token**
   ```
   Update @accessToken in HTTP file
   ```

3. **Test Endpoints**
   ```http
   GET https://localhost:7191/api/v1/tasks
   Authorization: Bearer {accessToken}
   ```

### Unit Testing

*Note: Unit tests are not yet implemented but planned for:*
- Service layer logic
- Repository operations
- Validation rules
- AI prompt generation
- Document parsing

---

## ⚠️ Known Limitations

### AI Features

1. **Token Limits**
   - Maximum document size: ~10 MB
   - Maximum tokens per request: 2000 (configurable)
   - Very large documents may be truncated

2. **Accuracy**
   - AI extraction confidence varies (typically 75-95%)
   - Date parsing may fail for ambiguous formats
   - Non-English documents have lower accuracy

3. **Rate Limits**
   - OpenAI: 3 requests/minute (free tier)
   - Azure OpenAI: Depends on quota allocation
   - Consider implementing caching for summaries

4. **Cost Considerations**
   - AI calls incur per-token charges
   - Monitor usage in OpenAI dashboard
   - Consider implementing usage limits per user

### Document Processing

1. **Supported Formats**
- Only PDF, DOCX, TXT, and MSG (Outlook email) files
- Scanned PDFs (images) not supported (would need OCR)
- Excel/CSV files not supported

2. **File Size Limits**
   - Maximum upload size: 10 MB
   - Large files increase processing time
   - Consider implementing file size warnings

### General Limitations

1. **Database**
   - Single database instance (no sharding)
   - No support for distributed transactions

2. **Authentication**
   - Single device logout not supported
   - No two-factor authentication (2FA)
   - No social login (Google, Facebook)

3. **Features**
   - No real-time notifications
   - No task sharing between users
   - No task attachments
   - No recurring tasks

---

## 🔧 Troubleshooting

### Common Issues

#### 1. Database Connection Failed

**Error:** `Cannot connect to SQL Server`

**Solutions:**
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure firewall allows connections
- For SQL Server Authentication, verify credentials

#### 2. AI Service Unavailable

**Error:** `Specified method is not supported` or `Deployment not found`

**Solutions:**
- Verify API key is correct
- Create Model in Microsft Foundry (new Azure AI Studio)
- Check deployment name matches exactly
- Ensure `ServiceType` is correctly set "AzureOpenAI"
- Check Azure OpenAI deployment status in Azure Portal

#### 3. JWT Token Issues

**Error:** `Unauthorized` or `Invalid token`

**Solutions:**
- Check token expiration (30 minutes default)
- Use refresh token to get new access token
- Verify JWT secret matches between requests
- Clear browser cache/cookies

#### 4. Document Upload Fails

**Error:** `Unsupported file type` or `File too large`

**Solutions:**
- Only PDF, DOCX, TXT, and MSG (Outlook email) files supported
- Maximum file size is 10 MB
- Check file is not corrupted
- Verify Content-Type header
- For .msg files, ensure they are valid Outlook message files

#### 5. CORS Errors

**Error:** `CORS policy blocked`

**Solutions:**
- Verify CORS is configured in Program.cs
- Check frontend origin is allowed
- Ensure `app.UseCors()` is before `app.UseAuthorization()`

### Debug Mode

Run with detailed logging:
```bash
dotnet run --project TaskList.Api --environment Development
```

Check logs in:
- Console output
- `logs/` folder (if file sink configured)

---

## 🎯 Roadmap

### Planned Features
- [ ] Real-time notifications with SignalR
- [ ] Task attachments (files, images)
- [ ] Recurring tasks
- [ ] Task sharing between users
- [ ] Two-factor authentication (2FA)
- [ ] Social login (Google, Microsoft)
- [ ] Mobile app (iOS/Android)
- [ ] Task templates
- [ ] Advanced analytics dashboard
- [ ] Voice input for task creation
- [ ] Smart task recommendations
