using System.Linq;
using AspNet.Security.OAuth.Reddit;
using Discord.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
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
                    string isContributor;
                    if (User.HasClaim(x => x.Type == "cla" && x.Value.Contains("done")) &&
                        User.HasClaim(x => x.Type == "cla" && x.Value.Contains("fpca")))
                        isContributor = "True";
                    else isContributor = "False";

                    HttpContext.Session.SetString("IsContributor", isContributor);
                    HttpContext.Session.SetString("Groups", User.Claims.FirstOrDefault(x => x.Type == "groups")?.Value);
                    HttpContext.Session.SetString("FasNickname",
                        User.Claims.FirstOrDefault(x => x.Type == "nickname")?.Value);
                    HttpContext.Session.SetString("BaseLoginType", "Fedora");
                    return RedirectToAction("Index", "Home");
                }

                if (User.HasClaim(x => x.Issuer == "Discord"))
                {
                    HttpContext.Session.SetString("DiscordId", User.Claims.FirstOrDefault(x => x.Type == "id")?.Value);
                    HttpContext.Session.SetString("DiscordUsername",
                        User.Claims.FirstOrDefault(x => x.Type == "username")?.Value);
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