using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class ClientesController : Controller
    {
        private readonly HttpClient _httpClient;

        public ClientesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Clientes";
            ViewBag.SubTitle = "General";

            ViewData["EsCv_Id"] = new SelectList("EsCv_Id", "EsCv_Descripcion");
            ViewData["Muni_Codigo"] = new SelectList("Muni_Codigo", "Muni_Descripcion");
            ViewData["Depa_Codigo"] = new SelectList("Depa_Codigo", "Depa_Descripcion");

            var response = await _httpClient.GetAsync("ListarClientes");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var clientes = JsonConvert.DeserializeObject<IEnumerable<ClienteViewModel>>(content);
                return View("Index", clientes);
            }
            return View(new List<ClienteViewModel>());
        }

        public async Task<IActionResult> Create()
        {
            var responseEsCv = await _httpClient.GetAsync("ListarEstadosCiviles");

            if (responseEsCv.IsSuccessStatusCode)
            {
                var content = await responseEsCv.Content.ReadAsStringAsync();
                var estadosCiviles = JsonConvert.DeserializeObject<List<EstadoCivilViewModel>>(content);

                ViewBag.EsCv_Id = new SelectList(estadosCiviles, "EsCv_Id", "EsCv_Descripcion");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al cargar los estados civiles.");
                ViewBag.EsCv_Id = new SelectList(new List<EstadoCivilViewModel>(), "EsCv_Id", "EsCv_Descripcion");
            }

            var responseMuni = await _httpClient.GetAsync("ListarMunicipios");

            if (responseMuni.IsSuccessStatusCode)
            {
                var content = await responseMuni.Content.ReadAsStringAsync();
                var municipios = JsonConvert.DeserializeObject<List<MunicipioViewModel>>(content);

                ViewBag.Muni_Codigo = new SelectList(municipios, "Muni_Codigo", "Muni_Descripcion");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al cargar los municipios.");
                ViewBag.Muni_Codigo = new SelectList(new List<MunicipioViewModel>(), "Muni_Codigo", "Muni_Descripcion");
            }

            var responseDepa = await _httpClient.GetAsync("ListarDepartamentos");

            if (responseDepa.IsSuccessStatusCode)
            {
                var content = await responseDepa.Content.ReadAsStringAsync();
                var departamentos = JsonConvert.DeserializeObject<List<DepartamentoViewModel>>(content);
                ViewBag.Depa_Codigo = new SelectList(departamentos, "Depa_Codigo", "Depa_Descripcion");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al cargar los departamentos.");
                ViewBag.Depa_Codigo = new SelectList(new List<DepartamentoViewModel>(), "Depa_Codigo", "Depa_Descripcion");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClienteViewModel clie)
        {
            clie.Usua_Creacion = 1;
            clie.Feca_Creacion = DateTime.Now;

            var response = await _httpClient.GetAsync("ListarEstadosCiviles");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var estadosCiviles = JsonConvert.DeserializeObject<List<EstadoCivilViewModel>>(content);

                ViewBag.EsCv_Id = new SelectList(estadosCiviles, "EsCv_Id", "EsCv_Descripcion",clie.EsCv_Id);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al cargar los estados civiles.");
                ViewBag.EsCv_Id = new SelectList(new List<EstadoCivilViewModel>(), "EsCv_Id", "EsCv_Descripcion");
            }

            var responseMuni = await _httpClient.GetAsync("ListarMunicipios");

            if (responseMuni.IsSuccessStatusCode)
            {
                var content = await responseMuni.Content.ReadAsStringAsync();
                var municipios = JsonConvert.DeserializeObject<List<MunicipioViewModel>>(content);
                ViewBag.Muni_Codigo = new SelectList(municipios, "Muni_Codigo", "Muni_Descripcion", clie.Muni_Codigo);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al cargar los municipios.");
                ViewBag.Muni_Codigo = new SelectList(new List<MunicipioViewModel>(), "Muni_Codigo", "Muni_Descripcion");
            }

            var responseDepa = await _httpClient.GetAsync("ListarDepartamentos");

            if (responseDepa.IsSuccessStatusCode)
            {
                var content = await responseDepa.Content.ReadAsStringAsync();
                var departamentos = JsonConvert.DeserializeObject<List<DepartamentoViewModel>>(content);
                ViewBag.Depa_Codigo = new SelectList(departamentos, "Depa_Codigo", "Depa_Descripcion", clie.Depa_Codigo);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al cargar los departamentos.");
                ViewBag.Depa_Codigo = new SelectList(new List<DepartamentoViewModel>(), "Depa_Codigo", "Depa_Descripcion");
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(clie);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var responsePost = await _httpClient.PostAsync("InsertarCliente", content);
                if (responsePost.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el cliente");
                }
            }

            return View(clie);
        }

        public async Task<IActionResult> Edit(string dni)
        {
            var data = new ClienteViewModel { Clie_DNI = dni };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarCliente", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<ClienteViewModel> clientes = JsonConvert.DeserializeObject<List<ClienteViewModel>>(content);

                ClienteViewModel clie = new ClienteViewModel();
                foreach (var item in clientes)
                {
                    clie.Clie_Id = item.Clie_Id;
                    clie.Clie_DNI = item.Clie_DNI;
                    clie.Clie_Nombre = item.Clie_Nombre;
                    clie.Clie_Apellido = item.Clie_Apellido;
                    clie.Clie_Sexo = item.Clie_Sexo;
                    clie.EsCv_Id = item.EsCv_Id;
                    clie.Muni_Codigo = item.Muni_Codigo;
                    clie.Clie_Direccion = item.Clie_Direccion;
                }
                return View(clie);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ClienteViewModel clie)
        {
            clie.Usua_Modificacion = 1;
            clie.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(clie);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarCLiente", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el cliente");
                }
            }
            return View(clie);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new ClienteViewModel { Clie_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("EliminarCliente", content2);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el cliente";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = new ClienteViewModel { Clie_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarCliente", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<ClienteViewModel> clientes = JsonConvert.DeserializeObject<List<ClienteViewModel>>(content);

                ClienteViewModel clie = new ClienteViewModel();
                foreach (var item in clientes)
                {
                    clie.Clie_Id = item.Clie_Id;
                    clie.Clie_DNI = item.Clie_DNI;
                    clie.Clie_Nombre = item.Clie_Nombre;
                    clie.Clie_Apellido = item.Clie_Apellido;
                    clie.Clie_Sexo = item.Clie_Sexo;
                    clie.EsCv_Id = item.EsCv_Id;
                    clie.Muni_Codigo = item.Muni_Codigo;
                    clie.Clie_Direccion = item.Clie_Direccion;
                    clie.Feca_Creacion = item.Feca_Creacion;
                    clie.Feca_Modificacion = item.Feca_Modificacion;
                    clie.Usua_Creacion = item.Usua_Creacion;
                    clie.Usua_Modificacion = item.Usua_Modificacion;
                    clie.UsuaC_Nombre = item.UsuaC_Nombre;
                    clie.UsuaM_Nombre = item.UsuaM_Nombre;
                }
                return View(clie);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}
