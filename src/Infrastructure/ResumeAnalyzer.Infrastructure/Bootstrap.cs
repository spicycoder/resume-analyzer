using Microsoft.Extensions.DependencyInjection;
using ResumeAnalyzer.Application.Abstractions;
using ResumeAnalyzer.Infrastructure.Pdf;

namespace ResumeAnalyzer.Infrastructure;

public static class InfrastructureBootstrap
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPdfTextExtractor, PdfTextExtractor>();
        return services;
    }
}
