using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ChatServerApp.SocketsManager
{
    public static class SocketsExtension
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<IConnectionManager, ConnectionManager>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(SocketHandler))
                {
                    services.AddSingleton(type);
                }
            }
            return services;
        }

        public static IApplicationBuilder MapSockets(
            this IApplicationBuilder app,
            PathString path,
            SocketHandler socketHandler)
        {
            app.Map(path, (x) => x.UseMiddleware<SocketMiddleware>(socketHandler));
            return app;
        }
    }
}
