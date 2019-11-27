using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VerificationWeb.Models;

namespace VerificationWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // TODO
                if (HttpContext.Session.GetString("login_type") == "Redhat")
                {
                    var userModel = new UserModel
                    {
                        Groups = HttpContext.Session.GetString("Groups"),
                        BaseLogintype = HttpContext.Session.GetString("BaseLoginType"),
                        FasNickname = HttpContext.Session.GetString("FasNickname"),
                        RedhatNickname = HttpContext.Session.GetString("RedhatNickname")
                    };
                }
                if(HttpContext.Session.GetString("login_type") == "Fedora")
                {
                    var userModel = new UserModel
                    {
                        Groups = HttpContext.Session.GetString("Groups"),
                        BaseLogintype = HttpContext.Session.GetString("BaseLoginType"),
                        FasNickname = HttpContext.Session.GetString("FasNickname"),
                        RedhatNickname = HttpContext.Session.GetString("RedhatNickname")
                    };
                    return View(userModel);
                }
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