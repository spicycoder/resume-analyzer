var builder = DistributedApplication.CreateBuilder(args);

var aiApiKey = builder.AddParameter("AiApiKey", secret: true);

builder.AddProject<Projects.ResumeAnalyzer_Api>("resumeanalyzer-api")
    .WithEnvironment("Ai__ApiKey", aiApiKey);

builder.Build().Run();