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
    public class InquilinoController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioInquilino repositorio;

        public InquilinoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorio = new RepositorioInquilino(configuration);
        }



        // GET: InquilinoController
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: InquilinoController/Details/5
        public ActionResult Details(int id)
        {
            Inquilino i = new Inquilino();
            i = repositorio.ObtenerPorId(id);
            return View(i);
            
        }

        // GET: InquilinoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InquilinoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {
                repositorio.Alta(inquilino);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(inquilino);
            }
        }

        // GET: InquilinoController/Edit/5
        public ActionResult Edit(int id)
        {
            var sujeto = repositorio.ObtenerPorId(id);
            return View(sujeto);
         
        }

        // POST: InquilinoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                repositorio.Modificacion(inquilino);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(inquilino);
            }
        }

        // GET: InquilinoController/Delete/5
        public ActionResult Delete(int id)
        {
            var sujeto = repositorio.ObtenerPorId(id);
            return View(sujeto);
           
        }

        // POST: InquilinoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inquilino i)
        {
            try
            {
                repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View(i);
            }
        }
    }
}
