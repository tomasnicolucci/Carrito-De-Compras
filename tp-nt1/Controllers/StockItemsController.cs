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
    [Authorize(Roles = "Administrador, Empleado")]
    public class StockItemsController : Controller
    {
        private readonly CarritoDbContext _context;

        public StockItemsController(CarritoDbContext context)
        {
            _context = context;
        }

        // GET: StockItems
        public IActionResult Index(Guid? sucursalId)
        {
            var id = sucursalId.HasValue ? sucursalId : TempData["sucursalId"] as Guid?;
            TempData["sucursalId"] = id;
            TempData.Keep("sucursalId");
            var stock = _context
               .StockItems
               .Include(x => x.Producto)
               .Include(x => x.Sucursal)
               .Where(x => (!id.HasValue || x.SucursalId == id.Value))
                .ToList();
            return View(stock);

        }

        // GET: StockItems/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockItem = await _context.StockItems
                .Include(s => s.Producto)
                .Include(s => s.Sucursal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockItem == null)
            {
                return NotFound();
            }

            return View(stockItem);
        }

        // GET: StockItems/Create
        public IActionResult Create()
        {
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre");
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre");
            return View();
        }

        // POST: StockItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Cantidad,SucursalId,ProductoId")] StockItem stockItem)
        {
            if (ModelState.IsValid)
            {
                stockItem.Id = Guid.NewGuid();
                _context.Add(stockItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", stockItem.ProductoId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", stockItem.SucursalId);
            return View(stockItem);
        }

        // GET: StockItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockItem = await _context.StockItems.FindAsync(id);
            if (stockItem == null)
            {
                return NotFound();
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", stockItem.ProductoId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", stockItem.SucursalId);
            return View(stockItem);
        }

        // POST: StockItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Cantidad,SucursalId,ProductoId")] StockItem stockItem)
        {
            if (id != stockItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockItemExists(stockItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index),stockItem.SucursalId);
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", stockItem.ProductoId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", stockItem.SucursalId);
            return View(stockItem);
        }

        // GET: StockItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockItem = await _context.StockItems
                .Include(s => s.Producto)
                .Include(s => s.Sucursal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockItem == null)
            {
                return NotFound();
            }

            return View(stockItem);
        }

        // POST: StockItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var stockItem = await _context.StockItems.FindAsync(id);
            _context.StockItems.Remove(stockItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockItemExists(Guid id)
        {
            return _context.StockItems.Any(e => e.Id == id);
        }

    }
}
