using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el departamento");
                }
            }
            return View(rol);
        }
    }
}
