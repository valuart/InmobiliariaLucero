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
    public class PropietarioController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioPropietario repositorio;

        public PropietarioController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorio = new RepositorioPropietario(configuration);
        }



        // GET: PropietarioController
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: PropietarioController/Details/5
        public ActionResult Details(int id)
        {
            var sujeto = repositorio.ObtenerPorId(id);
            return View(sujeto);
            
        }

        // GET: PropietarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropietarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                repositorio.Alta(propietario);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(propietario);
            }
        }

        // GET: PropietarioController/Edit/5
        public ActionResult Edit(int id)
        {
            var sujeto = repositorio.ObtenerPorId(id);
            return View(sujeto);
        }

        // POST: PropietarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietario propietario)
        {
            try
            {
                repositorio.Modificacion(propietario);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(propietario);
            }
        }

        // GET: PropietarioController/Delete/5
        public ActionResult Delete(int id)
        {
            var sujeto = repositorio.ObtenerPorId(id);
            return View(sujeto);
        }

        // POST: PropietarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Propietario p)
        {
            try
            {
                repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(p);
            }
        }
    }
}
