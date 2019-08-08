using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedditSharp;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleService _roleService;
        private readonly Reddit _redditService;

        public RoleController(RoleService roleService, Reddit redditService)
        {
            _roleService = roleService;
            _redditService = redditService;
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
                string redditName = User.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
                HttpContext.Session.SetString("RedditUsername", redditName);

                var loginType = HttpContext.Session.GetString("BaseLoginType");

                if (loginType == "Fedora")
                {
                    string isContributor = HttpContext.Session.GetString("IsContributor");
                    var addedRoles = new List<string>();
                    if (isContributor == "True")
                    {
                        await AddRedditFlair(redditName, "Contributor");
                        addedRoles.Add("Contributor");
                    }

                    ViewData.Add("roles", addedRoles);
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
            var subreddit = await _redditService.GetSubredditAsync("/r/fedora_verification");
            await subreddit.SetUserFlairAsync("hound_the", "", flair);
        }

        public async Task AddDiscordRoles(IEnumerable<string> roles, ulong userId)
        {
            // TODO Hide it from url access 
            await _roleService.AssignRoleAsync(Convert.ToUInt64(userId), roles);
        }
    }
}