using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Services
{
    public class RoleService
    {
        private readonly SocketGuild _guild;
        private Dictionary<DiscordRoles, IRole> _rolesDictionary = new Dictionary<DiscordRoles, IRole>();

        public enum DiscordRoles
        {
            Contributor,
            Dotnet
        }

        public RoleService(IConfiguration configuration, DiscordSocketClient client)
        {
            _guild = client.Guilds.FirstOrDefault(x => x.Id == Convert.ToUInt64(configuration["GuildId"]));
            _rolesDictionary.Add(DiscordRoles.Contributor,
                _guild?.GetRole(Convert.ToUInt64(configuration["ContributorId"])));
            _rolesDictionary.Add(DiscordRoles.Dotnet, _guild?.GetRole(Convert.ToUInt64(configuration["DotnetId"])));
        }

        public async Task AssignRoleAsync(ulong userId, DiscordRoles role)
            => await _guild.GetUser(userId).AddRoleAsync(_rolesDictionary[role]);
    }
}