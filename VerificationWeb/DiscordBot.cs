using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using VerificationWeb.Configuration;

namespace VerificationWeb
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
            _client.Disconnected += OnDisconnected;
            await Run();

            await Task.Delay(-1);
        }
        private Task OnDisconnected(Exception exception)
        {
            Console.WriteLine("Bot - Disconnected event.");

            if (exception.Message == "Server requested a reconnect" ||
                exception.Message == "Server missed last heartbeat")
                return Task.CompletedTask;

            Dispose();
            Console.WriteLine("Shutting down.");
            Environment.Exit(0); // force restart of the whole application
            return Task.CompletedTask;
        }
        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}