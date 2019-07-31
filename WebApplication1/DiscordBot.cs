using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApplication1
{
    public class DiscordBot : BackgroundService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _config;

        public DiscordBot(IConfiguration config, DiscordSocketClient client, CommandService commands)
        {
            _config = config;
            _client = client;
            _commands = commands;


            _serviceProvider = new ServiceCollection()
                .AddSingleton(_client)
                .BuildServiceProvider();
        }

        private async Task Run()
        {
            await _client.LoginAsync(TokenType.Bot, _config["DiscordToken"]);
            await _client.StartAsync();

            await _client.SetGameAsync("By: HoundThe");
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("DiscordBot is up and running");

            _client.Log += Log;
            _client.MessageReceived += HandleCommand;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

            await Run();

            await Task.Delay(-1);
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }

        private async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            await message.Channel.SendMessageAsync("Hello");
            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(context, argPos, _serviceProvider);
        }

        public async Task AssignRole(ulong roleId, ulong userId)
        {
            var user = _client.Guilds.FirstOrDefault().Users.FirstOrDefault(x => x.Id == userId);
            await user.AddRoleAsync(_client.Guilds.FirstOrDefault().GetRole(roleId));
        }
    }
}