namespace ResumeAnalyzer.Domain.Exceptions;

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
