# Resume Analyzer

| Build | Coverage |
| --- | --- |
| [![CI Build](https://github.com/spicycoder/resume-analyzer/actions/workflows/build.yml/badge.svg)](https://github.com/spicycoder/resume-analyzer/actions/workflows/build.yml) | [![Coverage](https://img.shields.io/badge/coverage-94.9%25-brightgreen)](https://github.com/spicycoder/resume-analyzer) |

---

## High-Level Architecture

```mermaid
graph TD
    Client[Client] -- "HTTP POST /api/analyze<br/>(PDF JD + PDF Resume)" --> API[ResumeAnalyzer.Api]
    
    subgraph "Core"
        API -- AnalyzeResumeQuery --> Bus[Wolverine Bus]
        Bus -- Query Handler --> Application[ResumeAnalyzer.Application]
        Application -- AnalysisResult --> Bus
    end

    subgraph "Infrastructure"
        Application -- IPdfTextExtractor --> PDF[PdfTextExtractor]
        Application -- IResumeAnalyzer --> LLM[OpenAiResumeAnalyzer]
        
        PDF -- Text Content --> Application
        LLM -- Structured Analysis --> Application
    end

    Bus -- AnalysisResult --> API
    API -- 200 OK / JSON --> Client
```

---

## Sequence Diagram

```mermaid
sequenceDiagram
    participant C as Client
    participant API as ResumeAnalyzer.Api
    participant Bus as Wolverine Bus
    participant App as ResumeAnalyzer.Application
    participant PDF as PdfTextExtractor
    participant LLM as OpenAiResumeAnalyzer

    C->>API: POST /api/analyze (JD.pdf, Resume.pdf)
    API->>Bus: InvokeAsync(AnalyzeResumeQuery)
    Bus->>App: Handle(AnalyzeResumeQuery)
    
    par Extract Text
        App->>PDF: ExtractText(JD.pdf)
        PDF-->>App: JD Text
    and
        App->>PDF: ExtractText(Resume.pdf)
        PDF-->>App: Resume Text
    end
    
    App->>LLM: Analyze(JD Text, Resume Text)
    LLM-->>App: AnalysisResult
    
    App-->>Bus: AnalysisResult
    Bus-->>API: AnalysisResult
    API-->>C: 200 OK (AnalysisResult JSON)
```

---

## Component Overview

| Project | Description |
| :--- | :--- |
| **ResumeAnalyzer.Api** | Entry point. Validates and accepts multipart/form-data containing two PDF files: the Job Description (JD) and the Resume. Dispatches queries via Wolverine. |
| **ResumeAnalyzer.Application** | Core business logic. Coordinates PDF extraction for both files and performs LLM analysis comparing them. |
| **ResumeAnalyzer.Infrastructure** | Concrete implementations for external services (PDF processing, OpenAI integration). |
| **ResumeAnalyzer.Domain** | Shared models and domain entities. |
| **ResumeAnalyzer.AppHost** | .NET Aspire orchestration. |
