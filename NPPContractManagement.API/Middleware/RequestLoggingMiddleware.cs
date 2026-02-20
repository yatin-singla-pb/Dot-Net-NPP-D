using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NPPContractManagement.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next; _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (HttpMethods.IsPost(context.Request.Method) || HttpMethods.IsPut(context.Request.Method))
            {
                try
                {
                    context.Request.EnableBuffering();
                    context.Request.Body.Position = 0;
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Items["RawBody"] = body;
                    context.Request.Body.Position = 0;

                    // Duplicate key detection (case-insensitive)
                    var duplicateKeys = GetDuplicateKeys(body);
                    if (duplicateKeys.Count > 0)
                    {
                        context.Items["HasDuplicateKeys"] = true;
                        context.Items["DuplicateKeys"] = duplicateKeys;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to capture request body for {Path}", context.Request.Path);
                }
            }

            await _next(context);
        }

        private static List<string> GetDuplicateKeys(string json)
        {
            var dups = new List<string>();
            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                var reader = new Utf8JsonReader(bytes, new JsonReaderOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        var name = reader.GetString() ?? string.Empty;
                        if (!keys.Add(name))
                        {
                            var norm = name.ToLowerInvariant();
                            if (!dups.Contains(norm)) dups.Add(norm);
                        }
                    }
                }
            }
            catch
            {
                // ignore parsing issues
            }
            return dups;
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLoggingWithBody(this IApplicationBuilder app)
            => app.UseMiddleware<RequestLoggingMiddleware>();
    }
}

