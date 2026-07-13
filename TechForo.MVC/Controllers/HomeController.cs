using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TechForo.Core.Business;
using TechForo.Data.Entidades;
using TechForo.Models.Vista_de_modelos;

namespace TechForo.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly PreguntaBusiness _preguntaBusiness;
        private readonly RespuestaBusiness _respuestaBusiness;

        public HomeController()
        {
            _preguntaBusiness = new PreguntaBusiness();
            _respuestaBusiness = new RespuestaBusiness();
        }

        // Avance 3: landing page conectada a datos reales. Etiquetas/TotalVistas/
        // Resuelta ya existen como columnas en Preguntas, pero todavia nadie las
        // escribe (eso lo agrega Isaac en el formulario de Crear/Editar pregunta),
        // asi que por ahora van a aparecer vacias/en 0/false para preguntas creadas
        public ActionResult Index()
        {
            List<Pregunta> preguntas = _preguntaBusiness.ObtenerTodas();
            List<PreguntaResumenViewModel> modelo = preguntas
                .Select(MapearResumen)
                .ToList();

            return View(modelo);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        // DP: Factory Method - centraliza como se arma una tarjeta de la landing
        // a partir de una Pregunta real, en vez de repetir este mapeo en Index.
        private PreguntaResumenViewModel MapearResumen(Pregunta pregunta)
        {
            return new PreguntaResumenViewModel
            {
                PreguntaID = pregunta.PreguntaID,
                Titulo = pregunta.Titulo,
                Resumen = RecortarTexto(pregunta.Descripcion, 140),
                AutorNombre = pregunta.UsuarioNombre,
                AutorIniciales = ObtenerIniciales(pregunta.UsuarioNombre),
                Etiquetas = string.IsNullOrWhiteSpace(pregunta.Etiquetas)
                    ? new List<string>()
                    : pregunta.Etiquetas.Split(',').Select(e => e.Trim()).ToList(),
                TotalRespuestas = _respuestaBusiness.ObtenerPorPregunta(pregunta.PreguntaID).Count,
                TotalVistas = pregunta.TotalVistas,
                Resuelta = pregunta.Resuelta,
                FechaCreacion = pregunta.FechaCreacion
            };
        }

        private string RecortarTexto(string texto, int longitudMaxima)
        {
            if (string.IsNullOrEmpty(texto) || texto.Length <= longitudMaxima)
                return texto;

            return texto.Substring(0, longitudMaxima).TrimEnd() + "...";
        }

        private string ObtenerIniciales(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                return "??";

            var partes = nombreCompleto.Trim().Split(' ');

            if (partes.Length == 1)
                return partes[0].Substring(0, 1).ToUpper();

            return (partes[0].Substring(0, 1) + partes[1].Substring(0, 1)).ToUpper();
        }
    }
}