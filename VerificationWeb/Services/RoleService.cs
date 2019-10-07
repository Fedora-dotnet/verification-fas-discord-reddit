using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using VerificationWeb.Configuration;

namespace VerificationWeb.Services
{
    public class RoleService
    {
        private readonly SocketGuild _guild;
        public readonly Config Config;

        public RoleService(Config config, DiscordSocketClient client)
        {
            Config = config;
            _guild = client.Guilds.FirstOrDefault(x => x.Id == Config.GuildId);
        }

        public async Task AssignRoleAsync(ulong userId, IEnumerable<string> roleNames)
        {
            var rolesToAdd = new List<IRole>();
                foreach (string roleName in roleNames)
                {
                    var roleId = Config.DiscordRoles[roleName];
                    rolesToAdd.Add(_guild.GetRole(roleId));
                }

            await _guild.GetUser(userId).AddRolesAsync(rolesToAdd);
        }
    }
}