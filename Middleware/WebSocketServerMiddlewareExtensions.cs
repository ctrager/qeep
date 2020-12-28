using Microsoft.AspNetCore.Builder;


namespace qeep
{


    public static class WebSocketServerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorey(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketServerMiddleware>();
        }
    }
}