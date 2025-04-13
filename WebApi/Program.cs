using Application;
using WebApi;
using WebApi.Logging;
using Infrastructure;
using Microsoft.OpenApi.Models;
using WebApi.Middlewares;
using Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data.DbContext;
var builder = WebApplication.CreateBuilder(args);

// Configure custom colored console logging
builder.Logging.ClearProviders();
builder.Logging.AddColoredConsoleLogger(options =>
{
    options.LogLevel = LogLevel.Debug;
    options.ColorEnabled = true;
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplicationService();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddWebApiService(builder.Configuration);


// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

// Only use HTTPS redirection in production to avoid warnings in development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Configure health check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.UseCors("AllowAllOrigins");


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
