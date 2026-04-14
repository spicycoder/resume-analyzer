namespace ResumeAnalyzer.Application.Abstractions;

public interface IPdfTextExtractor
{
    string ExtractText(Stream pdfStream);
}
