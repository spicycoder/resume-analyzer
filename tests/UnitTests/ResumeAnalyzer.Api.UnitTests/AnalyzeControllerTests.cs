using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Shouldly;
using ResumeAnalyzer.Api.Controllers;
using ResumeAnalyzer.Domain.Exceptions;
using ResumeAnalyzer.Domain.Models;
using Wolverine;
using Xunit;

namespace ResumeAnalyzer.Api.UnitTests;

public class AnalyzeControllerTests
{
    private readonly IMessageBus _bus;
    private readonly AnalyzeController _controller;

    public AnalyzeControllerTests()
    {
        _bus = Substitute.For<IMessageBus>();
        _controller = new AnalyzeController(_bus);
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
        _bus.InvokeAsync<AnalysisResult>(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        var okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task Analyze_ValidationError_ReturnsBadRequest()
    {
        _bus.When(b => b.InvokeAsync<AnalysisResult>(Arg.Any<object>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new FluentValidation.ValidationException("Invalid"));

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Analyze_PdfEncrypted_ReturnsUnprocessableEntity()
    {
        _bus.When(b => b.InvokeAsync<AnalysisResult>(Arg.Any<object>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new PdfEncryptedException("PDF is encrypted"));

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        result.ShouldBeOfType<UnprocessableEntityObjectResult>();
    }

    [Fact]
    public async Task Analyze_PdfParseError_ReturnsUnprocessableEntity()
    {
        _bus.When(b => b.InvokeAsync<AnalysisResult>(Arg.Any<object>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new PdfParseException("Parse failed"));

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        result.ShouldBeOfType<UnprocessableEntityObjectResult>();
    }

    [Fact]
    public async Task Analyze_CancellationRequested_Returns499()
    {
        using var cts = new CancellationTokenSource();
        _bus.When(b => b.InvokeAsync<AnalysisResult>(Arg.Any<object>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new OperationCanceledException());

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), cts.Token);

        result.ShouldBeOfType<StatusCodeResult>();
        ((StatusCodeResult)result).StatusCode.ShouldBe(499);
    }

    [Fact]
    public async Task Analyze_RateLimitExceeded_Returns429()
    {
        _bus.When(b => b.InvokeAsync<AnalysisResult>(Arg.Any<object>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new RateLimitExceededException("rate limited"));

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        var statusCodeResult = result.ShouldBeOfType<ObjectResult>();
        statusCodeResult.StatusCode.ShouldBe(429);
    }

    [Fact]
    public async Task Analyze_TimeoutException_Returns504()
    {
        _bus.When(b => b.InvokeAsync<AnalysisResult>(Arg.Any<object>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new TimeoutException("timed out"));

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        var statusCodeResult = result.ShouldBeOfType<ObjectResult>();
        statusCodeResult.StatusCode.ShouldBe(504);
    }

    [Fact]
    public async Task Analyze_GenericException_Returns500()
    {
        _bus.When(b => b.InvokeAsync<AnalysisResult>(Arg.Any<object>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new InvalidOperationException("unexpected"));

        var result = await _controller.Analyze(CreateFakeFile("jd.pdf"), CreateFakeFile("resume.pdf"), CancellationToken.None);

        var statusCodeResult = result.ShouldBeOfType<ObjectResult>();
        statusCodeResult.StatusCode.ShouldBe(500);
    }

    private static IFormFile CreateFakeFile(string fileName)
    {
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns(fileName);
        file.Length.Returns(100);
        file.OpenReadStream().Returns(new MemoryStream(new byte[100]));
        return file;
    }
}
