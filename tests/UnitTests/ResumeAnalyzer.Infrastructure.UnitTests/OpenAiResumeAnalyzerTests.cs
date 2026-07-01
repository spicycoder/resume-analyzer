using System.Net;
using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using ResumeAnalyzer.Domain.Abstractions;
using ResumeAnalyzer.Domain.Exceptions;
using ResumeAnalyzer.Domain.Models;
using ResumeAnalyzer.Infrastructure.Ai;
using Xunit;

namespace ResumeAnalyzer.Infrastructure.UnitTests;

public class OpenAiResumeAnalyzerTests
{
    private readonly IChatClient _chatClient;
    private readonly SystemPrompt _systemPrompt = new("You are a resume analyzer.");
    private readonly OpenAiResumeAnalyzer _analyzer;

    private static IConfiguration MakeConfig(string timeout = "150")
    {
        var dict = new Dictionary<string, string?> { ["Ai:TimeoutSeconds"] = timeout };
        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    public OpenAiResumeAnalyzerTests()
    {
        _chatClient = Substitute.For<IChatClient>();
        _analyzer = new OpenAiResumeAnalyzer(_chatClient, _systemPrompt, MakeConfig());
    }

    [Fact]
    public void Constructor_AcceptsNullChatClient()
    {
        var analyzer = new OpenAiResumeAnalyzer(null!, _systemPrompt, MakeConfig());
        analyzer.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_AcceptsNullSystemPrompt()
    {
        var analyzer = new OpenAiResumeAnalyzer(_chatClient, null!, MakeConfig());
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

        _chatClient
            .GetResponseAsync(
                Arg.Any<IEnumerable<ChatMessage>>(),
                Arg.Any<ChatOptions?>(),
                Arg.Any<CancellationToken>())
            .Returns(new ChatResponse([new ChatMessage(ChatRole.Assistant, responseJson)]));

        var result = await _analyzer.AnalyzeAsync("resume text", "job description", CancellationToken.None);

        result.MatchPercentage.ShouldBe(85);
        await _chatClient.Received(1).GetResponseAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatOptions?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AnalyzeAsync_ResponseWithMarkdownJson_CleansUpAndDeserializes()
    {
        var analysisResult = new AnalysisResult(
            72,
            [new Flag("Experience", "5 years exp")], [new Flag("Skill", "Proficient in React")]);
        var responseJson = "```json\n" + JsonSerializer.Serialize(analysisResult) + "\n```";

        _chatClient
            .GetResponseAsync(
                Arg.Any<IEnumerable<ChatMessage>>(),
                Arg.Any<ChatOptions?>(),
                Arg.Any<CancellationToken>())
            .Returns(new ChatResponse([new ChatMessage(ChatRole.Assistant, responseJson)]));

        var result = await _analyzer.AnalyzeAsync("resume", "jd", CancellationToken.None);

        result.MatchPercentage.ShouldBe(72);
    }

    [Fact]
    public async Task AnalyzeAsync_ResponseWithTripleBackticks_CleansUpAndDeserializes()
    {
        var responseJson = "```\n{\"matchPercentage\":90,\"greenFlags\":[],\"redFlags\":[]}\n```";

        _chatClient
            .GetResponseAsync(
                Arg.Any<IEnumerable<ChatMessage>>(),
                Arg.Any<ChatOptions?>(),
                Arg.Any<CancellationToken>())
            .Returns(new ChatResponse([new ChatMessage(ChatRole.Assistant, responseJson)]));

        var result = await _analyzer.AnalyzeAsync("resume", "jd", CancellationToken.None);

        result.MatchPercentage.ShouldBe(90);
    }

    [Fact]
    public async Task AnalyzeAsync_NullAiResponse_Throws()
    {
        _chatClient
            .GetResponseAsync(
                Arg.Any<IEnumerable<ChatMessage>>(),
                Arg.Any<ChatOptions?>(),
                Arg.Any<CancellationToken>())
            .Returns((ChatResponse)null!);

        await Should.ThrowAsync<NullReferenceException>(() =>
            _analyzer.AnalyzeAsync("resume", "jd", CancellationToken.None));
    }

    [Fact]
    public async Task AnalyzeAsync_SystemPromptIncludedInRequest()
    {
        var responseJson = JsonSerializer.Serialize(new AnalysisResult(50, [], []));
        List<ChatMessage>? capturedMessages = null;

        _chatClient
            .GetResponseAsync(
                Arg.Any<IEnumerable<ChatMessage>>(),
                Arg.Any<ChatOptions?>(),
                Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                capturedMessages = ci.ArgAt<IEnumerable<ChatMessage>>(0).ToList();
                return new ChatResponse([new ChatMessage(ChatRole.Assistant, responseJson)]);
            });

        await _analyzer.AnalyzeAsync("my resume", "my jd", CancellationToken.None);

        capturedMessages.ShouldNotBeNull();
        capturedMessages.Count.ShouldBe(2);
        capturedMessages[0].Role.ShouldBe(ChatRole.System);
        capturedMessages[0].Text.ShouldBe("You are a resume analyzer.");
        capturedMessages[1].Role.ShouldBe(ChatRole.User);
        capturedMessages[1].Text.ShouldContain("my resume");
        capturedMessages[1].Text.ShouldContain("my jd");
    }

    [Fact]
    public async Task AnalyzeAsync_OperationCancelled_ThrowsTimeoutException()
    {
        _chatClient
            .GetResponseAsync(Arg.Any<IEnumerable<ChatMessage>>(), Arg.Any<ChatOptions?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<ChatResponse>(new OperationCanceledException()));

        var ex = await Should.ThrowAsync<TimeoutException>(() =>
            _analyzer.AnalyzeAsync("resume", "jd", CancellationToken.None));

        ex.Message.ShouldContain("150 seconds");
    }

    [Fact]
    public async Task AnalyzeAsync_HttpRequest429_ThrowsRateLimitExceededException()
    {
        _chatClient
            .GetResponseAsync(Arg.Any<IEnumerable<ChatMessage>>(), Arg.Any<ChatOptions?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<ChatResponse>(new HttpRequestException(null, null, HttpStatusCode.TooManyRequests)));

        var ex = await Should.ThrowAsync<RateLimitExceededException>(() =>
            _analyzer.AnalyzeAsync("resume", "jd", CancellationToken.None));

        ex.Message.ShouldContain("rate limit");
    }

    [Fact]
    public async Task AnalyzeAsync_InvalidJsonResponse_ThrowsInvalidOperationException()
    {
        _chatClient
            .GetResponseAsync(Arg.Any<IEnumerable<ChatMessage>>(), Arg.Any<ChatOptions?>(), Arg.Any<CancellationToken>())
            .Returns(new ChatResponse([new ChatMessage(ChatRole.Assistant, "null")]));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _analyzer.AnalyzeAsync("resume", "jd", CancellationToken.None));
    }
}
