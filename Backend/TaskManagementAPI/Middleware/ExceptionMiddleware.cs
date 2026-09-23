using System.Net;
using System.Text.Json;
using TaskManagementAPI.Exceptions;

namespace TaskManagementAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ForbiddenAccessException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (BadRequestException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new { message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
