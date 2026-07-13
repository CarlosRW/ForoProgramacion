using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TechForo.Core.Business;
using TechForo.Data.Entidades;
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

        // Avance 3: pantalla de mantenimiento de usuario (perfil editable).
        // Ya no usa Session como "base de datos" - lee y guarda contra la
        // tabla Usuarios a traves de UsuarioBusiness/UsuarioRepository.
        // SOLID: Single Responsibility - el controlador solo orquesta la peticion
        // HTTP (arma el ViewModel, revisa ModelState, guarda el archivo), toda
        // la logica de negocio (validar, guardar) vive en UsuarioBusiness.
        [Authorize]
        [HttpGet]
        public ActionResult Perfil()
        {
            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);
            Usuario usuario = _usuarioBusiness.ObtenerPerfil(usuarioID);

            if (usuario == null)
                return HttpNotFound();

            PerfilUsuarioViewModel model = new PerfilUsuarioViewModel
            {
                UsuarioID = usuario.UsuarioID,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Titular = usuario.Titular,
                Biografia = usuario.Biografia,
                Ubicacion = usuario.Ubicacion,
                AvatarUrl = usuario.AvatarUrl
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Perfil(PerfilUsuarioViewModel model, HttpPostedFileBase imagenPerfil)
        {
            if (!ModelState.IsValid)
                return View(model);

            int usuarioID = Convert.ToInt32(Session["UsuarioID"]);

            // No confiamos en el AvatarUrl que viene del formulario para decidir
            // que guardar: se lee el valor actual desde la BD y solo se
            // reemplaza si el usuario subio una imagen nueva de verdad.
            Usuario usuarioActual = _usuarioBusiness.ObtenerPerfil(usuarioID);

            if (usuarioActual == null)
                return HttpNotFound();

            string avatarUrl = usuarioActual.AvatarUrl;

            if (imagenPerfil != null && imagenPerfil.ContentLength > 0)
                avatarUrl = GuardarImagen(imagenPerfil);

            Usuario usuario = new Usuario
            {
                UsuarioID = usuarioID,
                Nombre = model.Nombre,
                Titular = model.Titular,
                Biografia = model.Biografia,
                Ubicacion = model.Ubicacion,
                AvatarUrl = avatarUrl
            };

            string mensajeError;
            bool exito = _usuarioBusiness.ActualizarPerfil(usuario, out mensajeError);

            if (!exito)
            {
                ModelState.AddModelError("", mensajeError);
                model.AvatarUrl = avatarUrl;
                return View(model);
            }

            // El nombre tambien se usa en el _Layout ("Hola, @Session[...]"),
            // se actualiza aca para que se refleje sin tener que volver a loguear.
            Session["UsuarioNombre"] = model.Nombre;

            TempData["MensajeExito"] = "Perfil actualizado con éxito.";
            return RedirectToAction("Perfil");
        }

        // DP: mismo Factory Method simple que ya usan PreguntasController y
        // RespuestasController para guardar imagenes subidas por el usuario.
        private string GuardarImagen(HttpPostedFileBase imagen)
        {
            string carpeta = Server.MapPath("~/Uploads/Perfiles/");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            imagen.SaveAs(rutaCompleta);

            return "/Uploads/Perfiles/" + nombreArchivo;
        }
    }
}