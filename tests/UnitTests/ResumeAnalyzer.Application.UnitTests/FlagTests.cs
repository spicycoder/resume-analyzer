using Shouldly;
using ResumeAnalyzer.Domain.Models;
using Xunit;

namespace ResumeAnalyzer.Application.UnitTests;

public class FlagTests
{
    [Fact]
    public void Create_WithValidParams_PropertiesSetCorrectly()
    {
        var flag = new Flag("generic", "Strong technical background");

        flag.Category.ShouldBe("generic");
        flag.Description.ShouldBe("Strong technical background");
    }

    [Fact]
    public void Create_WithJdSpecific_CategorySet()
    {
        var flag = new Flag("jd-specific", "React experience");

        flag.Category.ShouldBe("jd-specific");
    }
}