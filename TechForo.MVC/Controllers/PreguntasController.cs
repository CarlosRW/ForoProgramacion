using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using TechForo.Core.Business;
using TechForo.Data.Entidades;
using TechForo.Models.Vista_de_modelos;

namespace TechForo.MVC.Controllers
{
    public class PreguntasController : Controller
    {
        private readonly PreguntaBusiness _preguntaBusiness;
        private readonly RespuestaBusiness _respuestaBusiness;

        public PreguntasController()
        {
            _preguntaBusiness = new PreguntaBusiness();
            _respuestaBusiness = new RespuestaBusiness();
        }

        public ActionResult Index()
        {
            var preguntas = _preguntaBusiness.ObtenerTodas();
            return View(preguntas);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var pregunta = _preguntaBusiness.ObtenerPorId(id.Value);

            if (pregunta == null)
                return HttpNotFound();

            var respuestas = _respuestaBusiness.ObtenerPorPregunta(id.Value);

            PreguntaDetalleViewModel model = new PreguntaDetalleViewModel
            {
                PreguntaID = pregunta.PreguntaID,
                Titulo = pregunta.Titulo,
                Descripcion = pregunta.Descripcion,
                Codigo = pregunta.Codigo,
                ImagenUrl = pregunta.ImagenUrl,
                FechaCreacion = pregunta.FechaCreacion,
                UsuarioID = pregunta.UsuarioID,
                UsuarioNombre = pregunta.UsuarioNombre,
                Respuestas = new List<RespuestaModel>()
            };

            foreach (var respuesta in respuestas)
            {
                model.Respuestas.Add(new RespuestaModel
                {
                    RespuestaID = respuesta.RespuestaID,
                    Contenido = respuesta.Contenido,
                    Codigo = respuesta.Codigo,
                    ImagenUrl = respuesta.ImagenUrl,
                    FechaCreacion = respuesta.FechaCreacion,
                    UsuarioID = respuesta.UsuarioID,
                    PreguntaID = respuesta.PreguntaID,
                    UsuarioNombre = respuesta.UsuarioNombre
                });
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            return View(new PreguntaModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PreguntaModel model, HttpPostedFileBase imagen)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            Pregunta pregunta = new Pregunta
            {
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                Codigo = model.Codigo,
                ImagenUrl = GuardarImagen(imagen),
                UsuarioID = usuarioID
            };

            _preguntaBusiness.Crear(pregunta);

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            var pregunta = _preguntaBusiness.ObtenerPorId(id);

            if (pregunta == null)
                return HttpNotFound();

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            if (pregunta.UsuarioID != usuarioID)
                return RedirectToAction("Index");

            PreguntaModel model = new PreguntaModel
            {
                PreguntaID = pregunta.PreguntaID,
                Titulo = pregunta.Titulo,
                Descripcion = pregunta.Descripcion,
                Codigo = pregunta.Codigo,
                ImagenUrl = pregunta.ImagenUrl
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PreguntaModel model, HttpPostedFileBase imagen)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var preguntaActual = _preguntaBusiness.ObtenerPorId(model.PreguntaID);

            if (preguntaActual == null)
                return HttpNotFound();

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            if (preguntaActual.UsuarioID != usuarioID)
                return RedirectToAction("Index");

            string imagenUrl = preguntaActual.ImagenUrl;

            if (imagen != null && imagen.ContentLength > 0)
                imagenUrl = GuardarImagen(imagen);

            Pregunta pregunta = new Pregunta
            {
                PreguntaID = model.PreguntaID,
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                Codigo = model.Codigo,
                ImagenUrl = imagenUrl,
                UsuarioID = usuarioID
            };

            _preguntaBusiness.Actualizar(pregunta);

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            var pregunta = _preguntaBusiness.ObtenerPorId(id);

            if (pregunta == null)
                return HttpNotFound();

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            if (pregunta.UsuarioID != usuarioID)
                return RedirectToAction("Index");

            _preguntaBusiness.Eliminar(id, usuarioID);

            return RedirectToAction("Index");
        }

        private string GuardarImagen(HttpPostedFileBase imagen)
        {
            if (imagen == null || imagen.ContentLength == 0)
                return "";

            string carpeta = Server.MapPath("~/Uploads/Preguntas/");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            imagen.SaveAs(rutaCompleta);

            return "/Uploads/Preguntas/" + nombreArchivo;
        }
    }
}