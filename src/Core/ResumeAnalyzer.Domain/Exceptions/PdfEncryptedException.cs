namespace ResumeAnalyzer.Domain.Exceptions;

public class PdfEncryptedException : PdfParseException
{
    public PdfEncryptedException()
    {
    }

    public PdfEncryptedException(string message) : base(message)
    {
    }

    public PdfEncryptedException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
