namespace ResumeAnalyzer.Application.UseCases.Queries;

public class RateLimitExceededException : Exception
{
    public RateLimitExceededException()
    {
    }

    public RateLimitExceededException(string message) : base(message)
    {
    }

    public RateLimitExceededException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
