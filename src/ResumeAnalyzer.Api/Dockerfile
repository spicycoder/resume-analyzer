FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY Directory.Build.props Directory.Packages.props nuget.config ./
COPY src/ResumeAnalyzer.Api/ResumeAnalyzer.Api.csproj src/ResumeAnalyzer.Api/
COPY src/Core/ResumeAnalyzer.Application/ResumeAnalyzer.Application.csproj src/Core/ResumeAnalyzer.Application/
COPY src/Core/ResumeAnalyzer.Domain/ResumeAnalyzer.Domain.csproj src/Core/ResumeAnalyzer.Domain/
COPY src/Infrastructure/ResumeAnalyzer.Infrastructure/ResumeAnalyzer.Infrastructure.csproj src/Infrastructure/ResumeAnalyzer.Infrastructure/
COPY src/ResumeAnalyzer.ServiceDefaults/ResumeAnalyzer.ServiceDefaults.csproj src/ResumeAnalyzer.ServiceDefaults/
RUN dotnet restore src/ResumeAnalyzer.Api/ResumeAnalyzer.Api.csproj

COPY . .
RUN dotnet publish src/ResumeAnalyzer.Api/ResumeAnalyzer.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime

WORKDIR /app
EXPOSE 8080

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget -qO- http://localhost:8080/health || exit 1

CMD ASPNETCORE_URLS="http://0.0.0.0:${PORT:-8080}" exec dotnet ResumeAnalyzer.Api.dll
