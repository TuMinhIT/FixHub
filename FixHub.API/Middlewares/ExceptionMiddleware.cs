using System.Net;
using System.Text.Json;
using FixHub.Application.Common.Exceptions; 
using FixHub.Application.Common.Models;
using FluentValidation;

namespace FixHub.API.Middlewares
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

         
            string message = exception.Message;
            List<ValidationError> errors = new();

            int statusCode = exception switch
            {
                // catch Validation của FluentValidation
                ValidationException validationEx => 
                    ValidateAndFormatErrors(validationEx, out message, out errors),
                BadRequestException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ForbiddenException or ForbiddenAccessException => (int)HttpStatusCode.Forbidden,
                NotFoundException => (int)HttpStatusCode.NotFound,
                ConflictException => (int)HttpStatusCode.Conflict,
                ServiceUnavailableException => (int)HttpStatusCode.ServiceUnavailable,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var response = new ErrorResponse
            {
                success = false,
                errorMessage = message,           
                errors = errors, 
                data = null
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonResponse = JsonSerializer.Serialize(response, options);          
            await context.Response.WriteAsync(jsonResponse);
        }

        private static int ValidateAndFormatErrors(ValidationException validationEx, out string message, out List<ValidationError> errors)
        {
            message = "Data input is not valid!";
            
            // Map từ FluentValidation.Error sang List<ValidationError>
            errors = validationEx.Errors
                .Select(e => new ValidationError
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                })
                .ToList();

            return (int)HttpStatusCode.BadRequest;
        }
    }
}
