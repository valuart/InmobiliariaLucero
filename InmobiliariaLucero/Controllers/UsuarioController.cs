using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        protected readonly IConfiguration configuration;
        private readonly IWebHostEnvironment envir;
        RepositorioUsuario ru;

        public UsuarioController(IConfiguration configuration, IWebHostEnvironment envir)
        {
            this.configuration = configuration;
            this.envir = (IWebHostEnvironment)envir;
            ru = new RepositorioUsuario(configuration);
        }

        // GET: UsuarioController
        // GET: Usuario
        [Authorize(Policy = "Administrador")]
        public ActionResult Index()
        {
            var lista = ru.ObtenerTodos();
            return View(lista);
        }

        // GET: Usuario/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
       [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario u)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: u.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    u.Clave = hashed;
                    u.Avatar = "";
                    u.Rol = User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador") ? u.Rol : (int)enRoles.Empleado;
                    var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                    var res = ru.Alta(u);
                    TempData["Id"] = u.IdUsuario;

                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(u);
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(u);
            }
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar usuario";
            var u = ru.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(u);
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario u)
        {
            var vista = "Edit";
            try
            {
                var usuario = ru.ObtenerPorId(id);
                if (User.IsInRole("Usuario"))
                {
                   
                    var usuarioActual = ru.ObtenerPorEmail(User.Identity.Name);
                    if (usuarioActual.IdUsuario != id)
                    {
                        return RedirectToAction(nameof(Index), "Home");
                    }
                    else
                    {
                        ru.Modificacion(u);
                        TempData["Mensaje"] = "Datos guardados correctamente";
                        return RedirectToAction(nameof(Index));
                    }

                }
                // TODO: Add update logic here
                u.Clave = usuario.Clave;
                u.Avatar = usuario.Avatar;
                ru.Modificacion(u);
                TempData["Mensaje"] = "Datos guardados correctamente";

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(vista, u);
            }
        }



        // GET: Usuario/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var u = ru.ObtenerPorId(id);
            return View(u);
        }

        // POST: Usuario/Delete/5
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Usuario usuario)
        {
            try
            {
                ru.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(usuario);
            }
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Usuario/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Login l)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: l.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var user = ru.ObtenerPorEmail(l.Email);
                if (user == null || user.Clave != hashed)
                {
                    ViewBag.Mensaje = "Datos inválidos";
                    return View();
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("FullName", user.Nombre + " " + user.Apellido),
                    new Claim(ClaimTypes.Role, user.RolNombre),
                    new Claim("IdUsuario", user.IdUsuario + ""),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                return RedirectToAction(nameof(Index), "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [AllowAnonymous]
        [Route("salir", Name = "logout")]
        // GET: Home/Logout
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
