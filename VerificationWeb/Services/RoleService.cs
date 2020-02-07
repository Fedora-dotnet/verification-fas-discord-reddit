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
        private readonly DiscordSocketClient _client;
        public readonly Config Config;

        public RoleService(Config config, DiscordSocketClient client)
        {
            Config = config;
            _client = client;
        }

        // Returns name of the roles
        public async Task<List<string>> AssignRoleAsync(ulong userId, string groups)
        {
            var roleNames = new List<string>();
            var rolesToAdd = new List<IRole>();

            foreach (var guild in _client.Guilds)
            {
                
                if (!guild.Users.Any(x => x.Id == userId))
                    break;

                foreach (var role in guild.Roles)
                {
                    if (groups.Contains("cla/done"))
                    {
                        if (Config.ContributorRoles.Contains(role.Id))
                        {
                            var newRole = guild.GetRole(role.Id);
                            rolesToAdd.Add(newRole);
                            roleNames.Add(newRole.Name);
                        }
                    }

                    if (groups.Contains("dotnet-team"))
                    {
                        if (Config.DotnetRoles.Contains(role.Id))
                        {
                            var newRole = guild.GetRole(role.Id);
                            rolesToAdd.Add(newRole);
                            roleNames.Add(newRole.Name);
                        }
                    }

                    if (groups.Contains("Redhat"))
                    {
                        if (Config.RedhatRoles.Contains(role.Id))
                        {
                            var newRole = guild.GetRole(role.Id);
                            rolesToAdd.Add(newRole);
                            roleNames.Add(newRole.Name);
                        }
                    }
                }

                await guild.GetUser(userId).AddRolesAsync(rolesToAdd);
                rolesToAdd.Clear();
            }

            return roleNames;
        }
    }
}