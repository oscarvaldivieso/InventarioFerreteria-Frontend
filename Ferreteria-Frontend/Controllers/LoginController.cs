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


        // Vista POST para iniciar sesión
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
         {
            // Verificar si el modelo es válido
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Por favor, ingresa un usuario y contraseña.");
                return View("Index"); // Volver a mostrar la vista en caso de error
            }

            // Crear el objeto de login para enviar
            var loginData = new { Usuario = username, Contrasena = password };
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Realizar la solicitud POST a la API para verificar las credenciales
            var response = await _httpClient.PostAsync("login", content);

            if (response.IsSuccessStatusCode)
            {
                // Si la autenticación es exitosa, redirigir al dashboard o página principal
                //var userInfo = await response.Content.ReadAsStringAsync();
                //// Aquí podrías guardar el ID o nombre del usuario en la sesión si es necesario
                //HttpContext.Session.SetString("UserName", username);

                return RedirectToAction("Index", "Home"); // Redirige al dashboard o donde lo necesites
            }
            else
            {
                // Si la autenticación falla, mostrar un mensaje de error
                ModelState.AddModelError(string.Empty, "Credenciales inválidas");
                return View("Index"); // Volver a mostrar la vista en caso de error
            }
        }
    }
}
