using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedditSharp;
using RedditSharp.Things;
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

            if (!User.Identity.IsAuthenticated || !User.HasClaim(x => x.Issuer == "Discord"))
                return Unauthorized();

            string username = User.Claims.FirstOrDefault(x => x.Type == SessionClaims.Username)?.Value;
            string userId = User.Claims.FirstOrDefault(x => x.Type == SessionClaims.Id)?.Value;

            SaveDiscordClaims(username, userId);

            var loginType = HttpContext.Session.GetString(SessionClaims.LoginType);

            if (loginType == SessionClaims.FedoraScheme)
            {
                string groups = HttpContext.Session.GetString(SessionClaims.Groups);

                var rolesNames = await _roleService.AssignRoleAsync(Convert.ToUInt64(userId), groups);
                ViewData.Add("AddedRoles", rolesNames);
                return View("Discord");
            }

            if (loginType == SessionClaims.RedhatScheme)
            {
                var rolesName = await _roleService.AssignRoleAsync(Convert.ToUInt64(userId), "Redhat");
                ViewData.Add("AddedRoles", rolesName);
                return View("Discord");
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
                    string[] groups = HttpContext.Session.GetString(SessionClaims.Groups).Trim().Split();

                    var availableFlairs = new List<string>();

                    string flair;
                    foreach (var group in groups)
                    {
                        if (_roleService.Config.RedditFlairs.TryGetValue(group, out flair))
                        {
                            availableFlairs.Add(flair);
                        }
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
            if (!User.Identity.IsAuthenticated || !User.HasClaim(x => x.Issuer == "Reddit")) return Unauthorized();

            var username = HttpContext.Session.GetString(SessionClaims.RedditUsername);
            var reddit = new Reddit(_redditWebAgent);
            Subreddit subreddit;
            string flairCss;
            // validate the parameter from spoofing
            if (HttpContext.Session.GetString(SessionClaims.LoginType) == SessionClaims.RedhatScheme)
            {
                if (_roleService.Config.RedditFlairs["Redhat"] != flair)
                    return Unauthorized("Nice try");

                subreddit = await reddit.GetSubredditAsync(_roleService.Config.RedhatSubreddit);
                flairCss = _roleService.Config.RedhatFlairCss;
            }
            else
            {
                string[] groups = HttpContext.Session.GetString(SessionClaims.Groups).Trim().Split();

                if (!_roleService.Config.RedditFlairs.Any(x => groups.Contains(x.Key) && x.Value == flair))
                    return Unauthorized("Nice try");

                subreddit = await reddit.GetSubredditAsync(_roleService.Config.Subreddit);
                flairCss = _roleService.Config.FedoraFlairCss;
            }

            if (subreddit != null)
                await subreddit.SetUserFlairAsync(username, flairCss, flair);

            // Check for error TODO (look into the lib source for possible error states)
            return View("Success");
        }

        private void SaveDiscordClaims(string name, string id)
        {
            HttpContext.Session.SetString(SessionClaims.DiscordUsername, name);
            HttpContext.Session.SetString(SessionClaims.DiscordId, id);
        }
    }
}
