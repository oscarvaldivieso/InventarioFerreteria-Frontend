using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class MedidasController : Controller
    {
        private readonly HttpClient _httpClient;

        public MedidasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Medidas";
            ViewBag.SubTitle = "Productos";

            var response = await _httpClient.GetAsync("ListarMedidas");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var medidas = JsonConvert.DeserializeObject<IEnumerable<MedidasViewModel>>(content);
                return View("Index", medidas);
            }
            return View(new List<MedidasViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MedidasViewModel medi)
        {
            medi.Usua_Creacion = 1;
            medi.Feca_Creacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(medi);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarMedida", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Creado Correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la medida");
                }
            }
            return View(medi);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = new MedidasViewModel { Medi_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarMedida", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<MedidasViewModel> medida = JsonConvert.DeserializeObject<List<MedidasViewModel>>(content);

                MedidasViewModel medi = new MedidasViewModel();
                foreach (var item in medida)
                {
                    medi.Medi_Id = item.Medi_Id;
                    medi.Medi_Descripcion = item.Medi_Descripcion;
                }

                return PartialView("_Edit", medi);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MedidasViewModel medi)
        {
            medi.Usua_Modificacion = 1;
            medi.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(medi);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarMedida", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Actualizado Correctamente";
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar la medida");
                }
            }
            return View(medi);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new MedidasViewModel { Medi_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("EliminarMedida", content2);

            if (response.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Eliminado Correctamente";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar la medida";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = new MedidasViewModel { Medi_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarMedida", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<MedidasViewModel> medida = JsonConvert.DeserializeObject<List<MedidasViewModel>>(content);

                MedidasViewModel medi = new MedidasViewModel();
                foreach (var item in medida)
                {
                    medi.Medi_Id = item.Medi_Id;
                    medi.Medi_Descripcion = item.Medi_Descripcion;
                    medi.Feca_Creacion = item.Feca_Creacion;
                    medi.Feca_Creacion = item.Feca_Creacion;
                    medi.Usua_Creacion = item.Usua_Creacion;
                    medi.Usua_Modificacion = item.Usua_Modificacion;
                    medi.UsuarioCreacion = item.UsuarioCreacion;
                    medi.UsuarioModificacion = item.UsuarioModificacion;
                }
                return View(medi);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}