using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;


namespace InmobiliariaLucero.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly RepositorioInmueble ri;
        private readonly RepositorioPropietario rp;
        private readonly RepositorioContrato rc;

        public InmuebleController(IConfiguration configuration, IWebHostEnvironment environment, RepositorioInmueble ri, RepositorioPropietario rp, RepositorioContrato rc )
        {
            this.configuration = configuration;
            this.environment = environment;
            this.ri = ri;
            this.rp = rp;
            this.rc = rc;
           
        }
        // GET: InmuebleController
        public ActionResult Index(int id)
        {
            var lista = ri.ObtenerTodos();
            return View(lista);

        }

        // GET: InmuebleController/Details/5
        public ActionResult Details(int id)
        {
            Inmueble i = new Inmueble();
            i = ri.ObtenerPorId(id);
            return View(i);

        }

        // GET: InmuebleController/Create
        public ActionResult Create()
        {
            ViewBag.Propietario = rp.ObtenerTodos();
            return View();
        }

        // POST: InmuebleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ri.Alta(inmueble);
                    TempData["Id"] = inmueble.IdInmueble;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Propietarios = rp.ObtenerTodos();
                    return View(inmueble);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inmueble);
            }
        }

        // GET: InmuebleController/Edit/5
        public ActionResult Edit(int id)
        {
            var sujeto = ri.ObtenerPorId(id);
            ViewBag.nombre = sujeto.Propietario.Nombre + " " + sujeto.Propietario.Apellido;
            return View(sujeto);
        }

        // POST: InmuebleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble inmueble)
        {
            try
            {
                inmueble.IdInmueble = id;
                ri.Modificacion(inmueble);
                TempData["Id"] = "actualizó el inmueble";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inmueble);
            }
        }

        // GET: InmuebleController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var sujeto = ri.ObtenerPorId(id);
            ViewBag.lugar = sujeto.Propietario.Nombre + " " + sujeto.Propietario.Apellido + " en " + sujeto.Direccion;
            return View(sujeto);

        }

        // POST: InmuebleController/Delete/5
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inmueble inmueble)
        {
            try
            {
                ri.Baja(id);
                TempData["Id"] = "eliminó el inmueble";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("The DELETE statement conflicted with the REFERENCE"))
                {
                    var sujeto = ri.ObtenerPorId(id);
                    ViewBag.lugar = sujeto.Propietario.Nombre + " " + sujeto.Propietario.Apellido + " en " + sujeto.Direccion;
                    ViewBag.Error = "No se puede eliminar el inmueble ya que tiene contratos a su nombre";
                }
                else
                {
                    ViewBag.Error = ex.Message;
                    ViewBag.StackTrate = ex.StackTrace;
                }
                return View(inmueble);
            }
        }
    }
}
