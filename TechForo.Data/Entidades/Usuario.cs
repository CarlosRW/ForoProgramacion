using System;

namespace TechForo.Data.Entidades
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }

        // Campos del Perfil (antes estaban solo en Session)
        public string Titular { get; set; }
        public string Biografia { get; set; }
        public string Ubicacion { get; set; }
        public string AvatarUrl { get; set; }
    }
}