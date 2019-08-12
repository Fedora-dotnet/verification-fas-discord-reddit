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
            if (User.Identity.IsAuthenticated)
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

//                        await AddRedditFlair(redditName, "Contributor");
//                        addedRoles.Add("Contributor");
//                    
//                    var addedRoles = new List<string>();
//                    if (isContributor == "True")
//                    {
//                    }

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
            // validate the parameter from spoofing
            if (!_roleService.Config.RedditFlairs.ContainsValue(flair))
            {
                return Unauthorized("Nice try");
            }

            var username = HttpContext.Session.GetString("RedditUsername");
            var subreddit = await _redditService.GetSubredditAsync(_roleService.Config.Subreddit);
            await subreddit.SetUserFlairAsync(username, "", flair);
            return Ok("Sucessfully added");
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