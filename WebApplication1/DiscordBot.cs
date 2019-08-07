using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using WebApplication1.Configuration;

namespace WebApplication1
{
    public class DiscordBot : BackgroundService
    {
        private readonly DiscordSocketClient _client;
        private readonly Config _config;

        public DiscordBot(Config config, DiscordSocketClient client)
        {
            _config = config;
            _client = client;
        }

        private async Task Run()
        {
            await _client.LoginAsync(TokenType.Bot, _config.DiscordToken);
            await _client.StartAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("DiscordBot is up and running");

            _client.Log += Log;

            await Run();

            await Task.Delay(-1);
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}