using InmobiliariaLucero.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;


namespace InmobiliariaLucero.Controllers
{
    public class InquilinoController : Controller
    {
        protected readonly IConfiguration configuration;
        RepositorioInquilino rinq;

        public InquilinoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            rinq = new RepositorioInquilino(configuration);
        }



        // GET: InquilinoController
        public ActionResult Index(int id)
        {
         
            var lista = rinq.ObtenerTodos();
            return View(lista);
        }

        // GET: InquilinoController/Details/5
        public ActionResult Details(int id)
        {
            var sujeto = rinq.ObtenerPorId(id);
            return View(sujeto);

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
                rinq.Alta(inquilino);
                TempData["Id"] = "creó el inquilino";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inquilino);
            }
        }

        // GET: InquilinoController/Edit/5
        public ActionResult Edit(int id)
        {
            var sujeto = rinq.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(sujeto);

        }

        // POST: InquilinoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                rinq.Modificacion(inquilino);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inquilino);
            }
        }

        // GET: InquilinoController/Delete/5
        public ActionResult Delete(int id)
        {
            var sujeto = rinq.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(sujeto);

        }

        // POST: InquilinoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inquilino inquilino)
        {
            try
            {
                rinq.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inquilino);
            }
        }
    }
}
