using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsuariosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        private async Task ListarRoles()
        {
            var response = await _httpClient.GetAsync("ListarRoles");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var roles = JsonConvert.DeserializeObject<IEnumerable<RolViewModel>>(content);
                ViewBag.Role_Id = new SelectList(roles, "Role_Id", "Role_Descripcion");
            }
        }

        private async Task ListarEmpleados()
        {
            var response = await _httpClient.GetAsync("ListarEmpleados");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var empleados = JsonConvert.DeserializeObject<IEnumerable<EmpleadoViewModel>>(content);
                ViewBag.Empl_Id = new SelectList(empleados, "Empl_Id", "Empl_Nombre");
            }
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Usuarios";
            ViewBag.SubTitle = "Acceso";

            var response = await _httpClient.GetAsync("ListarUsuarios");
            await ListarRoles();
            await ListarEmpleados();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<IEnumerable<UsuarioViewModel>>(content);
                return View("Index", usuarios);
            }
            await ListarRoles();
            await ListarEmpleados();
            return View(new List<UsuarioViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioViewModel usu)
        {
            ModelState.Remove("NuevaClave");
            ModelState.Remove("ConfirmarClave");
            ModelState.Remove("Empl_NombreCompleto");
            ModelState.Remove("Role_Descripcion");
            usu.Usua_Creacion = 1;
            usu.Feca_Creacion = DateTime.Now;
            await ListarRoles();
            await ListarEmpleados();
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(usu);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarUsuario", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Creado Correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el usuario");
                }
            }
            await ListarRoles();
            await ListarEmpleados();
            return View(usu);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = new UsuarioViewModel { Usua_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarUsuario", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<UsuarioViewModel> usuario = JsonConvert.DeserializeObject<List<UsuarioViewModel>>(content);

                UsuarioViewModel usu = new UsuarioViewModel();
                foreach (var item in usuario)
                {
                    usu.Usua_Id = item.Usua_Id;
                    usu.Usua_Nombre = item.Usua_Nombre;
                    usu.Empl_Id = item.Empl_Id;
                    usu.Role_Id = item.Role_Id;
                    usu.Usua_EsAdmin = item.Usua_EsAdmin;
                }
                await ListarRoles();
                await ListarEmpleados();
                return PartialView("_Edit", usu);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UsuarioViewModel usu)
        {
            ModelState.Remove("ConfirmarClave");
            ModelState.Remove("NuevaClave");
            ModelState.Remove("Empl_NombreCompleto");
            ModelState.Remove("Role_Descripcion");
            ModelState.Remove("Usua_Clave");
            usu.Usua_Modificacion = 1;
            usu.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(usu);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarUsuario", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Actualizado Correctamente";
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el usuario");
                }
            }
            return View(usu);
        }

        public async Task<IActionResult> RestablecerClave(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "ID de usuario inválido");
                return PartialView("_Restablecer", new UsuarioViewModel());
            }
            HttpContext.Session.SetInt32("IdRestablecer", id);
            ViewBag.Id = id; // Guardar el ID de usuario en una variable de vista
            var data = new UsuarioViewModel { Usua_Id = id }; // Crear el modelo con el ID de usuario
            return PartialView("_Restablecer", data); // Devolver la vista parcial con el modelo
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(UsuarioViewModel item)
        {
            int id = int.Parse(HttpContext.Session.GetInt32("IdRestablecer").ToString());
            item.Usua_Id = id;
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/RestablecerClave", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Clave Actualizada Correctamente";
                return RedirectToAction("Index", new { id });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar la clave");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ActivarUsuario(int id)
        {
            var data = new UsuarioViewModel { Usua_Id = id };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("ActivarUsuario", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Activado Correctamente";
                return RedirectToAction("Index");  // Redirige a la lista de usuarios después de realizar la acción
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al activar/desactivar el usuario");
                return RedirectToAction("Index");  // Redirige a la lista con un mensaje de error si no tuvo éxito
            }
        }


        [HttpPost]
        public async Task<IActionResult> DesactivarUsuario(int id)
        {
            var data = new UsuarioViewModel { Usua_Id = id };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("DesactivarUsuario", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Desactivado Correctamente";
                return RedirectToAction("Index");  // Redirige a la lista de usuarios después de realizar la acción
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al activar/desactivar el usuario");
                return RedirectToAction("Index");  // Redirige a la lista con un mensaje de error si no tuvo éxito
            }
        }
    }
}