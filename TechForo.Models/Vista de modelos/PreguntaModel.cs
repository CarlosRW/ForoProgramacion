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

        [Required]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Bloque de código")]
        public string Codigo { get; set; }

        public string ImagenUrl { get; set; }
    }
}