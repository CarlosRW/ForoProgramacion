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
    }
}