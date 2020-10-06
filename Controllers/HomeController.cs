using AgendaSalas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgendaSalas.Controllers
{
    public class HomeController : Controller
    {
        private AreasEntities db = new AreasEntities();

        // GET: Home
        public ActionResult Index(int? error)
        {
            ViewBag.error = error;
            return View();
        }

        [HttpPost]
        public ActionResult Iniciar(string uxu, string C0m)
        {
            var inputs = db.Usuarios.Where(x => x.EmpleadoEmail == uxu && x.Pass == C0m).ToList();

            if (inputs.Count > 0)
            {
                if (inputs[0].Estatus == true)
                {
                    Session["IdUsuario"] = inputs[0].IDUsuario;
                    Session["Nombre"] = inputs[0].UsuarioNombre;
                    Session["Tipo"] = inputs[0].Tipo;
                    return RedirectToAction("Menu", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { error = 1 });
                }
            }
            else
            {
                return RedirectToAction("Index", "Home", new { error = 1 });
            }
        }

        public ActionResult Cerrar()
        {
                Session.Abandon();
                return RedirectToAction("Index", "Home");
                   }

        public ActionResult Menu()
        {
            return View();
        }
    }
}