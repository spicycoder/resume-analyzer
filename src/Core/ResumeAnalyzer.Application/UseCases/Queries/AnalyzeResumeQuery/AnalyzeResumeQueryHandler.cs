using ResumeAnalyzer.Application.Abstractions;
using ResumeAnalyzer.Domain.Models;

namespace ResumeAnalyzer.Application.UseCases.Queries;

public class AnalyzeResumeQueryHandler(
    IPdfTextExtractor pdfTextExtractor,
    IResumeAnalyzer resumeAnalyzer)
{
    public async Task<AnalysisResult> Handle(AnalyzeResumeQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var jdText = pdfTextExtractor.ExtractText(query.JdStream);
        var resumeText = pdfTextExtractor.ExtractText(query.ResumeStream);

        return await resumeAnalyzer.AnalyzeAsync(resumeText, jdText, cancellationToken).ConfigureAwait(false);
    }
}
