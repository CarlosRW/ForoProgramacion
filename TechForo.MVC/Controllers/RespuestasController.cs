using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using TechForo.Core.Business;
using TechForo.Data.Entidades;
using TechForo.Models.Vista_de_modelos;

namespace TechForo.MVC.Controllers
{
    public class RespuestasController : Controller
    {
        private readonly RespuestaBusiness _respuestaBusiness;
        private readonly PreguntaBusiness _preguntaBusiness;

        public RespuestasController()
        {
            _respuestaBusiness = new RespuestaBusiness();
            _preguntaBusiness = new PreguntaBusiness();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RespuestaModel model, HttpPostedFileBase imagen)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            var pregunta = _preguntaBusiness.ObtenerPorId(model.PreguntaID);

            if (pregunta == null)
                return HttpNotFound();

            bool tieneImagen = imagen != null && imagen.ContentLength > 0;

            if (string.IsNullOrWhiteSpace(model.Contenido) &&
                string.IsNullOrWhiteSpace(model.Codigo) &&
                !tieneImagen)
            {
                TempData["ErrorRespuesta"] = "Debe escribir una respuesta, pegar un bloque de código o subir una imagen.";
                return RedirectToAction("Details", "Preguntas", new { id = model.PreguntaID });
            }

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            Respuesta respuesta = new Respuesta
            {
                Contenido = model.Contenido,
                Codigo = model.Codigo,
                ImagenUrl = GuardarImagen(imagen),
                PreguntaID = model.PreguntaID,
                UsuarioID = usuarioID
            };

            _respuestaBusiness.Crear(respuesta);

            return RedirectToAction("Details", "Preguntas", new { id = model.PreguntaID });
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return RedirectToAction("Index", "Preguntas");

            var respuesta = _respuestaBusiness.ObtenerPorId(id.Value);

            if (respuesta == null)
                return HttpNotFound();

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            if (respuesta.UsuarioID != usuarioID)
                return RedirectToAction("Details", "Preguntas", new { id = respuesta.PreguntaID });

            RespuestaModel model = new RespuestaModel
            {
                RespuestaID = respuesta.RespuestaID,
                Contenido = respuesta.Contenido,
                Codigo = respuesta.Codigo,
                ImagenUrl = respuesta.ImagenUrl,
                FechaCreacion = respuesta.FechaCreacion,
                UsuarioID = respuesta.UsuarioID,
                PreguntaID = respuesta.PreguntaID,
                UsuarioNombre = respuesta.UsuarioNombre
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RespuestaModel model, HttpPostedFileBase imagen)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            var respuestaActual = _respuestaBusiness.ObtenerPorId(model.RespuestaID);

            if (respuestaActual == null)
                return HttpNotFound();

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            if (respuestaActual.UsuarioID != usuarioID)
                return RedirectToAction("Details", "Preguntas", new { id = respuestaActual.PreguntaID });

            bool tieneImagenNueva = imagen != null && imagen.ContentLength > 0;

            if (string.IsNullOrWhiteSpace(model.Contenido) &&
                string.IsNullOrWhiteSpace(model.Codigo) &&
                string.IsNullOrWhiteSpace(respuestaActual.ImagenUrl) &&
                !tieneImagenNueva)
            {
                ModelState.AddModelError("", "Debe escribir una respuesta, pegar un bloque de código o subir una imagen.");

                model.ImagenUrl = respuestaActual.ImagenUrl;
                model.PreguntaID = respuestaActual.PreguntaID;

                return View(model);
            }

            string imagenUrl = respuestaActual.ImagenUrl;

            if (tieneImagenNueva)
                imagenUrl = GuardarImagen(imagen);

            Respuesta respuesta = new Respuesta
            {
                RespuestaID = model.RespuestaID,
                Contenido = model.Contenido,
                Codigo = model.Codigo,
                ImagenUrl = imagenUrl,
                PreguntaID = respuestaActual.PreguntaID,
                UsuarioID = usuarioID
            };

            _respuestaBusiness.Actualizar(respuesta);

            return RedirectToAction("Details", "Preguntas", new { id = respuestaActual.PreguntaID });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return RedirectToAction("Index", "Preguntas");

            var respuesta = _respuestaBusiness.ObtenerPorId(id.Value);

            if (respuesta == null)
                return HttpNotFound();

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            if (respuesta.UsuarioID != usuarioID)
                return RedirectToAction("Details", "Preguntas", new { id = respuesta.PreguntaID });

            int preguntaID = respuesta.PreguntaID;

            _respuestaBusiness.Eliminar(id.Value, usuarioID);

            return RedirectToAction("Details", "Preguntas", new { id = preguntaID });
        }

        private string GuardarImagen(HttpPostedFileBase imagen)
        {
            if (imagen == null || imagen.ContentLength == 0)
                return "";

            string carpeta = Server.MapPath("~/Uploads/Respuestas/");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            imagen.SaveAs(rutaCompleta);

            return "/Uploads/Respuestas/" + nombreArchivo;
        }
    }
}