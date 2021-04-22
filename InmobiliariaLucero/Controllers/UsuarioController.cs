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

        public UsuarioController(IConfiguration configuration)
        {
            this.configuration = configuration;
            envir = envir;
            ru = new RepositorioUsuario(configuration);
        }

        // GET: UsuarioController
        [Authorize(Policy = "Administrador")] 
       public ActionResult Index(int id)
        {
            var lista = ru.ObtenerTodos();
            return View(lista);
        }

        // GET: Admin/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: u.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                u.Clave = hashed;

                var nbreRnd = Guid.NewGuid();
                int res = ru.Alta(u);
              
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            var i = ru.ObtenerPorId(id);
            return View(i);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Usuario i)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    i.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: i.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    int res = ru.Modificacion(i);
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    ModelState.AddModelError("", "Error al actualizar!!");
                    return View();
                }


            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var i = ru.ObtenerPorId(id);
            return View(i);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Usuario i)
        {
            try
            {
                int res = ru.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                     password: login.Clave,
                     salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                     prf: KeyDerivationPrf.HMACSHA1,
                     iterationCount: 1000,
                     numBytesRequested: 256 / 8));

                    var e = ru.ObtenerPorEmail(login.Email);
                    if (e == null)
                    {
                        ModelState.AddModelError("", "El email y/o el password son incorrectos");
                        return View();
                    }
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                       // new Claim(ClaimTypes.Role, e.Rol),
                    };
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));


                    return RedirectToAction(nameof(Index), "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }

}


