using System.Security.Cryptography;
using System.Text;
using TechForo.Data.Entidades;
using TechForo.Data.Repositorios;

namespace TechForo.Core.Business
{
    public class UsuarioBusiness
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioBusiness()
        {
            _usuarioRepository = new UsuarioRepository();
        }

        // SOLID: Single Responsibility - esta clase solo maneja la lógica de negocio de Usuario,
        // transfiere el acceso a datos al Repository
        public Usuario ValidarLogin(string correo, string password)
        {
            Usuario usuario = _usuarioRepository.ObtenerPorCorreo(correo);

            if (usuario == null)
                return null;

            if (usuario.Password != EncriptarPassword(password))
                return null;

            return usuario;
        }

        public bool RegistrarUsuario(string nombre, string correo, string password, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (_usuarioRepository.ExisteCorreo(correo))
            {
                mensajeError = "Ya existe un usuario registrado con ese correo.";
                return false;
            }

            Usuario usuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Password = EncriptarPassword(password)
            };

            _usuarioRepository.Crear(usuario);
            return true;
        }

        private string EncriptarPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }
    }
}