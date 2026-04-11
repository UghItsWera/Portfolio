using PortfolioCMS.Data;
using PortfolioCMS.Models;

namespace PortfolioCMS.Middleware
{
    public class VisitTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        // Paths to ignore
        private static readonly string[] _ignoredPrefixes = new[]
        {
            "/Admin",
            "/css",
            "/js",
            "/lib",
            "/images",
            "/_",
            "/favicon"
        };

        public VisitTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            var path = context.Request.Path.Value ?? "/";

            var shouldTrack = !_ignoredPrefixes.Any(p =>
                path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            if (shouldTrack)
            {
                var visit = new PageVisit
                {
                    Path = path,
                    VisitedAt = DateTime.UtcNow,
                    UserAgent = context.Request.Headers["User-Agent"].ToString(),
                    IpAddress = context.Connection.RemoteIpAddress?.ToString()
                };

                db.PageVisits.Add(visit);
                await db.SaveChangesAsync();
            }

            await _next(context);
        }
    }
}