using System.Linq;
using AspNet.Security.OAuth.Reddit;
using Discord.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VerificationWeb.Services;

namespace VerificationWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly RoleService _roleService;

        public AuthController(RoleService roleService)
        {
            _roleService = roleService;
        }

        public IActionResult DiscordLogin()
        {
            return Challenge(
                new AuthenticationProperties {RedirectUri = "/Role/Discord"}, DiscordDefaults.AuthenticationScheme);
        }

        public IActionResult RedditLogin()
        {
            return Challenge(
                new AuthenticationProperties {RedirectUri = "/Role/Reddit"},
                RedditAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // maybe unhardcode this?
                if (User.HasClaim(x => x.Issuer == "OpenIdConnect"))
                {
                    string groups = "";
                    if (User.HasClaim(x => x.Type == "cla" && x.Value.Contains("done")))
                    {
                        groups += "cla/done ";
                    }

                    // string builder might be better
                    groups += User.Claims.FirstOrDefault(x => x.Type == "groups")?.Value;

                    HttpContext.Session.SetString("Groups", groups);
                    HttpContext.Session.SetString("FasNickname",
                        User.Claims.FirstOrDefault(x => x.Type == "nickname")?.Value);
                    HttpContext.Session.SetString("BaseLoginType", "Fedora");
                    return RedirectToAction("Index", "Home");
                }
            }

            return Unauthorized();
        }

        public IActionResult FedoraLogin()
        {
            return Challenge(new AuthenticationProperties {RedirectUri = "/auth"},
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}