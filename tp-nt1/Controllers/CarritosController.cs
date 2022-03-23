using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tp_nt1.Controllers;
using tp_nt1.Database;
using tp_nt1.Models;

namespace tp_nt1a_3.Controllers
{
    [Authorize]
    public class CarritosController : Controller
    {
        private readonly CarritoDbContext _context;

        public CarritosController(CarritoDbContext context)
        {
            _context = context;
        }

        // GET: Carritos
        public IActionResult Index()
        {
            ViewBag.Sucursales = new SelectList(_context.Sucursales, nameof(Sucursal.Id), nameof(Sucursal.Nombre));
            var username = User.Identity.Name;
            var carrito = _context.Carritos
                .Include(c => c.Cliente)
                .Include(c => c.CarritoItems)
                .FirstOrDefault(c => c.Cliente.Email == username && c.Activo == true);
            if (carrito != null)
            {
                var itemsCarrito = _context
                    .CarritoItems
                    .Include(itemsCarrito => itemsCarrito.Producto)
                    .Where(itemsCarrito => itemsCarrito.CarritoId == carrito.Id);
                return View(itemsCarrito);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> SeleccioneSucursal()
        {
            ViewBag.Sucursales = new SelectList(_context.Sucursales, nameof(Sucursal.Id), nameof(Sucursal.Nombre));
            var carritoDbContext = _context.Carritos.Include(c => c.Cliente);
            return View(await carritoDbContext.ToListAsync());
        }


        // GET: Carritos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos
                .Include(carrito => carrito.CarritoItems).ThenInclude(carritoItem => carritoItem.Producto).ThenInclude(producto => producto.Nombre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }
            ViewBag.Sucursales = new SelectList(_context.Sucursales, nameof(Sucursal.Id), nameof(Sucursal.Nombre));
            return View(carrito);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AgregarCarritoItem(Guid productoId, int cantidad)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(AccesosController.Ingresar), "Accesos");
            }
            var producto = await _context.Productos.FirstOrDefaultAsync(producto => producto.Id == productoId);
            var username = User.Identity.Name;
            var carrito = _context.Carritos
                .Include(c => c.Cliente)
                .Include(c => c.CarritoItems)
                .FirstOrDefault(c => c.Cliente.Email == username && c.Activo == true);
            if (cantidad == 0) {
                cantidad = 1;
            }

            var item = new CarritoItem()
            {
                Id = Guid.NewGuid(),
                ProductoId = productoId,
                CarritoId = carrito.Id,
                Cantidad = cantidad,
                ValorUnitario = producto.PrecioVigente,
                Subtotal = cantidad * producto.PrecioVigente,
            };
            _context.Add(item);
            carrito.Subtotal += item.Subtotal;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RemoverCarritoItem(Guid carritoItemId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(AccesosController.Ingresar), "Accesos");
            }

            var username = User.Identity.Name;
            var carrito = _context.Carritos
                .Include(c => c.Cliente)
                .Include(c => c.CarritoItems)
                .FirstOrDefault(c => c.Cliente.Email == username && c.Activo == true);
            var carritoItem = await _context.CarritoItems.FindAsync(carritoItemId);

            carrito.Subtotal -= carritoItem.Subtotal;

            _context.CarritoItems.Remove(carritoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> VaciarCarrito()
        {

            var username = User.Identity.Name;
            var carrito = _context.Carritos
                .Include(c => c.Cliente)
                .Include(c => c.CarritoItems)
                .FirstOrDefault(c => c.Cliente.Email == username && c.Activo == true);
            foreach (CarritoItem i in carrito.CarritoItems)
            {
                _context.CarritoItems.Remove(i);
            }
            carrito.Subtotal = 0;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Carritos/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido");
            return View();
        }

        // POST: Carritos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Empleado")]
        public async Task<IActionResult> Create([Bind("Id,Activo,Subtotal,ClienteId")] Carrito carrito)
        {
            if (ModelState.IsValid)
            {
                carrito.Id = Guid.NewGuid();
                _context.Add(carrito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }

        // GET: Carritos/Edit/5
        [Authorize(Roles = "Administrador, Empleado")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos.FindAsync(id);
            if (carrito == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }

        // POST: Carritos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Empleado")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Activo,Subtotal,ClienteId")] Carrito carrito)
        {
            if (id != carrito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }

        // GET: Carritos/Delete/5
        [Authorize(Roles = "Administrador, Empleado")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Empleado")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var carrito = await _context.Carritos.FindAsync(id);
            _context.Carritos.Remove(carrito);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoExists(Guid id)
        {
            return _context.Carritos.Any(e => e.Id == id);
        }
    }
}
