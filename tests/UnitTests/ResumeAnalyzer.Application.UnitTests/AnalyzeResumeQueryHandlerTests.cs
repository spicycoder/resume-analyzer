using Moq;
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

        var mockExtractor = new Mock<IPdfTextExtractor>();
        mockExtractor.SetupSequence(e => e.ExtractText(It.IsAny<Stream>()))
            .Returns(jdText)
            .Returns(resumeText);

        var mockAnalyzer = new Mock<IResumeAnalyzer>();
        mockAnalyzer.Setup(a => a.AnalyzeAsync(resumeText, jdText, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var handler = new AnalyzeResumeQueryHandler(mockExtractor.Object, mockAnalyzer.Object);

        var query = new AnalyzeResumeQuery(
            new MemoryStream(), "jd.pdf", 100,
            new MemoryStream(), "resume.pdf", 100);

        var result = await handler.Handle(query, CancellationToken.None);

        result.MatchPercentage.ShouldBe(expectedResult.MatchPercentage);
    }

    [Fact]
    public async Task Handle_QueryNull_ThrowsArgumentNullException()
    {
        var mockExtractor = new Mock<IPdfTextExtractor>();
        var mockAnalyzer = new Mock<IResumeAnalyzer>();
        var handler = new AnalyzeResumeQueryHandler(mockExtractor.Object, mockAnalyzer.Object);

        await Should.ThrowAsync<ArgumentNullException>(() => handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExtractsTextFromBothStreams()
    {
        var jdText = "Job description text";
        var resumeText = "Resume text";
        var expectedResult = new AnalysisResult(50, new List<Flag>(), new List<Flag>());

        var mockExtractor = new Mock<IPdfTextExtractor>();
        mockExtractor.SetupSequence(e => e.ExtractText(It.IsAny<Stream>()))
            .Returns(jdText)
            .Returns(resumeText);

        var mockAnalyzer = new Mock<IResumeAnalyzer>();
        mockAnalyzer.Setup(a => a.AnalyzeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var handler = new AnalyzeResumeQueryHandler(mockExtractor.Object, mockAnalyzer.Object);

        var query = new AnalyzeResumeQuery(
            new MemoryStream(), "jd.pdf", 100,
            new MemoryStream(), "resume.pdf", 100);

        await handler.Handle(query, CancellationToken.None);

        mockExtractor.Verify(e => e.ExtractText(It.IsAny<Stream>()), Times.Exactly(2));
    }
}