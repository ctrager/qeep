// From https://dotnetplaybook.com/which-is-best-websockets-or-signalr/

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace qeep
{

    public static class WebSocketServerMiddlewareExtensions
    {
        public static IApplicationBuilder UseQeepWebSocketMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketServerMiddleware>();
        }

        public static IServiceCollection AddConnectionManager(this IServiceCollection services)
        {
            services.AddSingleton<ConnectionManager>();
            return services;
        }
    }
}