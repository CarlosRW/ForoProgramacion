using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechForo.Models.Vista_de_modelos;

namespace TechForo.MVC.Controllers
{
    public class HomeController : Controller
    {
        // Avance 2: el listado todavia no consulta TechForo.Data / TechForo.Core,
        // se devuelven datos simulados para poder maquetar y probar la landing page
        // (segun lo acordado: "controladores que al menos devuelvan datos simulados").
        // Cuando el modulo de Preguntas este listo, reemplazar por algo como:
        // PreguntaBusiness.ObtenerUltimas() desde TechForo.Core.
        public ActionResult Index()
        {
            List<PreguntaResumenViewModel> preguntas = ObtenerPreguntasSimuladas();
            return View(preguntas);
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

        // DP: Factory Method - centraliza la construccion de los datos de ejemplo
        // en un solo metodo privado, facil de eliminar cuando se conecte la BD real.
        private List<PreguntaResumenViewModel> ObtenerPreguntasSimuladas()
        {
            return new List<PreguntaResumenViewModel>
            {
                new PreguntaResumenViewModel
                {
                    PreguntaID = 1,
                    Titulo = "¿Cómo evitar el problema N+1 al usar Entity Framework?",
                    Resumen = "Tengo un listado que carga relaciones hijas y el log muestra cientos de queries por cada request...",
                    AutorNombre = "Carlos M.",
                    AutorIniciales = "CM",
                    Etiquetas = new List<string> { "C#", "Entity Framework", "SQL" },
                    TotalRespuestas = 4,
                    TotalVistas = 132,
                    Resuelta = true,
                    FechaCreacion = DateTime.Now.AddHours(-3)
                },
                new PreguntaResumenViewModel
                {
                    PreguntaID = 2,
                    Titulo = "Diferencia real entre Task.Run y async/await en ASP.NET MVC clásico",
                    Resumen = "No me queda claro cuándo conviene usar Task.Run dentro de un controlador síncrono...",
                    AutorNombre = "Daniela R.",
                    AutorIniciales = "DR",
                    Etiquetas = new List<string> { "ASP.NET", "Async" },
                    TotalRespuestas = 2,
                    TotalVistas = 58,
                    Resuelta = false,
                    FechaCreacion = DateTime.Now.AddHours(-7)
                },
                new PreguntaResumenViewModel
                {
                    PreguntaID = 3,
                    Titulo = "Aplicar Repository Pattern sin acoplar la capa Business a SqlClient",
                    Resumen = "Estoy armando la capa Data del proyecto final y quiero respetar SOLID, ¿cómo evito el acoplamiento?",
                    AutorNombre = "Andrés P.",
                    AutorIniciales = "AP",
                    Etiquetas = new List<string> { "SOLID", "Design Patterns", "ADO.NET" },
                    TotalRespuestas = 6,
                    TotalVistas = 210,
                    Resuelta = true,
                    FechaCreacion = DateTime.Now.AddDays(-1)
                },
                new PreguntaResumenViewModel
                {
                    PreguntaID = 4,
                    Titulo = "Bootstrap 5: el modal no cierra al hacer submit con AJAX",
                    Resumen = "El modal para crear una respuesta se queda abierto después del POST exitoso...",
                    AutorNombre = "Carlos M.",
                    AutorIniciales = "CM",
                    Etiquetas = new List<string> { "Bootstrap", "JavaScript" },
                    TotalRespuestas = 0,
                    TotalVistas = 12,
                    Resuelta = false,
                    FechaCreacion = DateTime.Now.AddMinutes(-40)
                }
            };
        }
    }
}