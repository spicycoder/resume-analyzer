var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ResumeAnalyzer_Api>("resumeanalyzer-api");

builder.Build().Run();
