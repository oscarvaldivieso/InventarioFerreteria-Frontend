using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Usuarios";
            ViewBag.SubTitle = "Acceso";

            var response = await _httpClient.GetAsync("ListarUsuarios");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<IEnumerable<UsuarioViewModel>>(content);
                return View("Index", usuarios);
            }
            return View(new List<UsuarioViewModel>());
        }
    }
}
