using System.ComponentModel.DataAnnotations;

namespace TechForo.Models.Vista_de_modelos
{
    public class ConfiguracionModel
    {
        [Display(Name = "Modo oscuro")]
        public bool ModoOscuro { get; set; }

        [Display(Name = "Tamaño de fuente")]
        public string TamanoFuente { get; set; } = "Mediano";

        [Display(Name = "Idioma")]
        public string Idioma { get; set; } = "Español";

        [Display(Name = "Mostrar imágenes")]
        public bool MostrarImagenes { get; set; } = true;

        [Display(Name = "Mostrar bloques de código")]
        public bool MostrarCodigo { get; set; } = true;

        [Display(Name = "Vista compacta")]
        public bool VistaCompacta { get; set; } = false;
    }
}