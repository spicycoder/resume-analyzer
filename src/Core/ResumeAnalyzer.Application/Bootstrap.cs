using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.FluentValidation;

namespace ResumeAnalyzer.Application;

public static class ApplicationBootstrap
{
    public static IHostBuilder UseApplication(this IHostBuilder host)
    {
        // 1. Register Application Services (Validators, etc.)
        host.ConfigureServices(services =>
        {
            services.AddValidatorsFromAssembly(typeof(ApplicationBootstrap).Assembly, ServiceLifetime.Transient);

            services.AddSingleton(SystemPrompt.Default);
        });

        // 2. Configure Wolverine
        return host.UseWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(ApplicationBootstrap).Assembly);
            
            // Enable FluentValidation middleware
            opts.UseFluentValidation();
        });
    }
}
