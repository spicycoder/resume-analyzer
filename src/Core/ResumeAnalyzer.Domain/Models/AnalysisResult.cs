namespace ResumeAnalyzer.Domain.Models;

public record AnalysisResult(
    int MatchPercentage,
    IReadOnlyList<Flag> GreenFlags,
    IReadOnlyList<Flag> RedFlags);
