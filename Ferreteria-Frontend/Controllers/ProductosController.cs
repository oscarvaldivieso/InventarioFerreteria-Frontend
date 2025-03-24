using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class ProductosController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Productos";
            ViewBag.SubTitle = "Producto";

            var response = await _httpClient.GetAsync("ListarProductos");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var productos = JsonConvert.DeserializeObject<IEnumerable<ProductoViewModel>>(content);
                return View("Index", productos);
            }
            return View(new List<ProductoViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoViewModel prod)
        {
            prod.Usua_Creacion = 1;
            prod.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(prod);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarProducto", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el producto");
                }
            }
            return View(prod);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = new ProductoViewModel { Prod_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarProducto", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<ProductoViewModel> productos = JsonConvert.DeserializeObject<List<ProductoViewModel>>(content);

                ProductoViewModel prod = new ProductoViewModel();
                foreach (var item in productos)
                {
                    prod.Prod_Id = item.Prod_Id;
                    prod.Prod_Descripcion = item.Prod_Descripcion;
                    prod.Marc_Id = item.Marc_Id;
                    prod.Cate_Id = item.Cate_Id;
                    prod.Prov_Id = item.Prov_Id;
                    prod.Prod_Modelo = item.Prod_Modelo;
                    prod.Prod_Cantidad = item.Prod_Cantidad;
                    prod.Prod_URLImg = item.Prod_URLImg;
                    prod.Medi_Id = item.Medi_Id;
                }

                return PartialView("Edit", prod);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductoViewModel prod)
        {
            prod.Usua_Modificacion = 1;
            prod.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(prod);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarProducto", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el producto");
                }
            }
            return View(prod);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new ProductoViewModel { Prod_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/EliminarProducto", content2);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el producto";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = new ProductoViewModel { Prod_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarProducto", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<ProductoViewModel> productos = JsonConvert.DeserializeObject<List<ProductoViewModel>>(content);

                ProductoViewModel prod = new ProductoViewModel();
                foreach (var item in productos)
                {
                    prod.Prod_Id = item.Prod_Id;
                    prod.Prod_Descripcion = item.Prod_Descripcion;
                    prod.Marc_Id = item.Marc_Id;
                    prod.Cate_Id = item.Cate_Id;
                    prod.Prov_Id = item.Prov_Id;
                    prod.Prod_Modelo = item.Prod_Modelo;
                    prod.Prod_Cantidad = item.Prod_Cantidad;
                    prod.Prod_URLImg = item.Prod_URLImg;
                    prod.Medi_Id = item.Medi_Id;
                    prod.Feca_Creacion = item.Feca_Creacion;
                    prod.Feca_Modificacion = item.Feca_Modificacion;
                    prod.Usua_Creacion = item.Usua_Creacion;
                    prod.Usua_Modificacion = item.Usua_Modificacion;
                    prod.UsuaC_Nombre = item.UsuaC_Nombre;
                    prod.UsuaM_Nombre = item.UsuaM_Nombre;
                }
                return View(prod);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}
