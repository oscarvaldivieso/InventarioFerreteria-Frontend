using Ferreteria_Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Text;

namespace Ferreteria_Frontend.Controllers
{
    public class ComprasController : Controller
    {
        private readonly HttpClient _httpClient;

        public ComprasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7214/");
        }

        public async Task<IActionResult> ReporteCompra()
        {
            var response = await _httpClient.GetAsync("BuscarFecha");

            await ObtenerProductosAsync();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var productos = JsonConvert.DeserializeObject<IEnumerable<CompraViewModel>>(content);
                return View("ReporteCompra", productos);
            }
            return View("ReporteCompra", new List<CompraViewModel>());
        }

        public async Task<List<ProductoViewModel>> ObtenerProductosAsync()
        {
            var response = await _httpClient.GetAsync("ListarProductos");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ProductoViewModel>>(jsonString);
            }
            return new List<ProductoViewModel>();
        }

        private async Task CargarProductos()
        {
            var response = await _httpClient.GetAsync("ListarProductos");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var productos = JsonConvert.DeserializeObject<IEnumerable<ProductoViewModel>>(content);
                ViewBag.Prod_Id = new SelectList(productos, "Prod_Id", "Prod_Descripcion");
            }
        }

        private async Task CargarProveedores()
        {
            var response = await _httpClient.GetAsync("ListarProveedores");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var proveedores = JsonConvert.DeserializeObject<IEnumerable<ProveedoresViewModel>>(content);
                ViewBag.Prov_Id = new SelectList(proveedores, "Prov_Id", "Prov_Nombre");
            }
        }

        public async Task<List<ProveedoresViewModel>> ObtenerProveedoresAsync()
        {
            var response = await _httpClient.GetAsync("/ListarProveedores");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ProveedoresViewModel>>(jsonString);
            }
            return new List<ProveedoresViewModel>();
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Compras";
            ViewBag.SubTitle = "Compra";

            ViewBag.proveedores = await ObtenerProveedoresAsync();
            ViewBag.productos = await ObtenerProductosAsync();
            await CargarProveedores();
            await CargarProductos();
            var response = await _httpClient.GetAsync("ListarCompras");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var compras = JsonConvert.DeserializeObject<IEnumerable<CompraViewModel>>(content);
                return View("Index", compras);
            }
            return View(new List<CompraViewModel>());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.PageTitle = "Crear Comprar";
            ViewBag.SubTitle = "Compra";

            ViewBag.proveedores = await ObtenerProveedoresAsync();
            ViewBag.productos = await ObtenerProductosAsync();

            await CargarProveedores();
            await CargarProductos();

  
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CompraViewModel comp)
        {
            comp.Usua_Creacion = 1;
            comp.Feca_Creacion = DateTime.Now;

            // Validaciones básicas
            if (comp.Detalles == null || comp.Detalles.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Agrega al menos un detalle antes de guardar la compra.");
            }

            if (!ModelState.IsValid)
            {
                await CargarProveedores();
                await CargarProductos();
                return View(comp);
            }

            // Insertar encabezado
            var json = JsonConvert.SerializeObject(comp);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("InsertarCompra", content);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error al crear la compra (encabezado).");
                await CargarProveedores();
                await CargarProductos();
                return View(comp);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<dynamic>(responseContent);

            int compId = (int)obj.comp_Id;

            if (compId == 0)
            {
                ModelState.AddModelError(string.Empty, "No se pudo obtener el id de la compra creada.");
                await CargarProveedores();
                await CargarProductos();
                return View(comp);
            }

            // Insertar detalles uno por uno
            var erroresDetalles = new List<string>();
            foreach (var det in comp.Detalles)
            {
                var cpde = new CompraDetalleViewModel
                {
                    Comp_Id = compId,
                    Prod_Id = det.Prod_Id,
                    CpDe_Cantidad = det.CpDe_Cantidad,
                    CpDe_Precio = det.CpDe_Precio,
                    Usua_Creacion = 1,
                    Feca_Creacion = DateTime.Now
                };

                var jsonDet = JsonConvert.SerializeObject(cpde);
                var contentDet = new StringContent(jsonDet, Encoding.UTF8, "application/json");

                var respDet = await _httpClient.PostAsync("InsertarCompraDetalle", contentDet);
                if (!respDet.IsSuccessStatusCode)
                {
                    // leer detalle de error si la API lo devuelve
                    var err = await respDet.Content.ReadAsStringAsync();
                    erroresDetalles.Add($"Producto {det.Prod_Id}: {err}");
                }
            }

            if (erroresDetalles.Count > 0)
            {
                // Opcional: borrar encabezado si fallaron todos los detalles o mostrar aviso
                ModelState.AddModelError(string.Empty, "Algunos detalles no se pudieron insertar: " + string.Join(" | ", erroresDetalles));
                await CargarProveedores();
                await CargarProductos();
                return View(comp);
            }

            // Todo correcto
            TempData["MensajeExito"] = "Compra creada correctamente";
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.PageTitle = "Actualizar Comprar";
            ViewBag.SubTitle = "Compra";
            var data = new CompraViewModel { Comp_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("BuscarCompra", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<CompraViewModel> compras = JsonConvert.DeserializeObject<List<CompraViewModel>>(content);

                CompraViewModel comp = new CompraViewModel();
                foreach (var item in compras)
                {
                    comp.Comp_Id = item.Comp_Id;
                    comp.Prod_Id = item.Prod_Id;
                    comp.Prov_Id = item.Prov_Id;
                    comp.Comp_Fecha = item.Comp_Fecha;
                    comp.CpDe_Cantidad = item.CpDe_Cantidad;
                    comp.CpDe_Precio = item.CpDe_Precio;
                }

                await CargarProveedores();
                await CargarProductos();
                return View(comp);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CompraViewModel comp, int Prod_Id, int CpDe_Cantidad, double CpDe_Precio)
        {
            comp.Usua_Modificacion = 1;
            comp.Feca_Modificacion = DateTime.Now;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(comp);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("ActualizarCompra", content);
                if (response.IsSuccessStatusCode)
                {
                    CompraDetalleViewModel cpde = new CompraDetalleViewModel();
                    cpde.Comp_Id = comp.Comp_Id;
                    cpde.Prod_Id = Prod_Id;
                    cpde.CpDe_Cantidad = CpDe_Cantidad;
                    cpde.CpDe_Precio = CpDe_Precio;
                    cpde.Usua_Modificacion = 1;
                    cpde.Feca_Modificacion = DateTime.Now;

                    var json2 = JsonConvert.SerializeObject(cpde);
                    var content3 = new StringContent(json2, Encoding.UTF8, "application/json");

                    var response2 = await _httpClient.PutAsync("ActualizarCompraDetalle", content3);
                    if (response2.IsSuccessStatusCode)
                    {
                        TempData["MensajeExito"] = "Actualizado Correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error al actualizar el detalle de la compra");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar la compra");
                }
            }
            await CargarProveedores();
            await CargarProductos();
            return View(comp);
        }

        public async Task<IActionResult> Delete(int id, CompraViewModel comp)
        {
            var data = new CompraViewModel { Comp_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/EliminarCompra", content2);

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.SerializeObject(id);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response2 = await _httpClient.PutAsync("/EliminarCompra", content);
                if (response2.IsSuccessStatusCode)
                {
                    CompraDetalleViewModel cpde = new CompraDetalleViewModel();
                    cpde.Comp_Id = comp.Comp_Id;

                    var json2 = JsonConvert.SerializeObject(cpde);
                    var content3 = new StringContent(json2, Encoding.UTF8, "application/json");

                    var response3 = await _httpClient.PutAsync("EliminarCompraDetalle", content3);
                    if (response2.IsSuccessStatusCode)
                    {
                        TempData["MensajeExito"] = "Eliminado Correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error al eliminar el detalle de la compra");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al elminar la compra");
                }
            }
            else
            {
                TempData["Error"] = "Error al eliminar la compra";
                return RedirectToAction("Index");
            }
            TempData["MensajeExito"] = "Eliminado Correctamente";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = new CompraViewModel { Comp_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarCompra", content2);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<CompraViewModel> compras = new List<CompraViewModel>();

                CompraViewModel comp = new CompraViewModel();

                foreach (var item in compras)
                {
                    comp.Comp_Id = item.Comp_Id;
                    comp.Prov_Nombre = item.Prov_Nombre;
                    comp.Comp_Fecha = item.Comp_Fecha;
                    comp.Prod_Descripcion = item.Prod_Descripcion;
                    comp.CpDe_Cantidad = item.CpDe_Cantidad;
                    comp.CpDe_Precio = item.CpDe_Precio;
                }
                return View(comp);
            }
            else
            {
                TempData["error"] = "Error al mostrar detalle";
                return RedirectToAction("Index");
            }
        }
    }
}