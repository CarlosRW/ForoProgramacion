using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace TechForo.Data.Entidades
{
    public class Respuesta
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