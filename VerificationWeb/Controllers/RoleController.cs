using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RedditSharp;
using VerificationWeb.Models;
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
            // watchout user can spam reload on the action and trigger the rate limit,

            if (!User.Identity.IsAuthenticated || !User.HasClaim(x => x.Issuer == "Discord")) return Unauthorized();
            {
                string username = User.Claims.FirstOrDefault(x => x.Type == SessionClaims.Username)?.Value;
                string userId = User.Claims.FirstOrDefault(x => x.Type == SessionClaims.Id)?.Value;
                
                SaveDiscordClaims(username, userId);
                
                var loginType = HttpContext.Session.GetString(SessionClaims.LoginType);

                if (loginType == SessionClaims.FedoraScheme)
                {
                    string[] groups = HttpContext.Session.GetString(SessionClaims.Groups).Split();

                    var availableRoles = new List<string>();

                    string flair;
                    foreach (var group in groups)
                    {
                        if(_roleService.Config.RedditFlairs.TryGetValue(group, out flair));
                            availableRoles.Add(flair);
                    }

                    await _roleService.AssignRoleAsync(Convert.ToUInt64(userId), availableRoles);
                    ViewData.Add("AddedRoles", availableRoles);
                    return View("Discord");
                }
                
                if (loginType == SessionClaims.RedhatScheme)
                {
                    var rolesName = new List<string> {"Redhat"};
                    await _roleService.AssignRoleAsync(Convert.ToUInt64(userId), rolesName);
                    ViewData.Add("AddedRoles", rolesName);
                    return View("Discord");
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
                HttpContext.Session.SetString(SessionClaims.RedditUsername, redditName);

                var loginType = HttpContext.Session.GetString(SessionClaims.LoginType);

                if (loginType == SessionClaims.FedoraScheme)
                {
                    string[] groups = HttpContext.Session.GetString(SessionClaims.Groups).Split();

                    var availableFlairs = new List<string>();

                    string flair;
                    foreach (var group in groups)
                    {
                        if(_roleService.Config.RedditFlairs.TryGetValue(group, out flair));
                              availableFlairs.Add(flair);
                    }

                    ViewData.Add("roles", availableFlairs);
                    return View("Reddit");
                }
                
                if (loginType == SessionClaims.RedhatScheme)
                {
                    var rolesName = new List<string>();
                    rolesName.Add(_roleService.Config.RedditFlairs["Redhat"]);
                    ViewData.Add("roles", rolesName);
                    return View("Reddit");
                }
            }
            return Unauthorized();
        }

        public async Task<IActionResult> GetRedditFlair(string flair)
        {
            if (User.Identity.IsAuthenticated && User.HasClaim(x => x.Issuer == "Reddit"))
            {
                // validate the parameter from spoofing
                if (HttpContext.Session.GetString(SessionClaims.LoginType) == SessionClaims.RedhatScheme)
                {
                    if (_roleService.Config.RedditFlairs["Redhat"] != flair) return Unauthorized("Nice try");
                }
                else if (HttpContext.Session.GetString(SessionClaims.LoginType) == SessionClaims.FedoraScheme)
                {
                    // TODO
                }
            }

            var username = HttpContext.Session.GetString(SessionClaims.RedditUsername);
            var reddit = new Reddit(_redditWebAgent);
            var subreddit = await reddit.GetSubredditAsync(_roleService.Config.Subreddit);
            await subreddit.SetUserFlairAsync(username, "contributorFlair", flair);

            return View("Success");
        }

        private void SaveDiscordClaims(string name, string id)
        {
            HttpContext.Session.SetString(SessionClaims.DiscordUsername, name);
            HttpContext.Session.SetString(SessionClaims.DiscordId, id);
        }
    }
}