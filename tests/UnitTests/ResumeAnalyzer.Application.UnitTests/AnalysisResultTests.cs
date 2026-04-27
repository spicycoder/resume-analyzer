using Shouldly;
using ResumeAnalyzer.Domain.Models;
using Xunit;

namespace ResumeAnalyzer.Application.UnitTests;

public class AnalysisResultTests
{
    [Fact]
    public void Create_WithValidParams_PropertiesSetCorrectly()
    {
        var greenFlags = new List<Flag> { new("generic", "Good") };
        var redFlags = new List<Flag> { new("jd-specific", "Missing skills") };
        var result = new AnalysisResult(85, greenFlags, redFlags);

        result.MatchPercentage.ShouldBe(85);
        result.GreenFlags.ShouldHaveSingleItem();
        result.RedFlags.ShouldHaveSingleItem();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void Create_WithVariousPercentages_Accepted(int percentage)
    {
        var result = new AnalysisResult(percentage, new List<Flag>(), new List<Flag>());

        result.MatchPercentage.ShouldBe(percentage);
    }

    [Fact]
    public void Create_WithEmptyFlags_ListsAreEmpty()
    {
        var result = new AnalysisResult(50, new List<Flag>(), new List<Flag>());

        result.GreenFlags.ShouldBeEmpty();
        result.RedFlags.ShouldBeEmpty();
    }
}