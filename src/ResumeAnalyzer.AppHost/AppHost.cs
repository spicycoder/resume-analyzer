var builder = DistributedApplication.CreateBuilder(args);

var aiApiKey = builder.AddParameter("AiApiKey", secret: true);

var api = builder.AddProject<Projects.ResumeAnalyzer_Api>("resumeanalyzer-api")
    .WithEnvironment("Ai__ApiKey", aiApiKey);

builder.AddViteApp("web", "../../web")
    .WithPnpm()
    .WaitFor(api)
    .WithReference(api);

builder.Build().Run();