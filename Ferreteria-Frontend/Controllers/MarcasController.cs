using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Execution;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class MarcasController : Controller
    {
        private readonly HttpClient _httpClient;

        public MarcasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Marcas";
            ViewBag.SubTitle = "Productos";

            var response = await _httpClient.GetAsync("ListarMarcas");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var marcas = JsonConvert.DeserializeObject<IEnumerable<MarcaViewModel>>(content);
                return View("Index", marcas);
            }
            return View(new List<MarcaViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MarcaViewModel marc)
        {
            marc.Usua_Creacion = 1;
            marc.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(marc);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarMarca", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la marca");
                }
            }
            return View(marc);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = new MarcaViewModel { Marc_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarMarca", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<MarcaViewModel> marcas = JsonConvert.DeserializeObject<List<MarcaViewModel>>(content);

                MarcaViewModel marc = new MarcaViewModel();
                foreach (var item in marcas)
                {
                    marc.Marc_Id = item.Marc_Id;
                    marc.Marc_Descripcion = item.Marc_Descripcion;
                }
                return PartialView("_Edit", marc);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MarcaViewModel marc)
        {
            marc.Usua_Modificacion = 1;
            marc.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(marc);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarMarca", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar la marca");
                }
            }
            return View(marc);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new MarcaViewModel { Marc_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("EliminarMarca", content2);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el cargo";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = new CargoViewModel { Carg_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarMarca", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<MarcaViewModel> marcas = JsonConvert.DeserializeObject<List<MarcaViewModel>>(content);

                MarcaViewModel marc = new MarcaViewModel();
                foreach (var item in marcas)
                {
                    marc.Marc_Id = item.Marc_Id;
                    marc.Marc_Descripcion = item.Marc_Descripcion;
                    marc.Feca_Creacion = item.Feca_Creacion;
                    marc.Feca_Modificacion = item.Feca_Modificacion;
                    marc.Usua_Creacion = item.Usua_Creacion;
                    marc.Usua_Modificacion = item.Usua_Modificacion;
                    marc.UsuaC_Nombre = item.UsuaC_Nombre;
                    marc.UsuaM_Nombre = item.UsuaM_Nombre;
                }
                return View(marc);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}