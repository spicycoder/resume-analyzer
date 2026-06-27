using FluentValidation;
using Shouldly;
using ResumeAnalyzer.Application.UseCases.Queries;
using ResumeAnalyzer.Domain.Models;
using Xunit;

namespace ResumeAnalyzer.Application.UnitTests;

public class AnalyzeResumeQueryValidatorTests
{
    private readonly AnalyzeResumeQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_NoErrors()
    {
        using var jdStream = new MemoryStream(new byte[100]);
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 100,
            resumeStream, "resume.pdf", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Validate_JdStreamNull_Fails()
    {
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            null!, "jd.pdf", 100,
            resumeStream, "resume.pdf", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "JdStream");
        result.Errors.Count(e => e.PropertyName == "JdStream").ShouldBe(2);
    }

    [Fact]
    public void Validate_JdStreamEmpty_Fails()
    {
        using var jdStream = new MemoryStream();
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 0,
            resumeStream, "resume.pdf", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "JdStream");
    }

    [Fact]
    public void Validate_JdFileNameEmpty_Fails()
    {
        using var jdStream = new MemoryStream(new byte[100]);
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, "", 100,
            resumeStream, "resume.pdf", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "JdFileName");
    }

    [Fact]
    public void Validate_JdFileNameNotPdf_Fails()
    {
        using var jdStream = new MemoryStream(new byte[100]);
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.docx", 100,
            resumeStream, "resume.pdf", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "JdFileName");
    }

    [Theory]
    [InlineData("jd.pdf")]
    [InlineData("JD.PDF")]
    [InlineData("jd.PdF")]
    public void Validate_JdFileNamePdfCaseInsensitive_Passes(string fileName)
    {
        using var jdStream = new MemoryStream(new byte[100]);
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, fileName, 100,
            resumeStream, "resume.pdf", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_JdSizeTooSmall_Fails(int size)
    {
        using var jdStream = new MemoryStream(new byte[100]);
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", size,
            resumeStream, "resume.pdf", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "JdSize");
    }

    [Fact]
    public void Validate_JdSizeTooLarge_Fails()
    {
        using var jdStream = new MemoryStream(new byte[100]);
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 11 * 1024 * 1024,
            resumeStream, "resume.pdf", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "JdSize");
    }

    [Fact]
    public void Validate_ResumeStreamNull_Fails()
    {
        using var jdStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 100,
            null!, "resume.pdf", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ResumeStream");
    }

    [Fact]
    public void Validate_ResumeStreamEmpty_Fails()
    {
        using var jdStream = new MemoryStream(new byte[100]);
        using var resumeStream = new MemoryStream();

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 100,
            resumeStream, "resume.pdf", 0);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ResumeStream");
    }

    [Fact]
    public void Validate_ResumeFileNameEmpty_Fails()
    {
        using var jdStream = new MemoryStream(new byte[100]);
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 100,
            resumeStream, "", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ResumeFileName");
    }

    [Fact]
    public void Validate_ResumeFileNameNotPdf_Fails()
    {
        using var jdStream = new MemoryStream(new byte[100]);
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 100,
            resumeStream, "resume.doc", 100);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ResumeFileName");
    }

    [Fact]
    public void Validate_ResumeSizeTooLarge_Fails()
    {
        using var jdStream = new MemoryStream(new byte[100]);
        using var resumeStream = new MemoryStream(new byte[100]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 100,
            resumeStream, "resume.pdf", 11 * 1024 * 1024);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ResumeSize");
    }

    [Fact]
    public void Validate_MultipleErrors_AllReported()
    {
        var query = new AnalyzeResumeQuery(
            null!, "", 0,
            null!, "", 0);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(10);
    }

    [Fact]
    public void Validate_MaxSizeBoundary_Passes()
    {
        using var jdStream = new MemoryStream(new byte[10 * 1024 * 1024]);
        using var resumeStream = new MemoryStream(new byte[10 * 1024 * 1024]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 10 * 1024 * 1024,
            resumeStream, "resume.pdf", 10 * 1024 * 1024);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_MinSizeBoundary_Passes()
    {
        using var jdStream = new MemoryStream(new byte[1]);
        using var resumeStream = new MemoryStream(new byte[1]);

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 1,
            resumeStream, "resume.pdf", 1);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeTrue();
    }
}