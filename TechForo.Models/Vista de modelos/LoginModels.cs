using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TechForo.Models.Vista_de_modelos
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El password es obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class RegistroModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El password es obligatorio")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "El password debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe confirmar el password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Password")]
        [Compare("Password", ErrorMessage = "Los passwords no coinciden")]
        public string ConfirmPassword { get; set; }
    }
}