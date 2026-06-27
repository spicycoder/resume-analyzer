using NSubstitute;
using Shouldly;
using ResumeAnalyzer.Application.Abstractions;
using ResumeAnalyzer.Application.UseCases.Queries;
using ResumeAnalyzer.Domain.Models;
using Xunit;

namespace ResumeAnalyzer.Application.UnitTests;

public class AnalyzeResumeQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidQuery_ReturnsAnalysisResult()
    {
        var jdText = "Senior .NET Developer required.";
        var resumeText = "5 years .NET experience.";
        var expectedResult = new AnalysisResult(
            75,
            new List<Flag> { new("generic", "Strong .NET background") },
            new List<Flag>());

        var extractor = Substitute.For<IPdfTextExtractor>();
        extractor.ExtractText(Arg.Any<Stream>()).Returns(jdText, resumeText);

        var analyzer = Substitute.For<IResumeAnalyzer>();
        analyzer.AnalyzeAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        var handler = new AnalyzeResumeQueryHandler(extractor, analyzer);

        var query = new AnalyzeResumeQuery(
            new MemoryStream(), "jd.pdf", 100,
            new MemoryStream(), "resume.pdf", 100);

        var result = await handler.Handle(query, CancellationToken.None);

        result.MatchPercentage.ShouldBe(expectedResult.MatchPercentage);
    }

    [Fact]
    public async Task Handle_QueryNull_ThrowsArgumentNullException()
    {
        var extractor = Substitute.For<IPdfTextExtractor>();
        var analyzer = Substitute.For<IResumeAnalyzer>();
        var handler = new AnalyzeResumeQueryHandler(extractor, analyzer);

        await Should.ThrowAsync<ArgumentNullException>(() => handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExtractsTextFromBothStreams()
    {
        var jdText = "Job description text";
        var resumeText = "Resume text";
        var expectedResult = new AnalysisResult(50, new List<Flag>(), new List<Flag>());

        var extractor = Substitute.For<IPdfTextExtractor>();
        extractor.ExtractText(Arg.Any<Stream>()).Returns(jdText, resumeText);

        var analyzer = Substitute.For<IResumeAnalyzer>();
        analyzer.AnalyzeAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        var handler = new AnalyzeResumeQueryHandler(extractor, analyzer);

        var query = new AnalyzeResumeQuery(
            new MemoryStream(), "jd.pdf", 100,
            new MemoryStream(), "resume.pdf", 100);

        await handler.Handle(query, CancellationToken.None);

        extractor.Received(2).ExtractText(Arg.Any<Stream>());
    }
}