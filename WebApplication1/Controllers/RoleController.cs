using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        public IActionResult Reddit()
        {
            return Unauthorized();
        }

        public async Task<IActionResult> Discord()
        {
            // if user successfully authed at discord
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.Session.SetString("DiscordId", User.Claims.FirstOrDefault(x => x.Type == "id")?.Value);
                HttpContext.Session.SetString("DiscordUsername",
                    User.Claims.FirstOrDefault(x => x.Type == "username")?.Value);
                string userid = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
                string isContributor = HttpContext.Session.GetString("IsContributor");
                var loginType = HttpContext.Session.GetString("BaseLoginType");

                if (loginType == "Fedora")
                {
                    var roleConditions = new List<string>();

                    var roles = HttpContext.Session.GetString("Groups").Split();
                    roleConditions.AddRange(roles);

                    var rolesName = new List<string>();

                    if (isContributor == "True")
                    {
                        rolesName.Add("Contributor");
                    }

                    foreach (var x in roleConditions)
                    {
                        rolesName.Add(_roleService.Config.RolesConditions[x]);
                    }

                    await AddDiscordRoles(rolesName);

                    ViewData.Add("AddedRoles", rolesName);
                    return View("Discord");
                }
                else if (loginType == "RedHat")
                {
                }
            }

            return Unauthorized();
        }

        public IActionResult DiscordLogin()
        {
            return Challenge(
                new AuthenticationProperties {RedirectUri = "/Role/Discord"}, DiscordDefaults.AuthenticationScheme);
        }

        public IActionResult RedditLogin()
        {
            return Unauthorized();
        }

        public async Task AddDiscordRoles(IEnumerable<string> roles)
        {
            // TODO Hide it from url access 
            var userId = HttpContext.Session.GetString("DiscordId");
            await _roleService.AssignRoleAsync(Convert.ToUInt64(userId), roles);
        }
    }
}