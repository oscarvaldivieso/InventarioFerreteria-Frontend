using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class SucursalesController : Controller
    {
        private readonly HttpClient _httpClient;

        public SucursalesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        private async Task CargarMunicipios()
        {
            var response = await _httpClient.GetAsync("ListarMunicipios");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var municipios = JsonConvert.DeserializeObject<IEnumerable<MunicipioViewModel>>(content);
                ViewBag.Muni_Codigo = new SelectList(municipios, "Muni_Codigo", "Muni_Descripcion");
            }
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
            ViewBag.PageTitle = "Sucursales";
            ViewBag.SubTitle = "Ferreteria";

            var response = await _httpClient.GetAsync("ListarSucursales");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var sucursales = JsonConvert.DeserializeObject<IEnumerable<SucursalViewModel>>(content);
                return View("Index", sucursales);
            }
            return View(new List<SucursalViewModel>());
        }

        public async Task<IActionResult> Create()
        {
            await CargarMunicipios();
            await CargarDepartamentos();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SucursalViewModel sucu)
        {
            sucu.Usua_Creacion = 1;
            sucu.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(sucu);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarSucursal", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la sucursal");
                }
            }
            await CargarMunicipios();
            await CargarDepartamentos();
            return View(sucu);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = new SucursalViewModel { Sucu_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarSucursal", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<SucursalViewModel> sucursales = JsonConvert.DeserializeObject<List<SucursalViewModel>>(content);

                SucursalViewModel sucu = new SucursalViewModel();
                foreach (var item in sucursales)
                {
                    sucu.Sucu_Id = item.Sucu_Id;
                    sucu.Sucu_Nombre = item.Sucu_Nombre;
                    sucu.Depa_Codigo = item.Depa_Codigo;
                    sucu.Muni_Codigo = item.Muni_Codigo;
                    sucu.Sucu_DireccionExacta = item.Sucu_DireccionExacta;
                }

                await CargarMunicipios();
                await CargarDepartamentos();
                return PartialView("_Edit", sucu);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SucursalViewModel sucu)
        {
            sucu.Usua_Modificacion = 1;
            sucu.Feca_Modificacion = DateTime.Now;
            sucu.Sucu_Id = id;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(sucu);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarSucursal", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar la sucursal");
                }
            }
            await CargarMunicipios();
            await CargarDepartamentos();
            return View(sucu);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new SucursalViewModel { Sucu_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/EliminarSucursal", content2);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar la sucursal";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = new SucursalViewModel { Sucu_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarSucursal", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<SucursalViewModel> sucursales = JsonConvert.DeserializeObject<List<SucursalViewModel>>(content);

                SucursalViewModel sucu = new SucursalViewModel();
                foreach (var item in sucursales)
                {
                    sucu.Sucu_Id = item.Sucu_Id;
                    sucu.Sucu_Nombre = item.Sucu_Nombre;
                    sucu.Feca_Creacion = item.Feca_Creacion;
                    sucu.Feca_Modificacion = item.Feca_Modificacion;
                    sucu.Usua_Creacion = item.Usua_Creacion;
                    sucu.Usua_Modificacion = item.Usua_Modificacion;
                    sucu.UsuaC_Nombre = item.UsuaC_Nombre;
                    sucu.UsuaM_Nombre = item.UsuaM_Nombre;
                }
                return View(sucu);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}