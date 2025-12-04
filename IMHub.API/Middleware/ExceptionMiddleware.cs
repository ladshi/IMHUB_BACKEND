using System.Net;
using System.Text.Json;
using IMHub.ApplicationLayer.Common.Exceptions;

namespace IMHub.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionMiddleware(
            RequestDelegate next, 
            ILogger<ExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var correlationId = context.Items["CorrelationId"]?.ToString() ?? "Unknown";
                _logger.LogError(ex, "An unhandled exception has occurred. CorrelationId: {CorrelationId}", correlationId);
                await HandleExceptionAsync(context, ex, correlationId);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
        {
            context.Response.ContentType = "application/json";
            
            object response;
            int statusCode = StatusCodes.Status500InternalServerError;

            switch (exception)
            {
                case ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    response = new 
                    { 
                        error = "Validation Failed", 
                        details = validationEx.Errors,
                        correlationId = correlationId
                    };
                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }));
                    return;

                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    response = new { error = "Resource not found", correlationId = correlationId };
                    break;
                
                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    response = new { error = "Unauthorized", correlationId = correlationId };
                    break;
                
                case ArgumentException:
                    statusCode = StatusCodes.Status400BadRequest;
                    response = new { error = exception.Message, correlationId = correlationId };
                    break;
                
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    response = new { error = "Internal Server Error", correlationId = correlationId };
                    break;
            }

            context.Response.StatusCode = statusCode;
            
            // In Dev, show real error message. In Prod, show generic message.
            var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await context.Response.WriteAsync(result);
        }
    }
}
