using System;
using System.ComponentModel.DataAnnotations;

namespace TechForo.Models.Vista_de_modelos
{
    // SOLID: Single Responsibility - agrupa unicamente los datos editables del
    // perfil del usuario. No reutiliza la entidad Usuario de TechForo.Data porque
    // esta clase pertenece a la capa de presentacion (Avance 2: datos simulados
    // via Session, a futuro se conecta con UsuarioBusiness/UsuarioRepository).
    public class PerfilUsuarioViewModel
    {
        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [StringLength(120, ErrorMessage = "El titular debe tener máximo 120 caracteres")]
        [Display(Name = "Titular")]
        public string Titular { get; set; }

        [StringLength(280, ErrorMessage = "La biografía debe tener máximo 280 caracteres")]
        [Display(Name = "Biografía")]
        public string Biografia { get; set; }

        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }

        [Display(Name = "Foto de perfil (URL)")]
        public string AvatarUrl { get; set; }

        public DateTime MiembroDesde { get; set; }
        public int TotalPreguntas { get; set; }
        public int TotalRespuestas { get; set; }

        public string Iniciales
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Nombre))
                    return "?";

                string[] partes = Nombre.Trim().Split(' ');

                return partes.Length > 1
                    ? $"{partes[0][0]}{partes[1][0]}".ToUpper()
                    : partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
            }
        }
    }
}