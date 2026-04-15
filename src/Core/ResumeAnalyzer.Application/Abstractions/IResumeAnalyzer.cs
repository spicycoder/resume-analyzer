using ResumeAnalyzer.Domain.Models;

namespace ResumeAnalyzer.Application.Abstractions;

public interface IResumeAnalyzer
{
    Task<AnalysisResult> AnalyzeAsync(string resumeText, string jdText, CancellationToken cancellationToken);
}
