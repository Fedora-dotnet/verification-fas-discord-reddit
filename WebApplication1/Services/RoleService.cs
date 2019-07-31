using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Services
{
    public class RoleService
    {
        private readonly ulong _userid; // HoundThe
        private readonly SocketGuild _guild;
        private readonly IRole _contributorRole;

        public RoleService(IConfiguration configuration, DiscordSocketClient client)
        {
            _userid = 333985625155960837;

            var guildId = Convert.ToUInt64(configuration["GuildId"]);
            var roleId = Convert.ToUInt64(configuration["ContributorId"]);
            _guild = client.Guilds.FirstOrDefault(x => x.Id == guildId);
            _contributorRole = _guild?.GetRole(roleId);
        }

        public async Task AssignRoleAsync(ulong userId) => await _guild.GetUser(userId).AddRoleAsync(_contributorRole);
    }
}