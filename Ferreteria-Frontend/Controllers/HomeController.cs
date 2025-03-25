using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ferreteria_Frontend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;

namespace Ferreteria_Frontend.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7214/");
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

    public IActionResult ReporteProductos()
    {
        ViewBag.UsuarioSesion = "YO";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerProductos(string filtro)
    {
        var response = await _httpClient.GetAsync($"ListarProductos?filtro={filtro}");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var productos = JsonConvert.DeserializeObject<List<ProductoViewModel>>(jsonString);
            return Json(new { productos });
        }
        return Json(new { productos = new List<ProductoViewModel>() });
    }
}
