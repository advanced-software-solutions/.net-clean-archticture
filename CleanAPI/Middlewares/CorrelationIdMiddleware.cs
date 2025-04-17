using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace CleanAPI.Middlewares
{
    public class CorrelationIdMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.Headers.TryGetValue("Correlation-Id", out StringValues correlationIds);
            var correlationId = correlationIds.FirstOrDefault() ?? Guid.NewGuid().ToString();

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next(context);
            }
        }
    }
}
