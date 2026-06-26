# Tests

- [x] Maximum coverage with Unit tests
- [x] Just enough Integration Tests
- [x] Architecture enforcement tests

## Projects

| Project | Scope | What it tests | Speed |
|---|---|---|---|
| `ResumeAnalyzer.Api.UnitTests` | Unit | `AnalyzeController` (all status code branches), request validation, error handling | Fast |
| `ResumeAnalyzer.Application.UnitTests` | Unit | Command/query handlers, validators, business logic | Fast |
| `ResumeAnalyzer.Infrastructure.UnitTests` | Unit | PDF extraction, LLM client, external service integrations | Fast |
| `ResumeAnalyzer.ArchitectureTests` | Architecture | Layer rules, dependency validation, naming conventions | Fast |
| `ResumeAnalyzer.Api.IntegrationTests` | Integration | Full HTTP pipeline via Aspire: controller -> handler -> PDF extraction -> LLM -> response | Slow |
