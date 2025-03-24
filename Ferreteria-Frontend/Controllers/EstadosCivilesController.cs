using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class EstadosCivilesController : Controller
    {
        private readonly HttpClient _httpClient;

        public EstadosCivilesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Estados Civiles";
            ViewBag.SubTitle = "General";

            var response = await _httpClient.GetAsync("ListarEstadosCiviles");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var estadosciviles = JsonConvert.DeserializeObject<IEnumerable<EstadoCivilViewModel>>(content);
                return View("Index", estadosciviles);
            }
            return View(new List<EstadoCivilViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EstadoCivilViewModel escv)
        {
            escv.Usua_Creacion = 1;
            escv.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(escv);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarEstadoCivil", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el estado civil");
                }
            }
            return View(escv);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = new EstadoCivilViewModel { EsCv_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarEstadoCivil", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<EstadoCivilViewModel> estadosciviles = JsonConvert.DeserializeObject<List<EstadoCivilViewModel>>(content);

                EstadoCivilViewModel escv = new EstadoCivilViewModel();
                foreach (var item in estadosciviles)
                {
                    escv.EsCv_Id = item.EsCv_Id;
                    escv.EsCv_Descripcion = item.EsCv_Descripcion;
                }

                return PartialView("_Edit", escv);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EstadoCivilViewModel escv)
        {
            escv.Usua_Modificacion = 1;
            escv.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(escv);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarEstadoCivil", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el estado civil");
                }
            }
            return View(escv);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new EstadoCivilViewModel { EsCv_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/EliminarEstadoCivil", content2);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el estado civil";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = new EstadoCivilViewModel { EsCv_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarEstadoCivil", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<EstadoCivilViewModel> estadociviles = JsonConvert.DeserializeObject<List<EstadoCivilViewModel>>(content);

                EstadoCivilViewModel escv = new EstadoCivilViewModel();
                foreach (var item in estadociviles)
                {
                    escv.EsCv_Id = item.EsCv_Id;
                    escv.EsCv_Descripcion = item.EsCv_Descripcion;
                    escv.Feca_Creacion = item.Feca_Creacion;
                    escv.Feca_Modificacion = item.Feca_Modificacion;
                    escv.Usua_Creacion = item.Usua_Creacion;
                    escv.Usua_Modificacion = item.Usua_Modificacion;
                    escv.UsuaC_Nombre = item.UsuaC_Nombre;
                    escv.UsuaM_Nombre = item.UsuaM_Nombre;
                }
                return View(escv);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}
