using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace WebApi.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;
            string? requestBody = null;

            try
            {
                // Only capture request body in development to avoid performance issues in production
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    // Enable buffering to read request body
                    context.Request.EnableBuffering();
                    
                    using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
                    {
                        requestBody = await reader.ReadToEndAsync();
                        
                        // Reset position to allow middleware to read it again
                        context.Request.Body.Position = 0;
                    }
                }
                
                var timestamp = DateTimeOffset.UtcNow;
                
                await _next(context);
                
                stopwatch.Stop();
                var logEntry = FormatLogEntry(context, timestamp, stopwatch.Elapsed, requestBody);
                _logger.LogInformation(logEntry);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Request failed: {context.Request.Method} {context.Request.Path} - {ex.Message}");
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private string FormatLogEntry(HttpContext context, DateTimeOffset timestamp, TimeSpan elapsed, string? requestBody)
        {
            var statusCode = context.Response.StatusCode;
            var method = context.Request.Method;
            var path = context.Request.Path.Value + context.Request.QueryString.Value;
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "-";
            var userAgent = context.Request.Headers["User-Agent"].ToString() ?? "-";
            var contentLength = context.Response.ContentLength?.ToString() ?? "-";
            var elapsedMs = elapsed.TotalMilliseconds.ToString("0.000");
            
            // Use the same format as the Morgan example the user provided
            var bodyJson = string.IsNullOrEmpty(requestBody) ? "{}" : requestBody;
            
            return $"{timestamp:yyyy-MM-ddTHH:mm:ss.fffZ} [{method}] {path} {statusCode} {contentLength} - {elapsedMs} ms {bodyJson} - IP: {ip} - {userAgent}";
        }
    }
}