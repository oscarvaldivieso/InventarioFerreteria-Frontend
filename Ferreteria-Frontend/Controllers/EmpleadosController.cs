using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly HttpClient _httpClient;

        public EmpleadosController(IHttpClientFactory httpClientFactory)
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

        private async Task CargarCargos()
        {
            var response = await _httpClient.GetAsync("ListarCargos");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var cargos = JsonConvert.DeserializeObject<IEnumerable<CargoViewModel>>(content);
                ViewBag.Carg_Id = new SelectList(cargos, "Carg_Id", "Carg_Descripcion");
            }
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Empleados";
            ViewBag.SubTitle = "Ferreteria";

            await CargarDepartamentos();
            await CargarMunicipios();
            await CargarEstadosCiviles();
            await CargarCargos();

            var response = await _httpClient.GetAsync("ListarEmpleados");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var empleados = JsonConvert.DeserializeObject<IEnumerable<EmpleadoViewModel>>(content);
                return View("Index", empleados);
            }
            return View(new List<EmpleadoViewModel>());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.PageTitle = "Crear Empleado";
            ViewBag.SubTitle = "Ferreteria";

            await CargarDepartamentos();
            await CargarMunicipios();
            await CargarEstadosCiviles();
            await CargarCargos();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmpleadoViewModel empl)
        {
            empl.Usua_Creacion = 1;
            empl.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(empl);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("InsertarEmpleado", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Creado Correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el emplnte");
                }
            }
            await CargarDepartamentos();
            await CargarMunicipios();
            await CargarEstadosCiviles();
            await CargarCargos();
            return View(empl);
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.PageTitle = "Editar Empleado";
            ViewBag.SubTitle = "Ferreteria";

            var data = new EmpleadoViewModel { Empl_DNI = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarEmpleado", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<EmpleadoViewModel> empleados = JsonConvert.DeserializeObject<List<EmpleadoViewModel>>(content);

                EmpleadoViewModel empl = new EmpleadoViewModel();
                foreach (var item in empleados)
                {
                    empl.Empl_Id = item.Empl_Id;
                    empl.Empl_DNI = item.Empl_DNI;
                    empl.Empl_Nombre = item.Empl_Nombre;
                    empl.Empl_Apellido = item.Empl_Apellido;
                    empl.Empl_Sexo = item.Empl_Sexo;
                    empl.EsCv_Id = item.EsCv_Id;
                    empl.Carg_Id = item.Carg_Id;
                    empl.Depa_Codigo = item.Depa_Codigo;
                    empl.Muni_Codigo = item.Muni_Codigo;
                    empl.Empl_Direccion = item.Empl_Direccion;
                }
                await CargarEstadosCiviles();
                await CargarMunicipios();
                await CargarDepartamentos();
                await CargarCargos();
                return View(empl);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmpleadoViewModel empl)
        {
            empl.Usua_Modificacion = 1;
            empl.Feca_Modificacion = DateTime.Now;
            empl.Empl_Id = id;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(empl);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("ActualizarEmpleado", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Actualizado Correctamente";
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el emplnte");
                }
            }
            await CargarDepartamentos();
            await CargarMunicipios();
            await CargarEstadosCiviles();
            await CargarCargos();
            return View(empl);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new EmpleadoViewModel { Empl_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("EliminarEmpleado", content2);

            if (response.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Eliminado Correctamente";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el empleado";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            var data = new EmpleadoViewModel { Empl_DNI = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarEmpleado", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<EmpleadoViewModel> empleados = JsonConvert.DeserializeObject<List<EmpleadoViewModel>>(content);

                EmpleadoViewModel empl = new EmpleadoViewModel();
                foreach (var item in empleados)
                {
                    empl.Empl_Id = item.Empl_Id;
                    empl.Empl_DNI = item.Empl_DNI;
                    empl.Empl_Nombre = item.Empl_Nombre;
                    empl.Empl_Apellido = item.Empl_Apellido;
                    empl.Empl_Sexo = item.Empl_Sexo;
                    empl.EsCv_Id = item.EsCv_Id;
                    empl.Carg_Id = item.Carg_Id;
                    empl.Muni_Codigo = item.Muni_Codigo;
                    empl.Empl_Direccion = item.Empl_Direccion;
                    empl.Feca_Creacion = item.Feca_Creacion;
                    empl.Feca_Modificacion = item.Feca_Modificacion;
                    empl.Usua_Creacion = item.Usua_Creacion;
                    empl.Usua_Modificacion = item.Usua_Modificacion;
                    empl.UsuaC_Nombre = item.UsuaC_Nombre;
                    empl.UsuaM_Nombre = item.UsuaM_Nombre;
                }
                return View(empl);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}