using System.Security.Cryptography;
using FluentValidation;
using Shouldly;
using ResumeAnalyzer.Application.Abstractions;
using ResumeAnalyzer.Application.UseCases.Queries;
using ResumeAnalyzer.Domain.Models;
using Xunit;

namespace ResumeAnalyzer.Application.UnitTests;

public class AnalyzeResumeQueryTests
{
    [Fact]
    public void Create_WithValidParams_PropertiesSetCorrectly()
    {
        var jdStream = new MemoryStream();
        var resumeStream = new MemoryStream();

        var query = new AnalyzeResumeQuery(
            jdStream, "jd.pdf", 1024,
            resumeStream, "resume.pdf", 2048);

        query.JdStream.ShouldBe(jdStream);
        query.JdFileName.ShouldBe("jd.pdf");
        query.JdSize.ShouldBe(1024);
        query.ResumeStream.ShouldBe(resumeStream);
        query.ResumeFileName.ShouldBe("resume.pdf");
        query.ResumeSize.ShouldBe(2048);
    }

    [Fact]
    public void Create_WithDifferentFileNames_Supported()
    {
        using var jdStream = new MemoryStream();
        using var resumeStream = new MemoryStream();

        var query = new AnalyzeResumeQuery(
            jdStream, "JOB_DESCRIPTION.PDF", 100,
            resumeStream, "My_Resume.Pdf", 200);

        query.JdFileName.ShouldBe("JOB_DESCRIPTION.PDF");
        query.ResumeFileName.ShouldBe("My_Resume.Pdf");
    }
}