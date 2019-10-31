using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedditSharp;
using VerificationWeb.Services;

namespace VerificationWeb.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleService _roleService;
        private readonly RefreshTokenWebAgent _redditWebAgent;


        public RoleController(RoleService roleService, RefreshTokenWebAgent redditWebAgent)
        {
        _roleService = roleService;
        _redditWebAgent = redditWebAgent;
        }

        public async Task<IActionResult> Discord()
        {
            // user can spam reload on the action and trigger the rate limit,

            // if user successfully authed at discord
            if (User.Identity.IsAuthenticated && User.HasClaim(x => x.Issuer == "Discord"))
            {
                string username = User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
                string userId = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
                SaveDiscordClaims(username, userId);
                var loginType = HttpContext.Session.GetString("BaseLoginType");

                if (loginType == "Fedora")
                {
                    GetFedoraClaims(out string name, out string groups);
                    var roleConditions = new List<string>();

                    var roles = groups.Split();
                    roleConditions.AddRange(roles);

                    var rolesName = new List<string>();

                    foreach (var x in roleConditions)
                    {
                        // Check if the appropriate role exists so we wont get exceptions
                        if (_roleService.Config.RolesConditions.ContainsKey(x))
                            rolesName.Add(_roleService.Config.RolesConditions[x]);
                    }

                    await AddDiscordRoles(rolesName, Convert.ToUInt64(userId));

                    // weakly typed, for now?
                    ViewData.Add("AddedRoles", rolesName);
                    return View("Discord");
                }
                else if (loginType == "RedHat")
                {
                }
            }

            return Unauthorized();
        }

        public IActionResult Reddit()
        {
            // if user successfully authed at discord
            if (User.Identity.IsAuthenticated && User.HasClaim(x => x.Issuer == "Reddit"))
            {
                string redditName = User.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
                HttpContext.Session.SetString("RedditUsername", redditName);

                var loginType = HttpContext.Session.GetString("BaseLoginType");

                if (loginType == "Fedora")
                {
                    GetFedoraClaims(out string name, out string groups);

                    var roleConditions = new List<string>();

                    var roles = groups.Split();
                    roleConditions.AddRange(roles);

                    var rolesName = new List<string>();

                    foreach (var x in roleConditions)
                    {
                        // Check if the appropriate role exists so we wont get exceptions
                        if (_roleService.Config.RedditFlairs.ContainsKey(x))
                            rolesName.Add(_roleService.Config.RedditFlairs[x]);
                    }

                    ViewData.Add("roles", rolesName);
                    return View("Reddit");
                }
                else if (loginType == "RedHat")
                {
                }
            }

            return Unauthorized();
        }

        public async Task<IActionResult> GetRedditFlair(string flair)
        {
            if (User.Identity.IsAuthenticated && User.HasClaim(x => x.Issuer == "Reddit"))
            {
                // validate the parameter from spoofing
                if (!_roleService.Config.RedditFlairs.ContainsValue(flair))
                {
                    return Unauthorized("Nice try");
                }

                var username = HttpContext.Session.GetString("RedditUsername");
                var reddit = new Reddit(_redditWebAgent, true);
                var subreddit = await reddit.GetSubredditAsync(_roleService.Config.Subreddit);
                await subreddit.SetUserFlairAsync(username, "contributorFlair", flair);

                return View("Success");
            }
            return Unauthorized("Nice try");
        }

        public async Task AddDiscordRoles(IEnumerable<string> roles, ulong userId)
        {
            // TODO Hide it from url access 
            await _roleService.AssignRoleAsync(Convert.ToUInt64(userId), roles);
        }

        public void SaveDiscordClaims(string name, string id)
        {
            HttpContext.Session.SetString("DiscordUsername", name);
            HttpContext.Session.SetString("DiscordId", id);
        }

        public void GetFedoraClaims(out string name, out string groups)
        {
            name = HttpContext.Session.GetString("FasNickname");
            groups = HttpContext.Session.GetString("Groups");
        }
    }
}