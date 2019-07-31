using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Services;

namespace WebApplication1.EXtensions
{
    internal static class ServiceExtensions
    {
        public static IServiceCollection AddDiscordBot(this IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Debug
            }));

            services.AddSingleton(new CommandService(new CommandServiceConfig()
            {
                LogLevel = LogSeverity.Debug,
                DefaultRunMode = RunMode.Sync,
                CaseSensitiveCommands = false,
                SeparatorChar = ' '
            }));

            services.AddHostedService<DiscordBot>();
            services.AddSingleton<RoleService>();
            return services;
        }
    }
}