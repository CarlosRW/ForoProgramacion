using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using TechForo.Core.Business;
using TechForo.Data.Entidades;
using TechForo.Data.Repositorios;
using TechForo.Models.Vista_de_modelos;

namespace TechForo.MVC.Controllers
{
    // DP - MVC: este Controller recibe la petición HTTP y prepara el ViewModel;
    // las reglas pertenecen a PreguntaBusiness y el SQL a PreguntaRepository.
    // SOLID - SRP: aquí solo se coordina la navegación, ModelState, sesión y
    // archivos enviados por el navegador.
    public class PreguntasController : Controller
    {
        private const int MaximoImagenBytes = 5 * 1024 * 1024;

        private static readonly HashSet<string> ExtensionesPermitidas =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg", ".jpeg", ".png", ".gif", ".webp"
            };

        private readonly PreguntaBusiness _preguntaBusiness;
        private readonly RespuestaBusiness _respuestaBusiness;

        public PreguntasController()
        {
            _preguntaBusiness = new PreguntaBusiness(new PreguntaRepository());
            _respuestaBusiness = new RespuestaBusiness();
        }

        public ActionResult Index(string buscar)
        {
            ViewBag.Buscar = buscar;
            return View(_preguntaBusiness.Buscar(buscar));
        }

        public ActionResult Details(int? id)
        {
            if (id == null || id.Value <= 0)
                return RedirectToAction("Index");

            Pregunta pregunta = _preguntaBusiness.ObtenerDetalle(id.Value);

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
                Etiquetas = pregunta.Etiquetas,
                TotalVistas = pregunta.TotalVistas,
                Resuelta = pregunta.Resuelta,
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

            string imagenUrl;
            string mensajeError;

            if (!IntentarGuardarImagen(imagen, out imagenUrl, out mensajeError))
            {
                ModelState.AddModelError("", mensajeError);
                return View(model);
            }

            Pregunta pregunta = new Pregunta
            {
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                Codigo = model.Codigo,
                ImagenUrl = imagenUrl,
                Etiquetas = model.Etiquetas,
                Resuelta = false,
                UsuarioID = Convert.ToInt32(Session["UsuarioID"])
            };

            if (!_preguntaBusiness.Crear(pregunta, out mensajeError))
            {
                EliminarImagenLocal(imagenUrl);
                ModelState.AddModelError("", mensajeError);
                return View(model);
            }

            TempData["MensajeExito"] = "Pregunta publicada correctamente.";
            return RedirectToAction("Details", new { id = pregunta.PreguntaID });
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            Pregunta pregunta = _preguntaBusiness.ObtenerPorId(id);

            if (pregunta == null)
                return HttpNotFound();

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            if (pregunta.UsuarioID != usuarioID)
            {
                TempData["MensajeError"] = "No tiene permiso para editar esta pregunta.";
                return RedirectToAction("Index");
            }

            PreguntaModel model = new PreguntaModel
            {
                PreguntaID = pregunta.PreguntaID,
                Titulo = pregunta.Titulo,
                Descripcion = pregunta.Descripcion,
                Codigo = pregunta.Codigo,
                ImagenUrl = pregunta.ImagenUrl,
                Etiquetas = pregunta.Etiquetas,
                Resuelta = pregunta.Resuelta
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

            Pregunta preguntaActual = _preguntaBusiness.ObtenerPorId(model.PreguntaID);

            if (preguntaActual == null)
                return HttpNotFound();

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            if (preguntaActual.UsuarioID != usuarioID)
            {
                TempData["MensajeError"] = "No tiene permiso para editar esta pregunta.";
                return RedirectToAction("Index");
            }

            model.ImagenUrl = preguntaActual.ImagenUrl;

            if (!ModelState.IsValid)
                return View(model);

            string imagenUrlNueva;
            string mensajeError;

            if (!IntentarGuardarImagen(imagen, out imagenUrlNueva, out mensajeError))
            {
                ModelState.AddModelError("", mensajeError);
                return View(model);
            }

            string imagenUrlFinal = string.IsNullOrEmpty(imagenUrlNueva)
                ? preguntaActual.ImagenUrl
                : imagenUrlNueva;

            Pregunta pregunta = new Pregunta
            {
                PreguntaID = model.PreguntaID,
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                Codigo = model.Codigo,
                ImagenUrl = imagenUrlFinal,
                Etiquetas = model.Etiquetas,
                Resuelta = model.Resuelta,
                UsuarioID = usuarioID
            };

            if (!_preguntaBusiness.Actualizar(pregunta, out mensajeError))
            {
                EliminarImagenLocal(imagenUrlNueva);
                ModelState.AddModelError("", mensajeError);
                return View(model);
            }

            if (!string.IsNullOrEmpty(imagenUrlNueva))
                EliminarImagenLocal(preguntaActual.ImagenUrl);

            TempData["MensajeExito"] = "Pregunta actualizada correctamente.";
            return RedirectToAction("Details", new { id = model.PreguntaID });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Account");

            Pregunta pregunta = _preguntaBusiness.ObtenerPorId(id);

            if (pregunta == null)
                return HttpNotFound();

            string mensajeError;
            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            if (!_preguntaBusiness.Eliminar(id, usuarioID, out mensajeError))
            {
                TempData["MensajeError"] = mensajeError;
                return RedirectToAction("Index");
            }

            EliminarImagenLocal(pregunta.ImagenUrl);
            TempData["MensajeExito"] = "Pregunta eliminada correctamente.";
            return RedirectToAction("Index");
        }

        private bool IntentarGuardarImagen(
            HttpPostedFileBase imagen,
            out string imagenUrl,
            out string mensajeError)
        {
            imagenUrl = null;
            mensajeError = string.Empty;

            if (imagen == null || imagen.ContentLength == 0)
                return true;

            if (imagen.ContentLength > MaximoImagenBytes)
            {
                mensajeError = "La imagen no puede superar los 5 MB.";
                return false;
            }

            string extension = Path.GetExtension(imagen.FileName);

            if (string.IsNullOrEmpty(extension) || !ExtensionesPermitidas.Contains(extension))
            {
                mensajeError = "Solo se permiten imágenes JPG, JPEG, PNG, GIF o WEBP.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(imagen.ContentType) ||
                !imagen.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                mensajeError = "El archivo seleccionado no es una imagen válida.";
                return false;
            }

            string carpeta = Server.MapPath("~/Uploads/Preguntas/");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreArchivo = Guid.NewGuid().ToString("N") + extension.ToLowerInvariant();
            string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            imagen.SaveAs(rutaCompleta);
            imagenUrl = "/Uploads/Preguntas/" + nombreArchivo;
            return true;
        }

        private void EliminarImagenLocal(string imagenUrl)
        {
            if (string.IsNullOrWhiteSpace(imagenUrl) ||
                !imagenUrl.StartsWith("/Uploads/Preguntas/", StringComparison.OrdinalIgnoreCase))
                return;

            string rutaFisica = Server.MapPath("~" + imagenUrl);

            if (System.IO.File.Exists(rutaFisica))
                System.IO.File.Delete(rutaFisica);
        }
    }
}
