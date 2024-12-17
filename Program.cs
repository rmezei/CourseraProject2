using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.UseRouting();

// Global exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An unhandled exception has occurred.");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync($"Internal server error: {ex.Message}");
    }
});

// Simple token authentication middleware
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.TryGetValue("Authorization", out var token) || token != "Bearer simple-token")
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
        return;
    }

    await next.Invoke();
});

// Middleware to log requests and responses
app.Use(async (context, next) =>
{
    // Log the request
    var request = context.Request;
    logger.LogInformation($"Incoming Request: {request.Method} {request.Path}");

    // Copy the original response body stream
    var originalResponseBodyStream = context.Response.Body;

    using (var responseBody = new MemoryStream())
    {
        context.Response.Body = responseBody;

        await next.Invoke();

        // Log the response
        var response = context.Response;
        response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        logger.LogInformation($"Outgoing Response: {response.StatusCode} {responseText}");

        // Copy the contents of the new memory stream (which contains the response) to the original stream
        await responseBody.CopyToAsync(originalResponseBodyStream);
    }
});

// Configure the HTTP request pipeline.
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
