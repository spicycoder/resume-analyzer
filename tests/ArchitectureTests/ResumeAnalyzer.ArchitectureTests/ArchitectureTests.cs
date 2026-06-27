using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace ResumeAnalyzer.ArchitectureTests;

public sealed class LayerArchitectureTests
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(ResumeAnalyzer.Domain.Abstractions.IPdfTextExtractor).Assembly,
            typeof(ResumeAnalyzer.Application.ApplicationBootstrap).Assembly,
            typeof(ResumeAnalyzer.Infrastructure.InfrastructureBootstrap).Assembly,
            typeof(ResumeAnalyzer.Api.Controllers.AnalyzeController).Assembly
        )
        .Build();

    private static readonly IObjectProvider<IType> DomainLayer =
        Types().That().ResideInNamespace("ResumeAnalyzer.Domain").As("Domain Layer");

    private static readonly IObjectProvider<IType> ApplicationLayer =
        Types().That().ResideInNamespace("ResumeAnalyzer.Application").As("Application Layer");

    private static readonly IObjectProvider<IType> InfrastructureLayer =
        Types().That().ResideInNamespace("ResumeAnalyzer.Infrastructure").As("Infrastructure Layer");

    private static readonly IObjectProvider<IType> ApiLayer =
        Types().That().ResideInNamespace("ResumeAnalyzer.Api").As("API Layer");

    [Fact]
    public void Domain_Should_Not_Depend_On_Application()
    {
        Types().That().Are(DomainLayer)
            .Should().NotDependOnAny(ApplicationLayer)
            .WithoutRequiringPositiveResults().Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        Types().That().Are(DomainLayer)
            .Should().NotDependOnAny(InfrastructureLayer)
            .WithoutRequiringPositiveResults().Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_API()
    {
        Types().That().Are(DomainLayer)
            .Should().NotDependOnAny(ApiLayer)
            .WithoutRequiringPositiveResults().Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure()
    {
        Types().That().Are(ApplicationLayer)
            .Should().NotDependOnAny(InfrastructureLayer)
            .WithoutRequiringPositiveResults().Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Depend_On_API()
    {
        Types().That().Are(ApplicationLayer)
            .Should().NotDependOnAny(ApiLayer)
            .WithoutRequiringPositiveResults().Check(Architecture);
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Application()
    {
        Types().That().Are(InfrastructureLayer)
            .Should().NotDependOnAny(ApplicationLayer)
            .WithoutRequiringPositiveResults().Check(Architecture);
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_API()
    {
        Types().That().Are(InfrastructureLayer)
            .Should().NotDependOnAny(ApiLayer)
            .WithoutRequiringPositiveResults().Check(Architecture);
    }
}

