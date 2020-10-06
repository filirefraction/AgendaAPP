using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AgendaSalas.Models;

namespace AgendaSalas.Controllers
{
    public class SalasController : Controller
    {
        private AreasEntities db = new AreasEntities();

        // GET: Salas
        public ActionResult Index()
        {
            var salas = db.Salas.Include(s => s.Areas);
            return View(salas.ToList());
        }

        // GET: Salas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salas salas = db.Salas.Find(id);
            if (salas == null)
            {
                return HttpNotFound();
            }
            return View(salas);
        }

        // GET: Salas/Create
        public ActionResult Create()
        {
            ViewBag.IDArea = new SelectList(db.Areas, "IDArea", "DescArea");
            return View();
        }

        // POST: Salas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDSala,IDArea,DescSala,EspecificaHorario,HoraInicial,HoraFinal,Observaciones,Estatus")] Salas salas)
        {
            if (ModelState.IsValid)
            {
                db.Salas.Add(salas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDArea = new SelectList(db.Areas, "IDArea", "DescArea", salas.IDArea);
            return View(salas);
        }

        // GET: Salas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salas salas = db.Salas.Find(id);
            if (salas == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDArea = new SelectList(db.Areas, "IDArea", "DescArea", salas.IDArea);
            return View(salas);
        }

        // POST: Salas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDSala,IDArea,DescSala,EspecificaHorario,HoraInicial,HoraFinal,Observaciones,Estatus")] Salas salas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(salas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDArea = new SelectList(db.Areas, "IDArea", "DescArea", salas.IDArea);
            return View(salas);
        }

        // GET: Salas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salas salas = db.Salas.Find(id);
            if (salas == null)
            {
                return HttpNotFound();
            }
            return View(salas);
        }

        // POST: Salas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Salas salas = db.Salas.Find(id);
            db.Salas.Remove(salas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
