using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using TechForo.Core.Business;
using TechForo.Data.Entidades;
using TechForo.Data.Repositorios;
using TechForo.Models.Vista_de_modelos;

namespace TechForo.MVC.Controllers
{
    // DP - MVC: este Controller recibe la peticion HTTP y delega las reglas de
    // negocio a RespuestaBusiness.
    // SOLID - SRP: aqui solo se coordinan sesion, navegacion, ModelState y los
    // archivos enviados por el navegador.
    public class RespuestasController : Controller
    {
        private readonly RespuestaBusiness _respuestaBusiness;
        private readonly PreguntaBusiness _preguntaBusiness;

        public RespuestasController()
        {
            _respuestaBusiness = new RespuestaBusiness(new RespuestaRepository());
            _preguntaBusiness = new PreguntaBusiness(new PreguntaRepository());
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

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            Respuesta respuesta = new Respuesta
            {
                Contenido = model.Contenido,
                Codigo = model.Codigo,
                ImagenUrl = GuardarImagen(imagen),
                PreguntaID = model.PreguntaID,
                UsuarioID = usuarioID
            };

            string mensajeError;

            if (!_respuestaBusiness.Crear(respuesta, out mensajeError))
            {
                TempData["ErrorRespuesta"] = mensajeError;
                return RedirectToAction("Details", "Preguntas", new { id = model.PreguntaID });
            }

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

            string imagenUrl = respuestaActual.ImagenUrl;

            if (imagen != null && imagen.ContentLength > 0)
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

            string mensajeError;

            if (!_respuestaBusiness.Actualizar(respuesta, out mensajeError))
            {
                ModelState.AddModelError("", mensajeError);

                model.ImagenUrl = respuestaActual.ImagenUrl;
                model.PreguntaID = respuestaActual.PreguntaID;

                return View(model);
            }

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
