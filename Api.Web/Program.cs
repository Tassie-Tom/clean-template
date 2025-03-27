using Api.Application;
using Api.Infrastructure;
using Api.Infrastructure.BackgroundJobs;
using Api.Web.Extenstions;
using DotNetEnv;
using Hangfire;
using Hangfire.Dashboard;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;


if (File.Exists(".env") || File.Exists("../.env"))
{
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure Hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    // Disable authorization for testing
    Authorization = Array.Empty<IDashboardAuthorizationFilter>(),
    // Optional: Disable read-only mode if you want to allow operations from dashboard
    IsReadOnlyFunc = context => false
});

// Schedule outbox processing
RecurringJob.AddOrUpdate<OutboxProcessor>(
    "process-outbox",
    processor => processor.ProcessOutboxMessagesAsync(CancellationToken.None),
    "*/30 * * * * *"); // Run every 30 seconds

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
public partial class Program;

