using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using ResumeAnalyzer.Api.Controllers;
using ResumeAnalyzer.Application.UseCases.Queries;
using ResumeAnalyzer.Domain.Models;
using Wolverine;
using Xunit;

namespace ResumeAnalyzer.Api.UnitTests;

public class AnalyzeControllerTests
{
    private readonly Mock<IMessageBus> _busMock;
    private readonly AnalyzeController _controller;

    public AnalyzeControllerTests()
    {
        _busMock = new Mock<IMessageBus>();
        _controller = new AnalyzeController(_busMock.Object);
    }

    [Fact]
    public async Task Analyze_JdNull_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(() => 
            _controller.Analyze(null!, CreateFakeFile("resume.pdf"), CancellationToken.None));
    }

    [Fact]
    public async Task Analyze_ResumeNull_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(() => 
            _controller.Analyze(CreateFakeFile("jd.pdf"), null!, CancellationToken.None));
    }

    [Fact]
    public async Task Analyze_ValidRequest_ReturnsOk()
    {
        var expectedResult = new AnalysisResult(75, new List<Flag>(), new List<Flag>());
        _busMock.Setup(b => b.InvokeAsync<AnalysisResult>(It.IsAny<AnalyzeResumeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        var okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task Analyze_ValidationError_ReturnsBadRequest()
    {
        _busMock.Setup(b => b.InvokeAsync<AnalysisResult>(It.IsAny<AnalyzeResumeQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FluentValidation.ValidationException("Invalid"));

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Analyze_PdfEncrypted_ReturnsUnprocessableEntity()
    {
        _busMock.Setup(b => b.InvokeAsync<AnalysisResult>(It.IsAny<AnalyzeResumeQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PdfEncryptedException("PDF is encrypted"));

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        result.ShouldBeOfType<UnprocessableEntityObjectResult>();
    }

    [Fact]
    public async Task Analyze_PdfParseError_ReturnsUnprocessableEntity()
    {
        _busMock.Setup(b => b.InvokeAsync<AnalysisResult>(It.IsAny<AnalyzeResumeQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PdfParseException("Parse failed"));

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        result.ShouldBeOfType<UnprocessableEntityObjectResult>();
    }

    [Fact]
    public async Task Analyze_CancellationRequested_Returns499()
    {
        using var cts = new CancellationTokenSource();
        _busMock.Setup(b => b.InvokeAsync<AnalysisResult>(It.IsAny<AnalyzeResumeQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), cts.Token);

        result.ShouldBeOfType<StatusCodeResult>();
        ((StatusCodeResult)result).StatusCode.ShouldBe(499);
    }

    private static IFormFile CreateFakeFile(string fileName)
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.FileName).Returns(fileName);
        mock.Setup(f => f.Length).Returns(100);
        mock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[100]));
        return mock.Object;
    }
}