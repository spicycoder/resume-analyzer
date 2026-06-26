using AspNetCore.Swagger.Themes;

using ResumeAnalyzer.Application;
using ResumeAnalyzer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(
        Theme.Futuristic,
        options => options.SwaggerEndpoint("/openapi/v1.json", "Resume Analyzer API v1"));
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();