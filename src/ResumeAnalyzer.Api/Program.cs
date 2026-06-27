using AspNetCore.Swagger.Themes;

using ResumeAnalyzer.Application;
using ResumeAnalyzer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
if (allowedOrigins is { Length: > 0 })
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(Theme.Futuristic, options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Resume Analyzer API v1");
    });
}
else
{
    app.UseHttpsRedirection();
}

if (allowedOrigins is { Length: > 0 })
{
    app.UseCors();
}

app.UseAuthorization();
app.MapControllers();

app.Run();