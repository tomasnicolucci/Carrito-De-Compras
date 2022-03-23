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
    [Authorize]
    public class ComprasController : Controller
    {
        private readonly CarritoDbContext _context;

        public ComprasController(CarritoDbContext context)
        {
            _context = context;
        }

        // GET: Compras
        [Authorize(Roles = "Administrador, Empleado")]
        public async Task<IActionResult> Index()
        {
            var carritoDbContext = _context.Compras.Include(c => c.Carrito).Include(c => c.Cliente);
            return View(await carritoDbContext.ToListAsync());
        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        public async Task<IActionResult> ComprasDelMes()
        {
            var compras = await _context.Compras
                .Where(compra => compra.Fecha.Year == DateTime.Today.Year && compra.Fecha.Month == DateTime.Today.Month)
                .OrderByDescending(compra => (double)compra.Total)
                .ToListAsync();
            return View(compras);
        }

        public async Task<IActionResult> CompraExitosa()
        {
            var username = User.Identity.Name;
            var cliente = _context.Clientes
                .FirstOrDefault(cliente => cliente.Email == username);

            Guid idCompra = Guid.Parse(TempData["Compra"].ToString());
            TempData["Sucu"] = TempData["Sucursal"].ToString();
            var compra = await _context.Compras
                .Include(c => c.Carrito).ThenInclude(c => c.CarritoItems)
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id && c.Id == idCompra);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // GET: Compras/Create
        public IActionResult Create()
        {
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id");
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido");
            return View();
        }

        // POST: Compras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Total,ClienteId,CarritoId")] Compra compra)
        {
            if (ModelState.IsValid)
            {
                compra.Id = Guid.NewGuid();
                _context.Add(compra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", compra.ClienteId);
            return View(compra);
        }

        // GET: Compras/Edit/5
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", compra.ClienteId);
            return View(compra);
        }

        // POST: Compras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Total,ClienteId,CarritoId")] Compra compra)
        {
            if (id != compra.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompraExists(compra.Id))
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
            ViewData["CarritoId"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoId);
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", compra.ClienteId);
            return View(compra);
        }

        // GET: Compras/Delete/5
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var compra = await _context.Compras.FindAsync(id);
            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompraExists(Guid id)
        {
            return _context.Compras.Any(e => e.Id == id);
        }

        public async Task<IActionResult> MisCompras()
        {
            var username = User.Identity.Name;
            var cliente = _context.Clientes.FirstOrDefault(cliente => cliente.Email == username);

            var carritoDbContext = _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Cliente)
                .Where(c => c.Cliente == cliente);
            return View(await carritoDbContext.ToListAsync());
        }
    }
}
