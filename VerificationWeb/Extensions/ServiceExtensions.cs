using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using VerificationWeb.Services;


namespace VerificationWeb.EXtensions
{
    internal static class ServiceExtensions
    {
        /// <summary>
        /// Adds DiscordSocketClient and CommandService as singletons into the services and sets them up
        /// Then adds the DiscordBot as a hosted service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="discordSocketConfig"></param>
        /// <param name="commandServiceConfig"></param>
        /// <returns></returns>
        public static IServiceCollection AddDiscordBot(this IServiceCollection services,
            DiscordSocketConfig discordSocketConfig = null, CommandServiceConfig commandServiceConfig = null)
        {
            if (discordSocketConfig == null)
            {
                services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig()
                {
                    LogLevel = LogSeverity.Warning,
                    AlwaysDownloadUsers = true,
                }));
            }

            if (commandServiceConfig == null)
            {
                services.AddSingleton(new CommandService(new CommandServiceConfig()
                {
                    LogLevel = LogSeverity.Warning,
                    DefaultRunMode = RunMode.Sync,
                    CaseSensitiveCommands = false,
                    SeparatorChar = ' '
                }));
            }

            services.AddSingleton<RoleService>();
            services.AddHostedService<DiscordBot>();
            return services;
        }
    }
}