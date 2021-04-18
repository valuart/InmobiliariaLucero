using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;


namespace InmobiliariaLucero.Controllers
{
    public class InmuebleController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioInmueble ri;
        RepositorioPropietario rp;

        public InmuebleController(IConfiguration configuration)
        {
            this.configuration = configuration;
            ri = new RepositorioInmueble(configuration);
            rp = new RepositorioPropietario(configuration);
        }
        // GET: InmuebleController
        public ActionResult Index(int id)
        {
            ViewBag.IdSeleccionado = id;
            var lista = ri.ObtenerTodos();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
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
                ri.Alta(inmueble);
                TempData["Id"] = "creó el inmueble";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                ViewBag.propis = rp.ObtenerTodos();
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
        public ActionResult Delete(int id)
        {
            var sujeto = ri.ObtenerPorId(id);
            ViewBag.lugar = sujeto.Propietario.Nombre + " " + sujeto.Propietario.Apellido + " en " + sujeto.Direccion;
            return View(sujeto);

        }

        // POST: InmuebleController/Delete/5
        [HttpPost]
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
