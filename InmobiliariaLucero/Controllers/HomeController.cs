using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Controllers
{
    public class HomeController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioInmueble ri;
        RepositorioPropietario rp;
        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
            ri = new RepositorioInmueble(configuration);
            rp = new RepositorioPropietario(configuration);
        }
        public IActionResult Index()
        {
            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction(nameof(SiPermitido));
            }
            else if (User.IsInRole("Empleado"))
            {
                return RedirectToAction(nameof(NoPermitido));
            }
            else if (User.IsInRole("Propietario"))
            {
                return RedirectToAction(nameof(Privado));
            }
            else
            {
                return View();
            }
        }
        [Authorize(Policy = "Administrador")]
        public IActionResult SiPermitido()
        {
            return View();
        }
        [Authorize(Policy = "Propietario")]
        public IActionResult Privado()
        {
            Propietario p = rp.ObtenerPorEmail(User.Identity.Name);
            var lista = ri.BuscarPorPropietario(p.IdPropietario);
            return View(lista);

        }

        [Authorize(Policy = "Empleado")]
        public IActionResult NoPermitido()
        {
            return View();
        }
        public IActionResult Restringido()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
