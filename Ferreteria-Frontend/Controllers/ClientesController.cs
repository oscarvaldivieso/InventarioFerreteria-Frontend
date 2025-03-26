using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Text;
using System.Threading.Tasks;

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

        private async Task CargarEstadosCiviles()
        {
            var response = await _httpClient.GetAsync("ListarEstadosCiviles");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var estadosciviles = JsonConvert.DeserializeObject<IEnumerable<EstadoCivilViewModel>>(content);
                ViewBag.EsCv_Id = new SelectList(estadosciviles, "EsCv_Id", "EsCv_Descripcion");
            }
        }

        public async Task<List<EstadoCivilViewModel>> ObtenerEstadosCivilesAsync()
        {
            var response = await _httpClient.GetAsync("/ListarEstadosCiviles");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<EstadoCivilViewModel>>(jsonString);
            }
            return new List<EstadoCivilViewModel>();
        }

        public async Task<List<MunicipioViewModel>> ObtenerMunicipiosAsync()
        {
            var response = await _httpClient.GetAsync("ListarMunicipios");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MunicipioViewModel>>(jsonString);
            }
            return new List<MunicipioViewModel>();
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Clientes";
            ViewBag.SubTitle = "General";

            ViewBag.estados = await ObtenerEstadosCivilesAsync();
            ViewBag.municipios = await ObtenerMunicipiosAsync();
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
            ViewBag.PageTitle = "Crear Cliente";
            ViewBag.SubTitle = "General";

            await CargarEstadosCiviles();
            await CargarMunicipios();
            await CargarDepartamentos();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClienteViewModel clie)
        {
            ViewBag.PageTitle = "Crear Cliente";
            ViewBag.SubTitle = "General";

            clie.Usua_Creacion = 1;
            clie.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(clie);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarCliente", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Creado Correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el cliente");
                }
            }

            await CargarEstadosCiviles();
            await CargarMunicipios();
            await CargarDepartamentos();
            return View(clie);
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.PageTitle = "Editar Cliente";
            ViewBag.SubTitle = "General";

            ViewBag.estados = await ObtenerEstadosCivilesAsync();
            var data = new ClienteViewModel { Clie_DNI = id };
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
                    clie.Depa_Codigo = item.Depa_Codigo;
                    clie.Muni_Codigo = item.Muni_Codigo;
                    clie.Clie_Direccion = item.Clie_Direccion;
                }
                await CargarEstadosCiviles();
                await CargarMunicipios();
                await CargarDepartamentos();
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
            ViewBag.PageTitle = "Editar Cliente";
            ViewBag.SubTitle = "General";

            clie.Usua_Modificacion = 1;
            clie.Feca_Modificacion = DateTime.Now;
            clie.Clie_Id = id;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(clie);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarCLiente", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Actualizado Correctamente";
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el cliente");
                }
            }
            await CargarEstadosCiviles();
            await CargarMunicipios();
            await CargarDepartamentos();
            return View(clie);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new ClienteViewModel { Clie_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("EliminarCliente", content2);

            if (response.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Eliminado Correctamente";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el cliente";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            var data = new ClienteViewModel { Clie_DNI = id };
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