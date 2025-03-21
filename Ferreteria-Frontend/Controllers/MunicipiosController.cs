using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class MunicipiosController : Controller
    {
        private readonly HttpClient _httpClient;

        public MunicipiosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Municipios";

            var response = await _httpClient.GetAsync("ListarMunicipios");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var municipios = JsonConvert.DeserializeObject<IEnumerable<MunicipioViewModel>>(content);
                return View("Index", municipios);
            }
            return View(new List<MunicipioViewModel>());
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MunicipioViewModel muni)
        {
            muni.Usua_Creacion = 1;
            muni.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(muni);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarMunicipio", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el municipio");
                }
            }
            return View(muni);
        }


        public async Task<IActionResult> Edit(string id)
        {

            var data = new MunicipioViewModel { Muni_Codigo = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarMunicipio", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<MunicipioViewModel> munic = JsonConvert.DeserializeObject<List<MunicipioViewModel>>(content);

                MunicipioViewModel muni = new MunicipioViewModel();
                foreach (var item in munic)
                {
                    muni.Muni_Codigo = item.Muni_Codigo;
                    muni.Muni_Descripcion = item.Muni_Descripcion;
                    muni.Depa_Codigo = item.Depa_Codigo;


                }

                return PartialView("_Edit", muni);

            }
            else
            {
                return View();

            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, MunicipioViewModel muni)
        {
            muni.Usua_Modificacion = 1;
            muni.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(muni);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarMunicipio", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id });

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el municipio");
                }
            }
            return View(muni);
        }
    }
}
