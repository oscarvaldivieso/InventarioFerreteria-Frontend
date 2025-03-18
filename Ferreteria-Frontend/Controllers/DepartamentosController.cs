using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ferreteria_Frontend.Controllers
{
    public class DepartamentosController : Controller
    {
        private readonly HttpClient _httpClient;

        public DepartamentosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("ListarDepartamento");

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var departamentos = JsonConvert.DeserializeObject<IEnumerable<DepartamentoViewModel>>(content);
                return View("Index", departamentos);
            }   
            return View(new List<DepartamentoViewModel>());
        }
    }
}
