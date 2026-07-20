using TechForo.Models.Vista_de_modelos;

namespace TechForo.Core.Business
{
    // DP - Service Layer: concentra la lógica de business de configuración sin que el Controller tenga que conocer validaciones.
    // SOLID - Single responsability:  únicamente prepara y valida la configuración del usuario.
    public class ConfiguracionBusiness
    {
        public ConfiguracionModel ObtenerConfiguracion()
        {
            return new ConfiguracionModel
            {
                ModoOscuro = true,
                MostrarImagenes = true,
                MostrarCodigo = true,
                Notificaciones = true
            };
        }

        public bool Validar(ConfiguracionModel model)
        {
            return model != null;
        }
    }
}
