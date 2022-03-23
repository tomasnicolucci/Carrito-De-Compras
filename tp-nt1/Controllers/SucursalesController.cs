using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tp_nt1.Database;
using tp_nt1.Models;

namespace tp_nt1a_3.Controllers
{
    //[Authorize(Roles = "Administrador, Empleado")]
    public class SucursalesController : Controller
    {
        private readonly CarritoDbContext _context;

        public SucursalesController(CarritoDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Sucursales.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Sucursales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Direccion,Telefono,Email")] Sucursal sucursal)
        {
            if (ModelState.IsValid)
            {
                sucursal.Id = Guid.NewGuid();
                _context.Add(sucursal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sucursal);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
            {
                return NotFound();
            }
            return View(sucursal);
        }

        // POST: Sucursales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nombre,Direccion,Telefono,Email")] Sucursal sucursal)
        {
            if (id != sucursal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sucursal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SucursalExists(sucursal.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sucursal);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }

        // POST: Sucursales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var sucursal = _context.Sucursales
                .Include(s => s.StockItems)
                .FirstOrDefault(sucursal => sucursal.Id == id);
            
            if(sucursal.StockItems.Count == 0)
            {
                _context.Sucursales.Remove(sucursal);
                _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Error));
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificarStock(Guid sucursalid)
        {
            var username = User.Identity.Name;

            var carrito = _context.Carritos
                .Include(c => c.Cliente)
                .Include(c => c.CarritoItems)
                .FirstOrDefault(c => c.Cliente.Email == username && c.Activo == true);

            if (carrito.CarritoItems.Count == 0) {
                TempData["Message"] = "Tu carrito esta vacio, por favor agrega productos";
                return RedirectToAction(nameof(CarritosController.SeleccioneSucursal), "Carritos");
            }

            var sucursales = _context.Sucursales
                .ToArray();

            var stockItems = _context.StockItems.ToList();

            if (sucursalid == null) {
                sucursalid = sucursales[0].Id;
            }

            var hayStock = true;
            foreach (CarritoItem item in carrito.CarritoItems) { 
                var stockItemAux = stockItems
                    .FirstOrDefault(x => x.ProductoId == item.ProductoId && x.Sucursal.Id == sucursalid);
                if (stockItemAux != null)
                {
                    if (item.Cantidad > stockItemAux.Cantidad)
                    {
                        hayStock = false;
                    }
                }
                else { hayStock = false; }
                
            }

            if (hayStock)
            {
                carrito.Activo = false;
                foreach (CarritoItem item in carrito.CarritoItems)
                {
                    var stockItem = stockItems
                    .FirstOrDefault(x => x.ProductoId == item.ProductoId && x.Sucursal.Id == sucursalid);
                    if (item.Cantidad <= stockItem.Cantidad)
                    {
                        stockItem.Cantidad -= item.Cantidad;
                    }
                }
                
                var compraNueva = new Compra()
                { 
                    Id = Guid.NewGuid(),
                    Total = carrito.Subtotal,
                    ClienteId = carrito.Cliente.Id,
                    CarritoId = carrito.Id,
                    Fecha = DateTime.Now
                };
                _context.Add(compraNueva);

                var carritoNuevo = (new Carrito()
                { 
                    Id = Guid.NewGuid(),
                    Activo = true,
                    ClienteId = carrito.Cliente.Id,
                    CarritoItems = new List<CarritoItem>()
                });
                _context.Add(carritoNuevo);

                TempData["Sucursal"] = _context.Sucursales.FirstOrDefault(x => x.Id == sucursalid).Nombre;
                TempData["Compra"] = compraNueva.Id;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ComprasController.CompraExitosa), "Compras");
            }

            else {
                var hayStockEnSucursal = false;
                var hayStockEnSucursalBuscada = true;
                var sucursal = sucursales[0];
                int i = 0;
                while (!hayStockEnSucursal && i < sucursales.Length) {
                    sucursal = sucursales[i];
                    hayStockEnSucursalBuscada = true;
                    foreach (CarritoItem item in carrito.CarritoItems)
                    {
                        var stockItem = stockItems
                        .FirstOrDefault(x => x.ProductoId == item.ProductoId && x.Sucursal.Id == sucursal.Id);
                        if (stockItem != null)
                        {
                            if (item.Cantidad > stockItem.Cantidad)
                            {
                                hayStockEnSucursalBuscada = false;
                            }
                        }
                        else {
                            hayStockEnSucursalBuscada = false;
                        }
                    }
                         
                    if (hayStockEnSucursalBuscada) {
                        hayStockEnSucursal = true;
                    }
                    i++;
                }
                if (hayStockEnSucursal)
                {
                    TempData["Message"] = "Solo tenemos stock en " + sucursal.Nombre;

                }
                else {
                    TempData["Message"] = "No tenemos Stock en ninguna sucursal";
                }
                
                var carritoDbContext = _context.Carritos.Include(c => c.Cliente);
                return RedirectToAction(nameof(CarritosController.SeleccioneSucursal), "Carritos");
            };
        }

        public IActionResult Error(Guid? id)
        {
            return View();
        }

        private bool SucursalExists(Guid id)
        {
            return _context.Sucursales.Any(e => e.Id == id);
        }
    }
}
