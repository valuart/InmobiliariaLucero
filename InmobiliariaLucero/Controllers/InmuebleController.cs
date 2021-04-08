using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Controllers
{
    public class InmuebleController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioInmueble repositorio;

        public InmuebleController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorio = new RepositorioInmueble(configuration);
        }
        // GET: InmuebleController
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: InmuebleController/Details/5
        public ActionResult Details(int id)
        {
            Inmueble i = new Inmueble();
            i = repositorio.ObtenerPorId(id);
            return View(i);

        }

        // GET: InmuebleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InmuebleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble inmueble)
        {
            try
            {
                repositorio.Alta(inmueble);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(inmueble);
            }
        }

        // GET: InmuebleController/Edit/5
        public ActionResult Edit(int id)
        {
            var sujeto = repositorio.ObtenerPorId(id);
            return View(sujeto);
        }

        // POST: InmuebleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble inmueble)
        {
            try
            {
                repositorio.Modificacion(inmueble);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(inmueble);
            }
        }

        // GET: InmuebleController/Delete/5
        public ActionResult Delete(int id)
        {
            var sujeto = repositorio.ObtenerPorId(id);
            return View(sujeto);
          
        }

        // POST: InmuebleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inmueble i)
        {
            try
            {
                repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(i);
            }
        }
    }
}
