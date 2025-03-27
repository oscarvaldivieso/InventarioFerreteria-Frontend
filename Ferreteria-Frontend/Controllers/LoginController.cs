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

            var loginData = new LoginViewModel{ Usuario = username, Contrasena = password };
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
                        login.Id = item.Id;
                        login.Usuario = item.Usuario;
                        login.Nombre_Empleado = item.Nombre_Empleado;
                        login.Rol = item.Rol;
                        login.Empleado_Id = item.Empleado_Id;
                        login.Admin = item.Admin;

                        HttpContext.Session.SetInt32("Id", item.Id);
                        HttpContext.Session.SetString("Usuario", item.Usuario);
                        HttpContext.Session.SetString("Empleado", item.Nombre_Empleado);
                        HttpContext.Session.SetInt32("Rol", item.Rol);
                        HttpContext.Session.SetString("esAdmin", item.Admin.ToString());
                        
                    }


                    var data1 = new RolViewModel { Role_Id = (int)HttpContext.Session.GetInt32("Rol") };
                    var content1 = new StringContent(JsonConvert.SerializeObject(data1), Encoding.UTF8, "application/json");
                    var response1 = await _httpClient.PostAsync("BuscarRol", content1);


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
    }
}
