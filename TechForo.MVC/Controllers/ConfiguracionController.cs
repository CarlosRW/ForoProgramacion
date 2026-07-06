using System.Web.Mvc;
using TechForo.Models.Vista_de_modelos;

namespace TechForo.MVC.Controllers
{
    // SOLID - S: Administra solo la configuración 
    public class ConfiguracionController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ConfiguracionModel model = new ConfiguracionModel
            {
                ModoOscuro = true,
                MostrarImagenes = true,
                MostrarCodigo = true,
                Notificaciones = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ConfiguracionModel model)
        {
            // Simulación del guardado para el avance del proyecto.
            TempData["Mensaje"] = "Configuración guardada correctamente.";

            return View(model);
        }
    }
}