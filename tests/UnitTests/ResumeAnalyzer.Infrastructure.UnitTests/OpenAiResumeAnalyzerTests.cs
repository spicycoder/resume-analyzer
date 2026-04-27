using System.Text.Json;
using Microsoft.Extensions.AI;
using Moq;
using Shouldly;
using ResumeAnalyzer.Application;
using ResumeAnalyzer.Application.Abstractions;
using ResumeAnalyzer.Domain.Models;
using ResumeAnalyzer.Infrastructure.Ai;
using Xunit;

namespace ResumeAnalyzer.Infrastructure.UnitTests;

public class OpenAiResumeAnalyzerTests
{
    private readonly Mock<IChatClient> _chatClientMock;
    private readonly SystemPrompt _systemPrompt = new("You are a resume analyzer.");
    private readonly OpenAiResumeAnalyzer _analyzer;

    public OpenAiResumeAnalyzerTests()
    {
        _chatClientMock = new Mock<IChatClient>();
        _analyzer = new OpenAiResumeAnalyzer(_chatClientMock.Object, _systemPrompt);
    }

    [Fact]
    public void Constructor_AcceptsNullChatClient()
    {
        var analyzer = new OpenAiResumeAnalyzer(null!, _systemPrompt);
        analyzer.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_AcceptsNullSystemPrompt()
    {
        var analyzer = new OpenAiResumeAnalyzer(_chatClientMock.Object, null!);
        analyzer.ShouldNotBeNull();
    }

    [Fact]
    public async Task AnalyzeAsync_NullResumeText_ThrowsArgumentException()
    {
        var ex = await Should.ThrowAsync<ArgumentException>(() => 
            _analyzer.AnalyzeAsync(null!, "jd", CancellationToken.None));

        ex.Message.ShouldContain("resumeText");
    }

    [Fact]
    public async Task AnalyzeAsync_EmptyResumeText_ThrowsArgumentException()
    {
        var ex = await Should.ThrowAsync<ArgumentException>(() => 
            _analyzer.AnalyzeAsync("", "jd", CancellationToken.None));

        ex.Message.ShouldContain("resumeText");
    }

    [Fact]
    public async Task AnalyzeAsync_WhitespaceResumeText_ThrowsArgumentException()
    {
        var ex = await Should.ThrowAsync<ArgumentException>(() => 
            _analyzer.AnalyzeAsync("   ", "jd", CancellationToken.None));

        ex.Message.ShouldContain("resumeText");
    }

    [Fact]
    public async Task AnalyzeAsync_NullJdText_ThrowsArgumentException()
    {
        var ex = await Should.ThrowAsync<ArgumentException>(() => 
            _analyzer.AnalyzeAsync("resume", null!, CancellationToken.None));

        ex.Message.ShouldContain("jdText");
    }

    [Fact]
    public async Task AnalyzeAsync_EmptyJdText_ThrowsArgumentException()
    {
        var ex = await Should.ThrowAsync<ArgumentException>(() => 
            _analyzer.AnalyzeAsync("resume", "", CancellationToken.None));

        ex.Message.ShouldContain("jdText");
    }

    [Fact]
    public async Task AnalyzeAsync_ValidInput_ReturnsAnalysisResult()
    {
        var responseJson = JsonSerializer.Serialize(new AnalysisResult(
            85,
            [new Flag("Good", "Strong match")],
            []));

        _chatClientMock
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatResponse([new ChatMessage(ChatRole.Assistant, responseJson)]));

        var result = await _analyzer.AnalyzeAsync("resume text", "job description", CancellationToken.None);

        result.MatchPercentage.ShouldBe(85);
        _chatClientMock.Verify(c => c.GetResponseAsync(
            It.IsAny<IEnumerable<ChatMessage>>(),
            It.IsAny<ChatOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AnalyzeAsync_ResponseWithMarkdownJson_CleansUpAndDeserializes()
    {
        var analysisResult = new AnalysisResult(
            72,
            [new Flag("Experience", "5 years exp")], [new Flag("Skill", "Proficient in React")]);
        var responseJson = "```json\n" + JsonSerializer.Serialize(analysisResult) + "\n```";

        _chatClientMock
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatResponse([new ChatMessage(ChatRole.Assistant, responseJson)]));

        var result = await _analyzer.AnalyzeAsync("resume", "jd", CancellationToken.None);

        result.MatchPercentage.ShouldBe(72);
    }

    [Fact]
    public async Task AnalyzeAsync_ResponseWithTripleBackticks_CleansUpAndDeserializes()
    {
        var responseJson = "```\n{\"matchPercentage\":90,\"greenFlags\":[],\"redFlags\":[]}\n```";

        _chatClientMock
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatResponse([new ChatMessage(ChatRole.Assistant, responseJson)]));

        var result = await _analyzer.AnalyzeAsync("resume", "jd", CancellationToken.None);

        result.MatchPercentage.ShouldBe(90);
    }

    [Fact]
    public async Task AnalyzeAsync_NullAiResponse_Throws()
    {
        _chatClientMock
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChatResponse)null!);

        await Should.ThrowAsync<NullReferenceException>(() =>
            _analyzer.AnalyzeAsync("resume", "jd", CancellationToken.None));
    }

    [Fact]
    public async Task AnalyzeAsync_SystemPromptIncludedInRequest()
    {
        var responseJson = JsonSerializer.Serialize(new AnalysisResult(50, [], []));
        List<ChatMessage>? capturedMessages = null;

        _chatClientMock
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<ChatMessage>, ChatOptions?, CancellationToken>((msgs, _, _) => 
                capturedMessages = msgs.ToList())
            .ReturnsAsync(new ChatResponse([new ChatMessage(ChatRole.Assistant, responseJson)]));

        await _analyzer.AnalyzeAsync("my resume", "my jd", CancellationToken.None);

        capturedMessages.ShouldNotBeNull();
        capturedMessages.Count.ShouldBe(2);
        capturedMessages[0].Role.ShouldBe(ChatRole.System);
        capturedMessages[0].Text.ShouldBe("You are a resume analyzer.");
        capturedMessages[1].Role.ShouldBe(ChatRole.User);
        capturedMessages[1].Text.ShouldContain("my resume");
        capturedMessages[1].Text.ShouldContain("my jd");
    }
}