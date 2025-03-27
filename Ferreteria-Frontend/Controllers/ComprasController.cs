﻿using Ferreteria_Frontend.Models;
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

            await CargarProveedores();
            await CargarProductos();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CompraViewModel comp, int Prod_Id, int CpDe_Cantidad, double CpDe_Precio)
        {
            comp.Usua_Creacion = 1;
            comp.Feca_Creacion = DateTime.Now;

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(comp);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("InsertarCompra", content);
                if (response.IsSuccessStatusCode)
                {
                    var content2 = await response.Content.ReadAsStringAsync();
                    List<CompraViewModel> compras = JsonConvert.DeserializeObject<List<CompraViewModel>>(content2);

                    CompraViewModel comp2 = new CompraViewModel();
                    foreach (var item in compras)
                    {
                        comp2.Comp_Id = item.Comp_Id;
                    }

                    CompraDetalleViewModel cpde = new CompraDetalleViewModel();
                    cpde.Comp_Id = comp.Comp_Id;
                    cpde.Prod_Id = Prod_Id;
                    cpde.CpDe_Cantidad = CpDe_Cantidad;
                    cpde.CpDe_Precio = CpDe_Precio;
                    cpde.Usua_Creacion = 1;
                    cpde.Feca_Creacion = DateTime.Now;

                    var json2 = JsonConvert.SerializeObject(cpde);
                    var content3 = new StringContent(json, Encoding.UTF8, "application/json");

                    var response2 = await _httpClient.PostAsync("InsertarCompraDetalle", content3);

                    if (response2.IsSuccessStatusCode)
                    {
                        TempData["MensajeExito"] = "Creado Correctamente";
                    }

                    TempData["MensajeExito"] = "Creado Correctamente";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la compra");
                }
            }
            return View(comp);
        }

        public async Task<IActionResult> Edit(int id)
        {
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
                }

                return View(comp);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CompraViewModel comp)
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
                    TempData["MensajeExito"] = "Actualizado Correctamente";
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el cargo");
                }
            }
            return View(comp);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = new CargoViewModel { Carg_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/EliminarCompra", content2);

            if (response.IsSuccessStatusCode)
            {
                TempData["MensajeExito"] = "Eliminado Correctamente";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el cargo";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = new CargoViewModel { Carg_Id = id };
            var content2 = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/BuscarCargo", content2);
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