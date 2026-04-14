using FluentValidation;

namespace ResumeAnalyzer.Application.UseCases.Queries;

public class ReadPdfQueryValidator : AbstractValidator<ReadPdfQuery>
{
    public ReadPdfQueryValidator()
    {
        RuleFor(x => x.PdfStream)
            .NotNull()
            .Must(x => x.Length > 0).WithMessage("PDF stream cannot be empty.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(x => x.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only PDF files are supported.");

        RuleFor(x => x.Size)
            .InclusiveBetween(1, 10 * 1024 * 1024)
            .WithMessage("File size must be between 1 byte and 10MB.");
    }
}
