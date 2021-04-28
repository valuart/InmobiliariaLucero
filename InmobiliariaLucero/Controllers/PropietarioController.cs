﻿using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;


namespace InmobiliariaLucero.Controllers
{
  ///  [Authorize]
    public class PropietarioController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioPropietario rp;
        private readonly RepositorioInmueble ri;

        public PropietarioController(IConfiguration configuration)
        {
            ri = new RepositorioInmueble(configuration);
            rp = new RepositorioPropietario(configuration);
            this.configuration = configuration;
        }


        // GET: PropietarioController
        public ActionResult Index(int id)
        {
          
            var lista = rp.ObtenerTodos();
            return View(lista);
        }

        // GET: PropietarioController/Details/5
        public ActionResult Details(int id)
        {
            var sujeto = rp.ObtenerPorId(id);
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
        public ActionResult Create(Propietario p)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    int res = rp.Alta(p);
                    TempData["Id"] = p.IdPropietario;
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(p);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        // GET: PropietarioController/Edit/5
        public ActionResult Edit(int id)
        {
            var sujeto = rp.ObtenerPorId(id);
           
            return View(sujeto);
        }

        // POST: PropietarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietario p)
        {
            try
            {
              
                rp.Modificacion(p);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        // GET: PropietarioController/Delete/5
      //  [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var sujeto = rp.ObtenerPorId(id);
          
            return View(sujeto);
        }

        // POST: PropietarioController/Delete/5
        [HttpPost]
      //  [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Propietario p)
        {
            try
            {
            
                rp.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }
    }
}
