using Microsoft.AspNetCore.Mvc;

namespace Ferreteria_Frontend.Controllers
{
    public class RolesController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.PageTitle = "Roles";
            ViewBag.SubTitle = "Acceso";
            return View();
        }
    }
}
