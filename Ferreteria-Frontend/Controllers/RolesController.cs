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

        public IActionResult Create()
        {
            ViewBag.PageTitle = "Crear un rol";
            ViewBag.SubTitle = "Acceso";
            return View();
        }
    }
}
