using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class DepartamentosController : Controller
    {
        private readonly HttpClient _httpClient;

        public DepartamentosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Departamentos";

            var response = await _httpClient.GetAsync("ListarDepartamentos");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var departamentos = JsonConvert.DeserializeObject<IEnumerable<DepartamentoViewModel>>(content);
                return View("Index", departamentos);
            }
            return View(new List<DepartamentoViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartamentoViewModel depa)
        {
            depa.Usua_Creacion = 1;
            depa.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(depa);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarDepartamento", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el departamento");
                }
            }
            return View(depa);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var data = new DepartamentoViewModel { Depa_Codigo = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarDepartamento", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<DepartamentoViewModel> departa = JsonConvert.DeserializeObject<List<DepartamentoViewModel>>(content);

                DepartamentoViewModel depa = new DepartamentoViewModel();
                foreach (var item in departa)
                {
                    depa.Depa_Codigo = item.Depa_Codigo;
                    depa.Depa_Descripcion = item.Depa_Descripcion;
                }

                return PartialView("_Edit", depa);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, DepartamentoViewModel depa)
        {
            depa.Usua_Modificacion = 1;
            depa.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(depa);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarDepartamento", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar departamento");
                }
            }
            return View(depa);
        }

        public async Task<IActionResult> Delete (string id)
        {
            var data = new DepartamentoViewModel { Depa_Codigo = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("EliminarDepartamento",content2 );

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el departamento";
                return RedirectToAction("Index");
            }
        }
    }
}