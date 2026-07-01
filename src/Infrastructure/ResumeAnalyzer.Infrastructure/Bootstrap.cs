using System.ClientModel;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI;

using ResumeAnalyzer.Domain.Abstractions;
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
        var timeout = TimeSpan.FromSeconds(int.Parse(aiOptions["TimeoutSeconds"] ?? "150"));

        var clientOptions = new OpenAIClientOptions
        {
            NetworkTimeout = timeout
        };

        if (endpoint != null)
        {
            clientOptions.Endpoint = new Uri(endpoint);
        }

        var client = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);

        services.AddChatClient(client.GetChatClient(model).AsIChatClient());

        return services;
    }
}