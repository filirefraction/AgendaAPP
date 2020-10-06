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
    public class AreasController : Controller
    {
        private AreasEntities db = new AreasEntities();

        // GET: Areas
        public ActionResult Index()
        {
            var areas = db.Areas.Include(a => a.Sucursales);
            return View(areas.ToList());
        }

        // GET: Areas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Areas areas = db.Areas.Find(id);
            if (areas == null)
            {
                return HttpNotFound();
            }
            return View(areas);
        }

        // GET: Areas/Create
        public ActionResult Create()
        {
            ViewBag.IDSucursal = new SelectList(db.Sucursales, "IDSucursal", "DescSucursal");
            return View();
        }

        // POST: Areas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDArea,IDSucursal,DescArea")] Areas areas)
        {
            if (ModelState.IsValid)
            {
                db.Areas.Add(areas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDSucursal = new SelectList(db.Sucursales, "IDSucursal", "DescSucursal", areas.IDSucursal);
            return View(areas);
        }

        // GET: Areas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Areas areas = db.Areas.Find(id);
            if (areas == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDSucursal = new SelectList(db.Sucursales, "IDSucursal", "DescSucursal", areas.IDSucursal);
            return View(areas);
        }

        // POST: Areas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDArea,IDSucursal,DescArea")] Areas areas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(areas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDSucursal = new SelectList(db.Sucursales, "IDSucursal", "DescSucursal", areas.IDSucursal);
            return View(areas);
        }

        // GET: Areas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Areas areas = db.Areas.Find(id);
            if (areas == null)
            {
                return HttpNotFound();
            }
            return View(areas);
        }

        // POST: Areas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Areas areas = db.Areas.Find(id);
            db.Areas.Remove(areas);
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
