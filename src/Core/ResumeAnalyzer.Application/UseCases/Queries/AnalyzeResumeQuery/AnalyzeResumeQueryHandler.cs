using ResumeAnalyzer.Domain.Abstractions;
using ResumeAnalyzer.Domain.Models;

namespace ResumeAnalyzer.Application.UseCases.Queries;

public class AnalyzeResumeQueryHandler(
    IPdfTextExtractor pdfTextExtractor,
    IResumeAnalyzer resumeAnalyzer)
{
    public async Task<AnalysisResult> Handle(AnalyzeResumeQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var jdTask = Task.Run(() => pdfTextExtractor.ExtractText(query.JdStream), cancellationToken);
        var resumeTask = Task.Run(() => pdfTextExtractor.ExtractText(query.ResumeStream), cancellationToken);

        await Task.WhenAll(jdTask, resumeTask).ConfigureAwait(false);

        return await resumeAnalyzer.AnalyzeAsync(
            await resumeTask.ConfigureAwait(false), 
            await jdTask.ConfigureAwait(false), 
            cancellationToken).ConfigureAwait(false);
    }
}
