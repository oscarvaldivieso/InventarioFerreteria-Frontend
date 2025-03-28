using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ferreteria_Frontend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using Azure.Identity;

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


    //TODO DEL REPORTE DE PRODUCTOS
    private async Task CargarMarcas()
    {
        var response = await _httpClient.GetAsync("ListarMarcas");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var marcas = JsonConvert.DeserializeObject<IEnumerable<MarcaViewModel>>(content);
            ViewBag.Marc_Id = new SelectList(marcas, "Marc_Id", "Marc_Descripcion");
        }
    }

    public async Task<IActionResult> ReporteProductoAsync()
    {
        try
        {
            ViewBag.PageTitle = "Reporte Productos";
            ViewBag.SubTitle = "Producto";
            ViewBag.UsuarioSesion = "YO";
            await CargarMedidas();
            await CargarMarcas();
            await CargarCategorias();
            await CargarProveedores();

            await ObtenerCategoriasAsync();
            await ObtenerMarcasAsync();
            await ObtenerMedidasAsync();
            await ObtenerProveedoresAsync();
            return View();
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = "Error al mostrar el reporte";
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerProductos(int filtro)
    {
        try
        {
            var data = new { Cate_Id = filtro };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/ProductoPorCategoria", content2);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var productos = JsonConvert.DeserializeObject<List<ProductoViewModel>>(jsonString);
                return Json(new { productos });
            }
            return Json(new { productos = new List<ProductoViewModel>() });
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = "Error al mostrar el reporte";
            return RedirectToAction("Index");
        }
    }

    private async Task CargarCategorias()
    {
        var response = await _httpClient.GetAsync("ListarCategorias");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var categorias = JsonConvert.DeserializeObject<IEnumerable<CategoriaViewModel>>(content);
            ViewBag.Cate_Id = new SelectList(categorias, "Cate_Id", "Cate_Descripcion");
        }
    }

    private async Task CargarMedidas()
    {
        var response = await _httpClient.GetAsync("ListarMedidas");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var medidas = JsonConvert.DeserializeObject<IEnumerable<MedidasViewModel>>(content);
            ViewBag.Medi_Id = new SelectList(medidas, "Medi_Id", "Medi_Descripcion");
        }
    }

    private async Task CargarProveedores()
    {
        var response = await _httpClient.GetAsync("ListarProveedores");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var proveedores = JsonConvert.DeserializeObject<IEnumerable<ProveedoresViewModel>>(content);
            ViewBag.Prov_Id = new SelectList(proveedores, "Prov_Id", "Prov_Nombre");
        }
    }

    public async Task<List<MarcaViewModel>> ObtenerMarcasAsync()
    {
        var response = await _httpClient.GetAsync("ListarMarcas");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MarcaViewModel>>(jsonString);
        }
        return new List<MarcaViewModel>();
    }

    public async Task<List<CategoriaViewModel>> ObtenerCategoriasAsync()
    {
        var response = await _httpClient.GetAsync("ListarCategorias");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CategoriaViewModel>>(jsonString);
        }
        return new List<CategoriaViewModel>();
    }

    public async Task<List<MedidasViewModel>> ObtenerMedidasAsync()
    {
        var response = await _httpClient.GetAsync("ListarMedidas");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<MedidasViewModel>>(jsonString);
        }
        return new List<MedidasViewModel>();
    }

    public async Task<List<ProveedoresViewModel>> ObtenerProveedoresAsync()
    {
        var response = await _httpClient.GetAsync("ListarProveedores");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ProveedoresViewModel>>(jsonString);
        }
        return new List<ProveedoresViewModel>();
    }

    //TODO SOBRE EL REPORTE DE COMPRAS
    public async Task<IActionResult> ReporteCompra()
    {
        try
        {
            ViewBag.PageTitle = "Reporte Compra";
            ViewBag.SubTitle = "Compra";
            ViewBag.UsuarioSesion = "YO";
            await CargarCategorias();
            await CargarProveedores();

            return View();
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = "Error al mostrar el reporte";
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerCompra(DateTime fechaInicio, DateTime fechaFin)
    {
        try
        {
            var data = new { Fecha_Inicio = fechaInicio, Fecha_Fin = fechaFin };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarFecha", content2);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var compras = JsonConvert.DeserializeObject<List<CompraViewModel>>(jsonString);
                return Json(new { compras });
            }
            return Json(new { compras = new List<CompraViewModel>() });
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = "Error al mostrar el reporte";
            return RedirectToAction("Index");
        }
    }
}
