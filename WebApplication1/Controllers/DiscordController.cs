using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class DiscordController : Controller
    {
        private readonly RoleService _roleService;

        public DiscordController(RoleService roleService)
        {
            _roleService = roleService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.Session.SetString("UserId", User.Claims.FirstOrDefault(x => x.Type == "id")?.Value);
                return RedirectToAction("Fedora", "Home");
            }
            else return Unauthorized();
        }

        [AllowAnonymous]
        public IActionResult Discord()
        {
            return Challenge(
                new AuthenticationProperties {RedirectUri = "/Discord"}, DiscordDefaults.AuthenticationScheme);
        }

        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetRole()
        {
            var id = HttpContext.Session.GetString("UserId");
            if (id == null)
                return Challenge(
                    new AuthenticationProperties {RedirectUri = "/Discord"}, DiscordDefaults.AuthenticationScheme);

            await _roleService.AssignRoleAsync(Convert.ToUInt64(id));
            return Ok("Successfully added");
        }
    }
}