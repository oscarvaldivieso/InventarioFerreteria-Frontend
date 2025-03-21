using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class CargosController : Controller
    {
        private readonly HttpClient _httpClient;

        public CargosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Cargos";

            var response = await _httpClient.GetAsync("ListarCargos");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var cargos = JsonConvert.DeserializeObject<IEnumerable<CargoViewModel>>(content);
                return View("Index", cargos);
            }
            return View(new List<CargoViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CargoViewModel carg)
        {
            carg.Usua_Creacion = 1;
            carg.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(carg);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarCargo", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el cargo");
                }
            }
            return View(carg);
        }

        public async Task<IActionResult> Edit(int id)
        {

            var data = new CargoViewModel { Carg_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarCargo", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<CargoViewModel> cargos = JsonConvert.DeserializeObject<List<CargoViewModel>>(content);

                CargoViewModel carg = new CargoViewModel();
                foreach (var item in cargos)
                {
                    carg.Carg_Id = item.Carg_Id;
                    carg.Carg_Descripcion = item.Carg_Descripcion;


                }

                return PartialView("_Edit", carg);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CargoViewModel carg)
        {
            carg.Usua_Modificacion = 1;
            carg.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(carg);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarCargo", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el cargo");
                }
            }
            return View(carg);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new CargoViewModel { Carg_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/EliminarCargo", content2);

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
            var response = await _httpClient.PostAsync("/BuscarCargo", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<CargoViewModel> cargos = JsonConvert.DeserializeObject<List<CargoViewModel>>(content);

                CargoViewModel carg = new CargoViewModel();
                foreach (var item in cargos)
                {
                    carg.Carg_Id = item.Carg_Id;
                    carg.Carg_Descripcion = item.Carg_Descripcion;
                    carg.Feca_Creacion = item.Feca_Creacion;
                    carg.Feca_Modificacion = item.Feca_Modificacion;
                    carg.Usua_Creacion = item.Usua_Creacion;
                    carg.Usua_Modificacion = item.Usua_Modificacion;
                    carg.UsuaC_Nombre = item.UsuaC_Nombre;
                    carg.UsuaM_Nombre = item.UsuaM_Nombre;
                }
                return View(carg);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}
