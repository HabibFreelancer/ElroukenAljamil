using FluentValidation;
using ElroukenAljamil.BuildingBlocks.Common.Exceptions;
using System.Net;
using System.Text.Json;


namespace ElroukenAljamil.Listings.API.Middleware
{
    /// <summary>
    /// Middleware global de gestion des exceptions.
    /// Transforme les exceptions métier en réponses HTTP appropriées.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                ValidationException validationEx => (
                    HttpStatusCode.BadRequest,
                    new ErrorResponse("Validation échouée", validationEx.Errors.Select(e => e.ErrorMessage).ToList())
                ),
                DomainException domainEx => (
                    HttpStatusCode.BadRequest,
                    new ErrorResponse(domainEx.Message, new List<string>())
                ),
                KeyNotFoundException => (
                    HttpStatusCode.NotFound,
                    new ErrorResponse("Ressource introuvable", new List<string>())
                ),
                _ => (
                    HttpStatusCode.InternalServerError,
                    new ErrorResponse("Une erreur interne est survenue", new List<string>())
                )
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Erreur non gérée: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    public record ErrorResponse(string Message, List<string> Errors);

}
