using System.Text.Json;
using Microsoft.Extensions.AI;
using ResumeAnalyzer.Application;
using ResumeAnalyzer.Application.Abstractions;
using ResumeAnalyzer.Domain.Models;

namespace ResumeAnalyzer.Infrastructure.Ai;

public class OpenAiResumeAnalyzer(
    IChatClient chatClient,
    SystemPrompt systemPrompt) : IResumeAnalyzer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<AnalysisResult> AnalyzeAsync(string resumeText, string jdText, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resumeText);
        ArgumentException.ThrowIfNullOrWhiteSpace(jdText);

        var userPrompt = $"""
            Resume:
            {resumeText}

            Job Description:
            {jdText}
            """;

        var response = await chatClient.GetResponseAsync(
            [
                new ChatMessage(ChatRole.System, systemPrompt.Content),
                new ChatMessage(ChatRole.User, userPrompt)
            ],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var rawJson = response.Text.Trim();
        
        // Basic cleanup in case the AI wraps it in markdown code blocks
        if (rawJson.StartsWith("```json", StringComparison.Ordinal)) rawJson = rawJson[7..];
        if (rawJson.StartsWith("```", StringComparison.Ordinal)) rawJson = rawJson[3..];
        if (rawJson.EndsWith("```", StringComparison.Ordinal)) rawJson = rawJson[..^3];
        
        return JsonSerializer.Deserialize<AnalysisResult>(rawJson, JsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize AI response.");
    }
}
