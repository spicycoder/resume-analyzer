namespace ResumeAnalyzer.Application.UseCases.Queries;

public class PdfParseException : Exception
{
    public PdfParseException()
    {
    }

    public PdfParseException(string message) : base(message)
    {
    }

    public PdfParseException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
