using Shouldly;
using ResumeAnalyzer.Application;
using Xunit;

namespace ResumeAnalyzer.Application.UnitTests;

public class SystemPromptTests
{
    [Fact]
    public void Create_WithContent_ContentSet()
    {
        var prompt = new SystemPrompt("You are an expert hiring analyst.");

        prompt.Content.ShouldBe("You are an expert hiring analyst.");
    }

    [Fact]
    public void Create_WithEmptyContent_ContentSet()
    {
        var prompt = new SystemPrompt("");

        prompt.Content.ShouldBeEmpty();
    }
}