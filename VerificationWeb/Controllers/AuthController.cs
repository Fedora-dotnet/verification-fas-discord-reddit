using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Discord;
using AspNet.Security.OAuth.Reddit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VerificationWeb.Models;
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
                new AuthenticationProperties {RedirectUri = "/Role/Discord"}, DiscordAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult RedditLogin()
        {
            return Challenge(
                new AuthenticationProperties {RedirectUri = "/Role/Reddit"},
                RedditAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();
            
            if (User.HasClaim(x => x.Issuer == SessionClaims.FedoraScheme))
            {
                SetFedoraClaims();
                return RedirectToAction("Index", "Home");
            }

            if (User.HasClaim(x => x.Issuer == SessionClaims.RedhatScheme))
            {
                SetRedhatClaims();
                return RedirectToAction("Index", "Home");
            }

            return Unauthorized();
        }

        public IActionResult FedoraLogin()
        {
            return Challenge(new AuthenticationProperties {RedirectUri = "/auth"},
                SessionClaims.FedoraScheme);
        }
        public IActionResult RedhatLogin()
        {
            return Challenge(new AuthenticationProperties {RedirectUri = "/auth"},
                SessionClaims.RedhatScheme);
        }

        public IActionResult Logout()
        {
            // Authentication schemes have no log out endpoints, so I just delete all cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
              Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("Index", "Home");
        }
        
        private void SetFedoraClaims()
        {
            StringBuilder allGroups = new StringBuilder();
                
            if (User.HasClaim(x => x.Type == SessionClaims.Cla && x.Value.Contains("done")))
                allGroups.AppendLine("cla/done");

            allGroups.AppendLine(User.Claims.FirstOrDefault(x => x.Type == SessionClaims.Groups)?.Value);

            HttpContext.Session.SetString(SessionClaims.Groups, allGroups.ToString());
            HttpContext.Session.SetString(SessionClaims.Username, User.Claims.FirstOrDefault(x => x.Type == SessionClaims.Username)?.Value);
            HttpContext.Session.SetString(SessionClaims.LoginType, SessionClaims.FedoraScheme);
        }

        private void SetRedhatClaims()
        {
            HttpContext.Session.SetString(SessionClaims.Username, User.Claims.FirstOrDefault(x => x.Type == SessionClaims.Username)?.Value);
            HttpContext.Session.SetString(SessionClaims.LoginType, SessionClaims.RedhatScheme);
        }
    }
}