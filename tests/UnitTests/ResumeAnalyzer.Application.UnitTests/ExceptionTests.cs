using Shouldly;
using ResumeAnalyzer.Application.UseCases.Queries;
using Xunit;

namespace ResumeAnalyzer.Application.UnitTests;

public class PdfParseExceptionTests
{
    [Fact]
    public void Create_DefaultConstructor_HasMessage()
    {
        var ex = new PdfParseException();

        ex.Message.ShouldNotBeNull();
        ex.Message.ShouldContain("PdfParseException");
    }

    [Fact]
    public void Create_WithMessage_MessageSet()
    {
        var ex = new PdfParseException("PDF parsing failed");

        ex.Message.ShouldBe("PDF parsing failed");
    }

    [Fact]
    public void Create_WithMessageAndInnerException_InnerExceptionSet()
    {
        var inner = new InvalidOperationException("Inner");
        var ex = new PdfParseException("PDF parsing failed", inner);

        ex.Message.ShouldBe("PDF parsing failed");
        ex.InnerException.ShouldBe(inner);
    }
}

public class PdfEncryptedExceptionTests
{
    [Fact]
    public void Create_DefaultConstructor_HasMessage()
    {
        var ex = new PdfEncryptedException();

        ex.Message.ShouldNotBeNull();
        ex.Message.ShouldContain("PdfEncryptedException");
    }

    [Fact]
    public void Create_WithMessage_MessageSet()
    {
        var ex = new PdfEncryptedException("PDF is encrypted");

        ex.Message.ShouldBe("PDF is encrypted");
    }

    [Fact]
    public void Create_InheritsFromPdfParseException()
    {
        var ex = new PdfEncryptedException();

        ex.ShouldBeAssignableTo<PdfParseException>();
    }

    [Fact]
    public void Create_WithMessageAndInnerException_InnerExceptionSet()
    {
        var inner = new InvalidOperationException("Inner");
        var ex = new PdfEncryptedException("PDF is encrypted", inner);

        ex.Message.ShouldBe("PDF is encrypted");
        ex.InnerException.ShouldBe(inner);
    }
}