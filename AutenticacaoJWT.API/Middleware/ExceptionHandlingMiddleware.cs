using System.Net;
using System.Text.Json;

namespace AutenticacaoJWT.API.Middleware
{
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
                _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
                await WriteProblemAsync(context, HttpStatusCode.InternalServerError,
                    "Ocorreu um erro interno. Tente novamente mais tarde.");
            }
        }

        private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode status, string detail)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)status;

            var problem = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                title = status.ToString(),
                status = (int)status,
                detail,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
