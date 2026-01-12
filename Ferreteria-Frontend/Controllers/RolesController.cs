using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class RolesController : Controller
    {
        private readonly HttpClient _httpClient;

        public RolesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Roles";
            ViewBag.SubTitle = "Acceso";

            var response = await _httpClient.GetAsync("ListarRoles");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var roles = JsonConvert.DeserializeObject<IEnumerable<RolViewModel>>(content);
                return View("Index", roles);
            }
            return View(new List<RolViewModel>());
        }

        public IActionResult Create()
        {
            ViewBag.PageTitle = "Crear un rol";
            ViewBag.SubTitle = "Acceso";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolViewModel rol)
        {
            rol.Usua_Creacion = 1;
            rol.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(rol);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarRol", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Creado Correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el departamento");
                }
            }
            return View(rol);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.PageTitle = "Editar un rol";
            ViewBag.SubTitle = "Acceso";

            // petición a la API para obtener el/los registros del rol con sus pantallas
            var requestModel = new { role_Id = id };
            var content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("BuscarRol", content);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo obtener el rol desde la API.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();

            // La API devuelve una lista donde cada elemento puede contener pant_Id.
            JArray arr;
            try
            {
                arr = JArray.Parse(json);
            }
            catch
            {
                TempData["Error"] = "Respuesta inválida de la API.";
                return RedirectToAction("Index");
            }

            if (arr.Count == 0)
            {
                TempData["Error"] = "Rol no encontrado.";
                return RedirectToAction("Index");
            }

            // Construir el modelo: datos del primero + lista de pantIds
            var first = arr[0];
            var rol = new RolViewModel
            {
                Role_Id = (int?)first["role_Id"] ?? (int?)first["Role_Id"] ?? 0,
                Role_Descripcion = (string?)first["role_Descripcion"] ?? (string?)first["Role_Descripcion"] ?? string.Empty,
                PantIds = new List<int>()
            };

            foreach (var item in arr)
            {
                var pj = item["pant_Id"] ?? item["pantId"] ?? item["Pant_Id"] ?? item["pant_id"];
                if (pj != null && int.TryParse(pj.ToString(), out var pid))
                {
                    if (!rol.PantIds.Contains(pid))
                        rol.PantIds.Add(pid);
                }
            }

            return View(rol);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RolViewModel rol)
        {
            rol.Role_Id = id;
            rol.Usua_Modificacion = 1;
            rol.Feca_Modificacion = DateTime.Now;

            // Asegurarse de que la lista exista (evita nulls al serializar)
            if (rol.PantIds == null)
            {
                rol.PantIds = new List<int>();
            }

            if (!ModelState.IsValid)
            {
                // devolver la vista para corregir errores (si necesita repoblar ViewBags, hágalo aquí)
                return View(rol);
            }

            // Serializar en camelCase para coincidir con la API (pantIds)
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(rol, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("EditarRol", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Actualizado Correctamente";
                return RedirectToAction("Index");
            }
            else
            {
                // Leer mensaje de la API para mostrarlo (útil en debugging)
                var apiError = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Error al actualizar el rol (API): " + apiError);
                return View(rol);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new RolViewModel { Role_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("EliminarRol", content2);

            if (response.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Eliminado Correctamente";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el rol";
                return RedirectToAction("Index");
            }
        }
    }
}
