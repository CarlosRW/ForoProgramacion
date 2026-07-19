using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TechForo.Core.Business;
using TechForo.Data.Entidades;
using TechForo.Data.Repositorios;
using TechForo.Models.Vista_de_modelos;

namespace TechForo.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly PreguntaBusiness _preguntaBusiness;

        public HomeController()
        {
            _preguntaBusiness = new PreguntaBusiness(new PreguntaRepository());
        }

        // Landing conectada a datos reales de Preguntas. La búsqueda y el
        // resumen se coordinan por medio de PreguntaBusiness.
        public ActionResult Index(string buscar)
        {
            List<Pregunta> preguntas = _preguntaBusiness.Buscar(buscar);
            List<PreguntaResumenViewModel> modelo = preguntas
                .Select(MapearResumen)
                .ToList();

            ViewBag.Buscar = buscar;

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

        // SOLID - SRP: el mapeo de la entidad al ViewModel se mantiene separado
        // de la acción HTTP y se reutiliza para cada tarjeta de la lista.
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
                TotalRespuestas = pregunta.TotalRespuestas,
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
