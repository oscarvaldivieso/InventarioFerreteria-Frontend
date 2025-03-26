using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoriasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Categorias";
            ViewBag.SubTitle = "Productos";

            var response = await _httpClient.GetAsync("ListarCategorias");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categorias = JsonConvert.DeserializeObject<IEnumerable<CategoriaViewModel>>(content);
                return View("Index", categorias);
            }
            return View(new List<CategoriaViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoriaViewModel cate)
        {
            cate.Usua_Creacion = 1;
            cate.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(cate);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarCategoria", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Creado Correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la categoria");
                }
            }
            return View(cate);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = new CategoriaViewModel { Cate_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarCategoria", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<CategoriaViewModel> categorias = JsonConvert.DeserializeObject<List<CategoriaViewModel>>(content);

                CategoriaViewModel cate = new CategoriaViewModel();
                foreach (var item in categorias)
                {
                    cate.Cate_Id = item.Cate_Id;
                    cate.Cate_Descripcion = item.Cate_Descripcion;
                }

                return PartialView("_Edit", cate);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoriaViewModel cate)
        {
            cate.Usua_Modificacion = 1;
            cate.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(cate);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("/ActualizarCategoria", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Actualizado Correctamente";
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar la categoria");
                }
            }
            return View(cate);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new CategoriaViewModel { Cate_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/EliminarCategoria", content2);

            if (response.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Eliminado Correctamente";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar la categoria";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = new CategoriaViewModel { Cate_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarCategoria", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<CategoriaViewModel> categorias = JsonConvert.DeserializeObject<List<CategoriaViewModel>>(content);

                CategoriaViewModel cate = new CategoriaViewModel();
                foreach (var item in categorias)
                {
                    cate.Cate_Id = item.Cate_Id;
                    cate.Cate_Descripcion = item.Cate_Descripcion;
                    cate.Feca_Creacion = item.Feca_Creacion;
                    cate.Feca_Modificacion = item.Feca_Modificacion;
                    cate.Usua_Creacion = item.Usua_Creacion;
                    cate.Usua_Modificacion = item.Usua_Modificacion;
                    cate.UsuaC_Nombre = item.UsuaC_Nombre;
                    cate.UsuaM_Nombre = item.UsuaM_Nombre;
                }
                return View(cate);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}