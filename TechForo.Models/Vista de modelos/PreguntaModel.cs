using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TechForo.Models.Vista_de_modelos
{
    public class PreguntaModel
    {
        public int PreguntaID { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título debe tener máximo 200 caracteres")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Bloque de código")]
        public string Codigo { get; set; }

        public string ImagenUrl { get; set; }

        [StringLength(300, ErrorMessage = "Las etiquetas deben tener máximo 300 caracteres")]
        [Display(Name = "Etiquetas")]
        public string Etiquetas { get; set; }

        [Display(Name = "Pregunta resuelta")]
        public bool Resuelta { get; set; }
    }
}
