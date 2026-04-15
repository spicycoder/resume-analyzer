namespace ResumeAnalyzer.Application.UseCases.Queries;

public record AnalyzeResumeQuery(
    Stream JdStream, string JdFileName, long JdSize,
    Stream ResumeStream, string ResumeFileName, long ResumeSize);
