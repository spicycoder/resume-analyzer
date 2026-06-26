# Resume Analyzer

AI-driven resume analysis against job descriptions.

> The public instance is for demo purposes only. Self-host with your own OpenAI-compatible API key for production use.

| Build |
| --- |
| [![CI](https://github.com/spicycoder/resume-analyzer/actions/workflows/ci.yml/badge.svg)](https://github.com/spicycoder/resume-analyzer/actions/workflows/ci.yml) |

---

## Coverage

| Total Coverage % | Unit Tests # | Integration Tests # |
| --- | --- | --- |
| [![Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/spicycoder/66489e99c3b00af77142df8d681d62b3/raw/coverage.json)](https://github.com/spicycoder/resume-analyzer/actions) | ![Unit Tests](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/spicycoder/66489e99c3b00af77142df8d681d62b3/raw/unit.json) | ![Integration Tests](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/spicycoder/66489e99c3b00af77142df8d681d62b3/raw/integration.json) |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (22.x) + [pnpm](https://pnpm.io/installation)
- [Docker](https://www.docker.com/) or [Podman](https://podman.io/) (for integration tests). If using Podman, enable Docker compatibility.

## Getting Started

```bash
git clone https://github.com/spicycoder/resume-analyzer.git
cd resume-analyzer
./build.ps1
```

## Acknowledgements

Thanks to [Render](https://render.com) for providing free tiers that make side projects like this possible.

## Docs

- [Architecture & Design](docs/design.md)
- [API Endpoints](docs/endpoints.md)
- [Testing](docs/tests.md)
