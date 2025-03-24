using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProveedoresController(IHttpClientFactory httpClientFactory)
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

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Proveedores";
            ViewBag.SubTitle = "Compras";

            var response = await _httpClient.GetAsync("ListarProveedores");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var proveedores = JsonConvert.DeserializeObject<IEnumerable<ProveedoresViewModel>>(content);
                return View("Index", proveedores);
            }
            return View(new List<ProveedoresViewModel>());
        }

        public async Task<IActionResult> Create()
        {
            await CargarMunicipios();
            await CargarDepartamentos();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProveedoresViewModel prov)
        {
            prov.Usua_Creacion = 1;
            prov.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(prov);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarProveedor", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el proveedor");
                }
            }

            await CargarDepartamentos();
            await CargarMunicipios();
            return View(prov);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = new ProveedoresViewModel { Prov_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarProveedor", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<ProveedoresViewModel> proveedores = JsonConvert.DeserializeObject<List<ProveedoresViewModel>>(content);

                ProveedoresViewModel prov = new ProveedoresViewModel();
                foreach (var item in proveedores)
                {
                    prov.Prov_Id = item.Prov_Id;
                    prov.Prov_Nombre = item.Prov_Nombre;
                    prov.Prov_Contacto = item.Prov_Contacto;
                    prov.Muni_Codigo = item.Muni_Codigo;
                    prov.Prov_DireccionExacta = item.Prov_DireccionExacta;
                }
                await CargarDepartamentos();
                await CargarMunicipios();
                return PartialView("_Edit", prov);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, ProveedoresViewModel prov)
        {
            prov.Usua_Modificacion = 1;
            prov.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(prov);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("ActualizarProveedor", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el proveedor");
                }
            }
            return View(prov);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new ProveedoresViewModel { Prov_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/EliminaProveedor", content2);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el proveedor";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = new ProveedoresViewModel { Prov_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarProveedor", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<ProveedoresViewModel> proveedores = JsonConvert.DeserializeObject<List<ProveedoresViewModel>>(content);

                ProveedoresViewModel prov = new ProveedoresViewModel();
                foreach (var item in proveedores)
                {
                    prov.Prov_Id = item.Prov_Id;
                    prov.Prov_Nombre = item.Prov_Nombre;
                    prov.Prov_Contacto = item.Prov_Contacto;
                    prov.Muni_Codigo = item.Muni_Codigo;
                    prov.Prov_DireccionExacta = item.Prov_DireccionExacta;
                    prov.Feca_Creacion = item.Feca_Creacion;
                    prov.Feca_Modificacion = item.Feca_Modificacion;
                    prov.Usua_Creacion = item.Usua_Creacion;
                    prov.Usua_Modificacion = item.Usua_Modificacion;
                    prov.UsuaC_Nombre = item.UsuaC_Nombre;
                    prov.UsuaM_Nombre = item.UsuaM_Nombre;
                }
                return View(prov);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}