using System.Web.Mvc;
using TechForo.Core.Business;
using TechForo.Models.Vista_de_modelos;

namespace TechForo.MVC.Controllers
{
    // SOLID - single responsability: Controller solo recibe peticiones HTTP y delega la lógica a ConfiguracionBusiness.
    [Authorize]
    public class ConfiguracionController : Controller
    {
        private readonly ConfiguracionBusiness _business;

        public ConfiguracionController()
        {
            _business = new ConfiguracionBusiness();
        }

        [HttpGet]
        public ActionResult Index()
        {
            ConfiguracionModel model;

            if (Session["Configuracion"] == null)
            {
                model = _business.ObtenerConfiguracion();
            }
            else
            {
                model = (ConfiguracionModel)Session["Configuracion"];
            }

            if (Session["ModoOscuro"] != null)
            {
                model.ModoOscuro = (bool)Session["ModoOscuro"];
                model.MostrarImagenes = (bool)Session["MostrarImagenes"];
                model.MostrarCodigo = (bool)Session["MostrarCodigo"];
                model.Notificaciones = (bool)Session["Notificaciones"];
                if (Session["TamanoFuente"] != null)
                    model.TamanoFuente = Session["TamanoFuente"].ToString();

                if (Session["Idioma"] != null)
                    model.Idioma = Session["Idioma"].ToString();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ConfiguracionModel model)
        {
            if (!_business.Validar(model))
                return View(model);

            // Se guarda la configuración del usuario en Session para este avance
            Session["Configuracion"] = model;

            Session["ModoOscuro"] = model.ModoOscuro;
            Session["MostrarImagenes"] = model.MostrarImagenes;
            Session["MostrarCodigo"] = model.MostrarCodigo;
            Session["Notificaciones"] = model.Notificaciones;
            Session["TamanoFuente"] = model.TamanoFuente;
            Session["Idioma"] = model.Idioma;

            TempData["Mensaje"] = "Configuración guardada correctamente.";

            return RedirectToAction("Index");

        }
    }
}