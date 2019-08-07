using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
//    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userModel = new UserModel
                {
                    IsContributor = HttpContext.Session.GetString("IsContributor") == "True",
                    Groups = HttpContext.Session.GetString("Groups"),
//                    DiscordId = HttpContext.Session.GetString("DiscordId"),
                    BaseLogintype = HttpContext.Session.GetString("BaseLoginType"),
                    FasNickname = HttpContext.Session.GetString("FasNickname"),
                    RedhatNickname = HttpContext.Session.GetString("RedhatNickname")
//                    DiscordUsername = HttpContext.Session.GetString("DiscordUsername")
                };
                return View(userModel);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}