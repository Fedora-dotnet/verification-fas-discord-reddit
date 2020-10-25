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

            bool isRedhat = groups.Contains("Redhat");
            bool isContributor = groups.Contains("cla/done");
            bool isDotnet = groups.Contains("dotnet-team");
            
            // Maybe TODO, prepend name of the guild to the role name so the user can see where he got the roles?
            
            foreach (IGuild guild in _client.Guilds)
            {
                var user = await guild.GetUserAsync(userId); 
                if (user == null)
                    continue;

                if (isRedhat)
                {
                    foreach (var roleId in Config.RedhatRoles)
                    {
                        var newRole = guild.GetRole(roleId);
                        if (newRole != null)
                        {
                            rolesToAdd.Add(newRole);
                            roleNames.Add($"{guild.Name} - {newRole.Name}");
                            // Only 1 Redhat role per guild? Or is there reason for more roles to exist?
                            break;
                        }
                    }
                }
                else
                {
                    if (isContributor)
                    {
                        foreach (var roleId in Config.ContributorRoles)
                        {
                            var newRole = guild.GetRole(roleId);
                            if (newRole != null)
                            {
                                rolesToAdd.Add(newRole);
                                roleNames.Add($"{guild.Name} - {newRole.Name}");
                                break;
                            }
                        }
                    }

                    if (isDotnet)
                    {
                        foreach (var roleId in Config.DotnetRoles)
                        {
                            var newRole = guild.GetRole(roleId);
                            if (newRole != null)
                            {
                                rolesToAdd.Add(newRole);
                                roleNames.Add($"{guild.Name} - {newRole.Name}");
                                break;
                            }
                        }
                    }
                }
                
                await user.AddRolesAsync(rolesToAdd);
                rolesToAdd.Clear();
            }

            return roleNames;
        }
    }
}