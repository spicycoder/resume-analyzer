using ResumeAnalyzer.Application.Abstractions;

namespace ResumeAnalyzer.Application.UseCases.Queries;

public class ReadPdfQueryHandler(IPdfTextExtractor pdfTextExtractor)
{
    public Task<string> Handle(ReadPdfQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var text = pdfTextExtractor.ExtractText(query.PdfStream);
        return Task.FromResult(text);
    }
}
