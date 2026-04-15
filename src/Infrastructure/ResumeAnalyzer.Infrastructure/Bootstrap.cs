using Microsoft.Extensions.AI;
using System.ClientModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using ResumeAnalyzer.Application.Abstractions;
using ResumeAnalyzer.Infrastructure.Ai;
using ResumeAnalyzer.Infrastructure.Pdf;

namespace ResumeAnalyzer.Infrastructure;

public static class InfrastructureBootstrap
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddScoped<IPdfTextExtractor, PdfTextExtractor>();
        services.AddScoped<IResumeAnalyzer, OpenAiResumeAnalyzer>();

        // AI Provider Configuration (OpenAI compatible)
        var aiOptions = configuration.GetSection("Ai");
        var apiKey = aiOptions["ApiKey"] ?? "none";
        var endpoint = aiOptions["Endpoint"];
        var model = aiOptions["Model"] ?? "gpt-4o";

        var client = endpoint != null
            ? new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
            : new OpenAIClient(new ApiKeyCredential(apiKey));

        services.AddChatClient(client.GetChatClient(model).AsIChatClient());

        return services;
    }
}
