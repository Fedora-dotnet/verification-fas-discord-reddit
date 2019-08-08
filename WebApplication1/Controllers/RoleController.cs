using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Discord()
        {
            // if user successfully authed at discord
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.Session.SetString("DiscordUsername",
                    User.Claims.FirstOrDefault(x => x.Type == "username")?.Value);
                string userId = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
                HttpContext.Session.SetString("DiscordId", userId);
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

                    await AddDiscordRoles(rolesName, Convert.ToUInt64(userId));

                    ViewData.Add("AddedRoles", rolesName);
                    return View("Discord");
                }
                else if (loginType == "RedHat")
                {
                }
            }

            return Unauthorized();
        }

        public async Task<IActionResult> Reddit()
        {
            // if user successfully authed at discord
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.Session.SetString("RedditUsername",
                    User.Claims.FirstOrDefault(x => x.Type == "name")?.Value);
                string userId = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
                HttpContext.Session.SetString("RedditId", userId);

                var loginType = HttpContext.Session.GetString("BaseLoginType");

                if (loginType == "Fedora")
                {
                    return View("Reddit");
                }
                else if (loginType == "RedHat")
                {
                }
            }

            return Unauthorized();
        }

        public async Task AddRedditFlair(string username, string flair)
        {
            throw new NotImplementedException();
        }

        public async Task AddDiscordRoles(IEnumerable<string> roles, ulong userId)
        {
            // TODO Hide it from url access 
            await _roleService.AssignRoleAsync(Convert.ToUInt64(userId), roles);
        }
    }
}