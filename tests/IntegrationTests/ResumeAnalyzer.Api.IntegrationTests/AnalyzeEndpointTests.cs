using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace ResumeAnalyzer.Api.IntegrationTests;

public class AnalyzeEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AnalyzeEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Analyze_MissingFiles_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();

        using var content = new MultipartFormDataContent();

        var response = await client.PostAsync("/api/analyze", content);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Analyze_InvalidContentType_ReturnsUnsupportedMediaType()
    {
        var client = _factory.CreateClient();

        using var content = new StringContent("not a pdf");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await client.PostAsync("/api/analyze", content);

        response.StatusCode.ShouldBe(HttpStatusCode.UnsupportedMediaType);
    }
}