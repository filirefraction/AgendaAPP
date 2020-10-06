using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;
using System.Web.Mvc;
using AgendaSalas.Models;

namespace AgendaSalas.Controllers
{
    public class ReservacionesController : Controller
    {
        private AreasEntities db = new AreasEntities();

        // GET: Reservaciones
        public ActionResult Index()
        {
            ViewBag.ListSucursal = new SelectList(db.Sucursales, "IDSucursal", "DescSucursal");
            var reservacion = db.Reservacion.Include(r => r.Salas).Include(r => r.Usuarios);
            return View(reservacion.ToList());
        }

        // GET: Reservaciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = db.Reservacion.Find(id);
            if (reservacion == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/Reservaciones/Details.cshtml", reservacion);
        }

        // GET: Reservaciones/Create
        public ActionResult Create(int id)
        {
            ViewBag.IDSala = new SelectList(db.Salas, "IDSala", "DescSala", id);
            ViewBag.IDUsuario = new SelectList(db.Usuarios, "IDUsuario", "UsuarioNombre");
            return PartialView("~/Views/Reservaciones/Create.cshtml");
        }

        public JsonResult CreaReserva(Reservacion reservacion)
        {
            Resultados resultados = new Resultados();
            reservacion.DTRegistro = DateTime.Now;
            reservacion.Estatus = true;

            
            bool ReservaValida;
            bool IniTerReser;
            bool TerIniReser;

            //Validar que la fecha de Inicio sea mayor a la hora actual
            if (reservacion.DTInicio > DateTime.Now)
            {
                //Obtiene registros de reservaciones registrados en la base de Datos
                var reserva = (from r in db.Reservacion
                               where r.IDSala == reservacion.IDSala && r.DTInicio >= reservacion.DTInicio && r.DTInicio <= reservacion.DTFin ||
                                     r.IDSala == reservacion.IDSala && r.DTFin >= reservacion.DTInicio && r.DTFin <= reservacion.DTFin
                               orderby r.DTInicio
                               select r).ToList();

                switch (reserva.Count())
                {
                    case 0:
                        //No hay confilctos de Fecha y hora en la base de datos por lo tanto registra la reserva
                        try
                        {
                            db.Reservacion.Add(reservacion);
                            db.SaveChanges();
                            resultados.Result = true;
                        }
                        catch (Exception ex)
                        {
                            resultados.Result = false;
                            resultados.Mensaje = ex.Message;
                        }
                        break;
                    case 1:
                        //SI el evento que se quiere registrar empieza cuando termina uno existente o si termina cuando inicia un evento existente 
                        if (reservacion.DTInicio == reserva[0].DTFin || reservacion.DTFin == reserva[0].DTInicio)
                            ReservaValida = true;
                        else
                            ReservaValida = false;

                        if (ReservaValida == true)
                        {
                            try
                            {
                                db.Reservacion.Add(reservacion);
                                db.SaveChanges();
                                resultados.Result = true;
                            }
                            catch (Exception ex)
                            {
                                resultados.Result = false;
                                resultados.Mensaje = ex.Message;
                            }
                        }
                        else
                        {
                            resultados.Result = false;
                            resultados.Mensaje = "La sala ya está reservada en el horario indicado, elija otro horario o contacte con el administrador";
                        }
                        break;
                    case 2:
                        //verifica si las horas quedan entre reservas de horarios existentes
                        if (reservacion.DTInicio == reserva[0].DTFin)
                            IniTerReser = true;
                        else
                            IniTerReser = false;

                        if (reservacion.DTFin == reserva[1].DTInicio)
                            TerIniReser = true;
                        else
                            TerIniReser = false;

                        //si ambas coincidencias son ciertas significa que es un evento entre horas
                        if (IniTerReser == true && TerIniReser == true)
                        {
                            try
                            {
                                db.Reservacion.Add(reservacion);
                                db.SaveChanges();
                                resultados.Result = true;
                            }
                            catch (Exception ex)
                            {
                                resultados.Result = false;
                                resultados.Mensaje = ex.Message;
                            }
                        }
                        else
                        {
                            resultados.Result = false;
                            resultados.Mensaje = "La sala ya está reservada en el horario indicado, elija otro horario o contacte con el administrador";
                        }
                        break;
                    default:
                        resultados.Result = false;
                        resultados.Mensaje = "La sala ya está reservada en el horario indicado, elija otro horario o contacte con el administrador";
                        break;
                }

            }
            else
            {
                resultados.Result = false;
                resultados.Mensaje = "No puedes reservar fechas y hora anteriores a la actual";
            }

            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        // GET: Reservaciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = db.Reservacion.Find(id);
            if (reservacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDSala = new SelectList(db.Salas, "IDSala", "DescSala", reservacion.IDSala);
            ViewBag.IDUsuario = new SelectList(db.Usuarios, "IDUsuario", "UsuarioNombre", reservacion.IDUsuario);
            return PartialView("~/Views/Reservaciones/Edit.cshtml", reservacion);
        }

        public JsonResult EditReserva(Reservacion reservacion)
        {
            Resultados resultados = new Resultados();

            bool ReservaValida;
            bool IniTerReser;
            bool TerIniReser;

            //Validar que la fecha de Inicio sea mayor a la hora actual
            if (reservacion.DTInicio > DateTime.Now)
            {
                //Obtiene registros de reservaciones registrados en la base de Datos
                var reserva = (from r in db.Reservacion
                               where r.IDReservacion != reservacion.IDReservacion && r.IDSala == reservacion.IDSala && r.DTInicio >= reservacion.DTInicio && r.DTInicio <= reservacion.DTFin ||
                                     r.IDReservacion != reservacion.IDReservacion && r.IDSala == reservacion.IDSala && r.DTFin >= reservacion.DTInicio && r.DTFin <= reservacion.DTFin
                               orderby r.DTInicio
                               select r).ToList();

                switch (reserva.Count())
                {
                    case 0:
                        //No hay confilctos de Fecha y hora en la base de datos por lo tanto registra la reserva
                        try
                        {
                            db.Entry(reservacion).State = EntityState.Modified;
                            db.SaveChanges();
                            resultados.Result = true;
                        }
                        catch (Exception ex)
                        {
                            resultados.Result = false;
                            resultados.Mensaje = ex.Message;
                        }
                        break;
                    case 1:
                        //SI el evento que se quiere registrar empieza cuando termina uno existente o si termina cuando inicia un evento existente 
                        if (reservacion.DTInicio == reserva[0].DTFin || reservacion.DTFin == reserva[0].DTInicio)
                            ReservaValida = true;
                        else
                            ReservaValida = false;

                        if (ReservaValida == true)
                        {
                            try
                            {
                                db.Entry(reservacion).State = EntityState.Modified;
                                db.SaveChanges();
                                resultados.Result = true;
                            }
                            catch (Exception ex)
                            {
                                resultados.Result = false;
                                resultados.Mensaje = ex.Message;
                            }
                        }
                        else
                        {
                            resultados.Result = false;
                            resultados.Mensaje = "La sala ya está reservada en el horario indicado, elija otro horario o contacte con el administrador";
                        }
                        break;
                    case 2:
                        //verifica si las horas quedan entre reservas de horarios existentes
                        if (reservacion.DTInicio == reserva[0].DTFin)
                            IniTerReser = true;
                        else
                            IniTerReser = false;

                        if (reservacion.DTFin == reserva[1].DTInicio)
                            TerIniReser = true;
                        else
                            TerIniReser = false;

                        //si ambas coincidencias son ciertas significa que es un evento entre horas
                        if (IniTerReser == true && TerIniReser == true)
                        {
                            try
                            {
                                db.Entry(reservacion).State = EntityState.Modified;
                                db.SaveChanges();
                                resultados.Result = true;
                            }
                            catch (Exception ex)
                            {
                                resultados.Result = false;
                                resultados.Mensaje = ex.Message;
                            }
                        }
                        else
                        {
                            resultados.Result = false;
                            resultados.Mensaje = "La sala ya está reservada en el horario indicado, elija otro horario o contacte con el administrador";
                        }
                        break;
                    default:
                        resultados.Result = false;
                        resultados.Mensaje = "La sala ya está reservada en el horario indicado, elija otro horario o contacte con el administrador";
                        break;
                }

            }
            else
            {
                resultados.Result = false;
                resultados.Mensaje = "No puedes reservar fechas y hora anteriores a la actual";
            }

            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteReserva(int id)
        {
            Reservacion reservacion = db.Reservacion.Find(id);
            Resultados resultado = new Resultados();

            try
            {
                db.Reservacion.Remove(reservacion);
                db.SaveChanges();
                resultado.Mensaje = "Reservación Eliminada Correctamente";
            }
            catch (Exception ex)
            {
                resultado.Mensaje = ex.Message;
            }

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaAreas(int id)
        {
            var areas = (from a in db.Areas
                         where a.IDSucursal == id
                         select new
                         {
                             idarea = a.IDArea,
                             descarea = a.DescArea
                         }).ToList();

            return Json(areas, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaSalas(int id)
        {
            var salas = (from s in db.Salas
                         where s.IDArea == id
                         select new
                         {
                             idsala = s.IDSala,
                             descsala = s.DescSala
                         }).ToList();

            return Json(salas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Agenda(int id)
        {
            ViewBag.idsala = id;
            return PartialView("~/Views/Reservaciones/Agenda.cshtml");
        }

        public JsonResult GetEvents(int id)
        {
            var eventos = (from e in db.Reservacion join
                                u in db.Usuarios on e.IDUsuario equals u.IDUsuario
                           where e.IDSala == id
                           select new
                           {
                               id = e.IDReservacion,
                               titulo = e.DescReserv,
                               descripcion = " Creado: " + e.DTRegistro.ToString(),
                               inicio = e.DTInicio,
                               fin = e.DTFin,
                               temacolor = "blue",
                               diacompleto = false,
                               userid = e.IDUsuario,
                               username = u.UsuarioNombre + " " + u.UsuarioApellido1 + " " + u.UsuarioApellido2
                           }).ToList();

            return Json(eventos, JsonRequestBehavior.AllowGet);
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
