using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ferreteria_Frontend.Models;

namespace Ferreteria_Frontend.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewBag.PageTitle = "Página Principal";
        ViewBag.SubTitle = "Bienvenido";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ReporteProducto()
    {
        return View();
    }
}
