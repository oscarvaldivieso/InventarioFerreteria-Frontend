using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    }
}
