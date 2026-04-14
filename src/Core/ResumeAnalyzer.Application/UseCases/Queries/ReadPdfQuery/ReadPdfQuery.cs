namespace ResumeAnalyzer.Application.UseCases.Queries;

public record ReadPdfQuery(Stream PdfStream, string FileName, long Size);
