using System;
using System.Web.Mvc;
using System.Web.Security;
using TechForo.Core.Business;
using TechForo.Models.Vista_de_modelos;

namespace TechForo.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioBusiness _usuarioBusiness;

        public AccountController()
        {
            _usuarioBusiness = new UsuarioBusiness();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = _usuarioBusiness.ValidarLogin(model.Correo, model.Password);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Correo o password incorrectos.");
                return View(model);
            }

            FormsAuthentication.SetAuthCookie(usuario.Correo, false);
            Session["UsuarioID"] = usuario.UsuarioID;
            Session["UsuarioNombre"] = usuario.Nombre;

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegistroModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegistroModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string mensajeError;
            bool exito = _usuarioBusiness.RegistrarUsuario(model.Nombre, model.Correo, model.Password, out mensajeError);

            if (!exito)
            {
                ModelState.AddModelError("", mensajeError);
                return View(model);
            }

            TempData["MensajeExito"] = "Cuenta creada con éxito. Ya podés iniciar sesión.";
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Avance 2: pantalla de mantenimiento de usuario (perfil editable).
        // SOLID: Single Responsibility - el controlador solo orquesta la peticion
        // HTTP, no calcula nada de negocio; por ahora lee/escribe en Session porque
        // todavia no existe ObtenerPorId/ActualizarPerfil en UsuarioRepository.
        [Authorize]
        [HttpGet]
        public ActionResult Perfil()
        {
            PerfilUsuarioViewModel model = ObtenerPerfilDeSesion();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Perfil(PerfilUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // DP: Strategy (simplificado) - mientras no haya persistencia real,
            // "guardar" el perfil es simplemente actualizar los datos de Session
            // que ya usa el resto del sitio (ver _Layout, "Hola, @Session[...]").
            Session["UsuarioNombre"] = model.Nombre;
            Session["UsuarioAvatarUrl"] = model.AvatarUrl;
            Session["UsuarioTitular"] = model.Titular;
            Session["UsuarioBiografia"] = model.Biografia;
            Session["UsuarioUbicacion"] = model.Ubicacion;

            TempData["MensajeExito"] = "Perfil actualizado con éxito.";
            return RedirectToAction("Perfil");
        }

        private PerfilUsuarioViewModel ObtenerPerfilDeSesion()
        {
            return new PerfilUsuarioViewModel
            {
                UsuarioID = Session["UsuarioID"] != null ? (int)Session["UsuarioID"] : 0,
                Nombre = Session["UsuarioNombre"] as string ?? "Usuario DevSpace",
                Correo = User.Identity.Name,
                Titular = Session["UsuarioTitular"] as string ?? "Estudiante de Programación Avanzada",
                Biografia = Session["UsuarioBiografia"] as string ?? "",
                Ubicacion = Session["UsuarioUbicacion"] as string ?? "San José, Costa Rica",
                AvatarUrl = Session["UsuarioAvatarUrl"] as string ?? "",
                MiembroDesde = DateTime.Now.AddMonths(-2),
                TotalPreguntas = 3,
                TotalRespuestas = 8
            };
        }
    }
}