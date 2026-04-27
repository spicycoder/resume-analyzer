using System.Text;
using Shouldly;
using ResumeAnalyzer.Application.UseCases.Queries;
using ResumeAnalyzer.Infrastructure.Pdf;
using Xunit;

namespace ResumeAnalyzer.Infrastructure.UnitTests;

public class PdfTextExtractorTests
{
    private readonly PdfTextExtractor _extractor = new();

    [Fact]
    public void ExtractText_CorruptPdf_ThrowsPdfParseException()
    {
        using var stream = new MemoryStream(new byte[] { 0x00, 0x01, 0x02 });

        var ex = Should.Throw<PdfParseException>(() => _extractor.ExtractText(stream));

        ex.Message.ShouldNotBeNull();
    }

    [Fact]
    public void ExtractText_EmptyStream_ThrowsPdfParseException()
    {
        using var stream = new MemoryStream();

        var ex = Should.Throw<PdfParseException>(() => _extractor.ExtractText(stream));

        ex.Message.ShouldNotBeNull();
    }

    [Fact]
    public void ExtractText_InvalidPdfHeader_ThrowsPdfParseException()
    {
        using var stream = new MemoryStream(Encoding.ASCII.GetBytes("not a pdf"));

        var ex = Should.Throw<PdfParseException>(() => _extractor.ExtractText(stream));

        ex.Message.ShouldNotBeNull();
    }

    [Fact]
    public void ExtractText_GarbageBytes_ThrowsPdfParseException()
    {
        using var stream = new MemoryStream(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x20, 0xFF });

        var ex = Should.Throw<PdfParseException>(() => _extractor.ExtractText(stream));

        ex.Message.ShouldNotBeNull();
    }

    [Fact]
    public void ExtractText_ValidPdfWithNullText_HandlesGracefully()
    {
        // Test the catch (System.Exception) path - generic exception handling
        // Pass a stream that triggers a non-specific exception
        using var stream = new MemoryStream(Encoding.ASCII.GetBytes("%PDF-1.3"));

        var ex = Should.Throw<PdfParseException>(() => _extractor.ExtractText(stream));

        ex.Message.ShouldNotBeNull();
    }
}