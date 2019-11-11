using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using VerificationWeb.Services;

namespace VerificationWeb.EXtensions
{
    internal static class ServiceExtensions
    {
        public static IServiceCollection AddDiscordBot(this IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Warning
            }));

            services.AddSingleton(new CommandService(new CommandServiceConfig()
            {
                LogLevel = LogSeverity.Debug,
                DefaultRunMode = RunMode.Sync,
                CaseSensitiveCommands = false,
                SeparatorChar = ' '
            }));

            services.AddSingleton<RoleService>();
            services.AddHostedService<DiscordBot>();
            return services;
        }
    }
}