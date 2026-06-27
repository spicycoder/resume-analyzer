using System.Text;

using ResumeAnalyzer.Domain.Abstractions;
using ResumeAnalyzer.Domain.Exceptions;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Exceptions;

namespace ResumeAnalyzer.Infrastructure.Pdf;

public class PdfTextExtractor : IPdfTextExtractor
{
    public string ExtractText(Stream pdfStream)
    {
        try
        {
            using var document = PdfDocument.Open(pdfStream);
            StringBuilder textBuilder = new();

            foreach (var page in document.GetPages())
            {
                textBuilder.AppendLine(page.Text);
            }

            return textBuilder.ToString();
        }
        catch (PdfDocumentEncryptedException)
        {
            throw new PdfEncryptedException("The PDF file is password protected and cannot be read.");
        }
        catch (PdfDocumentFormatException ex)
        {
            throw new PdfParseException("The file is not a valid PDF or is corrupted.", ex);
        }
        catch (System.Exception ex)
        {
            throw new PdfParseException("An unexpected error occurred while reading the PDF.", ex);
        }
    }
}