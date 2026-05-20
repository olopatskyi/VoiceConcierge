using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VoiceConcierge.Api.Options;

namespace VoiceConcierge.Api.Middleware;

public class AgentSecretMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Agent-Secret";

    public async Task InvokeAsync(HttpContext context, IOptions<AgentAuthOptions> options)
    {
        var endpoint = context.GetEndpoint();
        var requiresSecret = endpoint?.Metadata.GetMetadata<RequireAgentSecretAttribute>() is not null;

        if (!requiresSecret)
        {
            await next(context);
            return;
        }

        var provided = context.Request.Headers[HeaderName].FirstOrDefault();
        var expected = options.Value.SharedSecret;

        if (!IsValid(provided, expected))
        {
            await WriteUnauthorizedAsync(context);
            return;
        }

        await next(context);
    }

    private static bool IsValid(string? provided, string expected)
    {
        if (string.IsNullOrEmpty(provided))
            return false;

        var providedBytes = Encoding.UTF8.GetBytes(provided);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        if (providedBytes.Length != expectedBytes.Length)
            return false;

        return CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes);
    }

    private static Task WriteUnauthorizedAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Agent authentication required",
            Detail = $"Missing or invalid {HeaderName} header.",
        });
    }
}
