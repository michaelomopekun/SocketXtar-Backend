using Microsoft.Extensions.Caching.Distributed;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDistributedCache _redis;

    public TokenBlacklistMiddleware(RequestDelegate next, IDistributedCache redis)
    {
        _next = next;
        _redis = redis;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Replace("Bearer ", "").Trim();

            var isBlacklisted = await _redis.GetStringAsync($"blacklist:{token}");

            if (!string.IsNullOrEmpty(isBlacklisted))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token has been revoked.");
                return;
            }
        }

        await _next(context);
    }
}
