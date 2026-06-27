namespace ResumeAnalyzer.Domain.Abstractions;

public interface IPdfTextExtractor
{
    string ExtractText(Stream pdfStream);
}
