using FluentValidation;

namespace ResumeAnalyzer.Application.UseCases.Queries;

public class AnalyzeResumeQueryValidator : AbstractValidator<AnalyzeResumeQuery>
{
    public AnalyzeResumeQueryValidator()
    {
        RuleFor(x => x.JdStream)
            .NotNull()
            .Must(x => x != null && x.Length > 0).WithMessage("JD stream cannot be empty.");

        RuleFor(x => x.JdFileName)
            .NotEmpty()
            .Must(x => x.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("JD must be a PDF file.");

        RuleFor(x => x.JdSize)
            .InclusiveBetween(1, 10 * 1024 * 1024)
            .WithMessage("JD file size must be between 1 byte and 10MB.");

        RuleFor(x => x.ResumeStream)
            .NotNull()
            .Must(x => x != null && x.Length > 0).WithMessage("Resume stream cannot be empty.");

        RuleFor(x => x.ResumeFileName)
            .NotEmpty()
            .Must(x => x.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Resume must be a PDF file.");

        RuleFor(x => x.ResumeSize)
            .InclusiveBetween(1, 10 * 1024 * 1024)
            .WithMessage("Resume file size must be between 1 byte and 10MB.");
    }
}
