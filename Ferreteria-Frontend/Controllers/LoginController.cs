using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        // Constructor para inyectar HttpClient
        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/"); // URL de tu API
        }

        // Vista GET de Login
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
         {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Por favor, ingresa un usuario y contraseña.");
                return View("Index"); 
            }

            var loginData = new LoginViewModel{ Usua_Nombre = username, Usua_Clave = password };
            var json = JsonConvert.SerializeObject(loginData);
            var content2 = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("login", content2);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<LoginViewModel> logins = JsonConvert.DeserializeObject<List<LoginViewModel>>(content);

                LoginViewModel login = new LoginViewModel();

                if(logins.Count > 0)
                {
                    foreach(var item in logins)
                    {
                        login.Usua_Id = item.Usua_Id;
                        login.Usua_Nombre = item.Usua_Nombre;
                        login.Empl_NombreCompleto = item.Empl_NombreCompleto;
                        login.Role_Id = item.Role_Id;
                        login.Empl_Id = item.Empl_Id;
                        login.Usua_EsAdmin = item.Usua_EsAdmin;

                        HttpContext.Session.SetInt32("Id", item.Usua_Id);
                        HttpContext.Session.SetString("Usuario", item.Usua_Nombre);
                        HttpContext.Session.SetString("Empleado", item.Empl_NombreCompleto);
                        HttpContext.Session.SetInt32("Rol_Id", item.Role_Id);
                        HttpContext.Session.SetString("NombreEmpleado", item.Empl_NombreCompleto);
                        HttpContext.Session.SetString("esAdmin", item.Usua_EsAdmin.ToString());
                        
                    }


                    var data1 = new RolViewModel { Role_Id = (int)HttpContext.Session.GetInt32("Rol_Id") };
                    var content1 = new StringContent(JsonConvert.SerializeObject(data1), Encoding.UTF8, "application/json");
                    var response1 = await _httpClient.PostAsync("BuscarPantallas", content1);


                    var rolExistenteContent = await response1.Content.ReadAsStringAsync();
                    var rolExistenteList = JsonConvert.DeserializeObject<List<RolViewModel>>(rolExistenteContent);

                    string cadenaPantallas = "";
                    foreach(var item in rolExistenteList)
                    {
                        cadenaPantallas += item.Pant_Descripcion + ",";

                    }

                    ViewData["Pantallas"] = rolExistenteList;
                    HttpContext.Session.SetString("Pantallas", cadenaPantallas);

                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    TempData["Error"] = "Credenciales invalidas, por favor intente de nuevo";
                    return View("Index");
                }
            }
            else
            {
                TempData["Error"] = "Error";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
