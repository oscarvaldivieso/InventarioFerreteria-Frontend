using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class MunicipiosController : Controller
    {
        private readonly HttpClient _httpClient;

        public MunicipiosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }
        private async Task CargarDepartamentos()
        {
            var response = await _httpClient.GetAsync("ListarDepartamentos");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var departamentos = JsonConvert.DeserializeObject<IEnumerable<DepartamentoViewModel>>(content);
                ViewBag.Depa_Codigo = new SelectList(departamentos, "Depa_Codigo", "Depa_Descripcion");
            }
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Municipios";
            ViewBag.SubTitle = "General";

            var response = await _httpClient.GetAsync("ListarMunicipios");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var municipios = JsonConvert.DeserializeObject<IEnumerable<MunicipioViewModel>>(content);
                return View("Index", municipios);
            }
            return View(new List<MunicipioViewModel>());
        }

        public async Task<IActionResult> Create()
        {
            await CargarDepartamentos();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MunicipioViewModel muni)
        {
            muni.Usua_Creacion = 1;
            muni.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(muni);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarMunicipio", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el municipio");
                }
            }
            return View(muni);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var data = new MunicipioViewModel { Muni_Codigo = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarMunicipio", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<MunicipioViewModel> munic = JsonConvert.DeserializeObject<List<MunicipioViewModel>>(content);

                MunicipioViewModel muni = new MunicipioViewModel();
                foreach (var item in munic)
                {
                    muni.Muni_Codigo = item.Muni_Codigo;
                    muni.Muni_Descripcion = item.Muni_Descripcion;
                    muni.Depa_Codigo = item.Depa_Codigo;
                }
                await CargarDepartamentos();
                return PartialView("_Edit", muni);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, MunicipioViewModel muni)
        {
            muni.Usua_Modificacion = 1;
            muni.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(muni);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarMunicipio", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el municipio");
                }
            }

            return View(muni);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var data = new MunicipioViewModel { Muni_Codigo = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("EliminarMunicipio", content2);

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

        public async Task<IActionResult> Details(string id)
        {
            var data = new MunicipioViewModel { Muni_Codigo = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarMunicipio", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<MunicipioViewModel> municip = JsonConvert.DeserializeObject<List<MunicipioViewModel>>(content);

                MunicipioViewModel muni = new MunicipioViewModel();
                foreach (var item in municip)
                {
                    muni.Muni_Codigo = item.Muni_Codigo;
                    muni.Muni_Descripcion = item.Muni_Descripcion;
                    muni.Depa_Codigo = item.Depa_Codigo;
                    muni.Feca_Creacion = item.Feca_Creacion;
                    muni.Feca_Creacion = item.Feca_Creacion;
                    muni.Usua_Creacion = item.Usua_Creacion;
                    muni.Usua_Modificacion = item.Usua_Modificacion;
                    muni.UsuaC_Nombre = item.UsuaC_Nombre;
                    muni.UsuaM_Nombre = item.UsuaM_Nombre;
                }
                return View(muni);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}