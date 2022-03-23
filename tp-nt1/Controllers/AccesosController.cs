using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tp_nt1.Database;
using tp_nt1.Models;
using tp_nt1.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace tp_nt1.Controllers
{
    public class AccesosController : Controller
    {

        private readonly CarritoDbContext _context;
        private const string _Return_Url = "ReturnUrl";
        public AccesosController(CarritoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Ingresar(string returnUrl)
        {
            TempData[_Return_Url] = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Ingresar(string username, string password, Rol rol)
        {
            string returnUrl = TempData[_Return_Url] as string;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                Usuario usuario = null;

                if (rol == Rol.Cliente)
                {
                    usuario = _context.Clientes.FirstOrDefault(cliente => cliente.Email == username);
                }
                else if (rol == Rol.Administrador)
                {
                    usuario = _context.Administradores.FirstOrDefault(administrador => administrador.Email == username);
                }
                else if(rol == Rol.Empleado)
                {
                    usuario = _context.Empleados.FirstOrDefault(empleado => empleado.Email == username);
                }

                if (usuario != null)
                {
                    var passwordEncriptada = password.Encriptar();

                    if (usuario.Password.SequenceEqual(passwordEncriptada))
                    {
                        
                        ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                        identity.AddClaim(new Claim(ClaimTypes.Name, username));

                        identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Rol.ToString()));

                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));

                        identity.AddClaim(new Claim(ClaimTypes.GivenName, usuario.Nombre));

                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait(); //aca se loguea

                        TempData["LoggedIn"] = true;

                        if (!string.IsNullOrWhiteSpace(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
            }
            ViewBag.Error = "Alguno de los datos ingresados es incorrecto";
            ViewBag.UserName = username;
            TempData[_Return_Url] = returnUrl;

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Salir()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult NoAutorizado()
        {
            return View();
        }
    }
}
