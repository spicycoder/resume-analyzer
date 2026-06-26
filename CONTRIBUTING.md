# Contributing

Thanks for your interest in contributing to Resume Analyzer!

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) or [Podman](https://podman.io/) (for Aspire integration tests)
  > If using Podman, enable Docker compatibility.

## Getting Started

```bash
git clone https://github.com/spicycoder/resume-analyzer.git
cd resume-analyzer
./build.ps1
```

`build.ps1` restores tools, builds, runs all tests, and generates coverage reports.

### Architecture

- **Clean Architecture**: Domain → Application → Infrastructure → API
- **WolverineFx**: Mediator for command/query dispatch
- **FluentValidation**: Validation at API and Application layers

### Test Structure

| Project | What | Speed |
|---|---|---|
| `ResumeAnalyzer.Api.UnitTests` | Controller branches, request validators | Fast |
| `ResumeAnalyzer.Application.UnitTests` | Handlers, validators | Fast |
| `ResumeAnalyzer.Infrastructure.UnitTests` | PDF extraction, LLM client | Fast |
| `ResumeAnalyzer.Api.IntegrationTests` | Full HTTP pipeline via Aspire | Slow |

## Pull Requests

1. Fork the repo
2. Create a branch from `main`
3. Make your changes
4. Ensure `dotnet build` passes
5. Ensure `dotnet test` passes
6. Submit a PR

### Commit Messages

Use [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: add new analysis endpoint
fix: handle empty PDF gracefully
test: add coverage for edge cases
docs: update architecture diagram
chore: bump dependencies
```

### Code Style

- Follow existing patterns in the codebase
- No warnings — treat warnings as errors
- Keep controllers thin — business logic in handlers
- Domain layer has zero external dependencies

## Reporting Issues

- Use [GitHub Issues](https://github.com/spicycoder/resume-analyzer/issues)
- Include steps to reproduce
- Include .NET version (`dotnet --version`)

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE).
