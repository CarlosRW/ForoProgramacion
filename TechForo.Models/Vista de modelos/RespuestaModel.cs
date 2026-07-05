using System;

namespace TechForo.Models.Vista_de_modelos
{
    public class RespuestaModel
    {
        public int RespuestaID { get; set; }
        public string Contenido { get; set; }
        public string Codigo { get; set; }
        public string ImagenUrl { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int UsuarioID { get; set; }
        public int PreguntaID { get; set; }
        public string UsuarioNombre { get; set; }
    }
}