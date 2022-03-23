using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tp_nt1.Database;
using tp_nt1.Models;
using Microsoft.AspNetCore.Authorization;
using tp_nt1.Extensions;
using tp_nt1.Controllers;

namespace tp_nt1a_3.Controllers
{
    [AllowAnonymous]
    public class ClientesController : Controller
    {
        private readonly CarritoDbContext _context;

        public ClientesController(CarritoDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DNI,Nombre,Apellido,Email,Telefono,Direccion")] Cliente cliente, String password, String Email)
        {
            try
            {
                password.ValidarPassword();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(Cliente.Password), ex.Message);
            }
            var user = _context.Clientes.FirstOrDefault(user => user.Email == Email);
            if (user!=null) {
                var message = "Este email ya esta en uso";
                ModelState.AddModelError(nameof(Cliente.Email), message);
            }

            if (ModelState.IsValid)
            {
                cliente.Id = Guid.NewGuid();
                cliente.FechaAlta = DateTime.Now;
                cliente.Password = password.Encriptar();
                cliente.Carrito = new List<Carrito>();
                cliente.Carrito.Add(new Carrito()
                { //Inicializamos el carrito con los valores default.
                    Id = Guid.NewGuid(),
                    Activo = true,
                    ClienteId = cliente.Id,
                    CarritoItems = new List<CarritoItem>()
                });
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("DNI,Id,Nombre,Apellido,Email,Telefono,Direccion,FechaAlta,Password")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        [HttpGet]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = nameof(Rol.Administrador))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(Guid id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpGet]
        public IActionResult EditMyself()
        {
            var username = User.Identity.Name;
            var cliente = _context.Clientes.FirstOrDefault(cliente => cliente.Email == username);

            return View(cliente);
        }

        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpPost]
        public IActionResult EditMyself(Cliente cliente, string pass)
        {
            if (!string.IsNullOrWhiteSpace(pass))
            {
                try
                {
                    pass.ValidarPassword();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(Cliente.Password), ex.Message);
                }
            }

            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var clienteDatabase = _context.Clientes.FirstOrDefault(cliente => cliente.Email == username);

                clienteDatabase.Nombre = cliente.Nombre;
                clienteDatabase.Apellido = cliente.Apellido;
                clienteDatabase.DNI = cliente.DNI;

                if (!string.IsNullOrWhiteSpace(pass))
                {
                    clienteDatabase.Password = pass.Encriptar();
                }

                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(cliente);
        }
    }
}
