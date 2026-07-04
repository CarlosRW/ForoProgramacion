using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace TechForo.Data.Entidades
{
    public class Pregunta
    {
        public int PreguntaID { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public string ImagenUrl { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int UsuarioID { get; set; }
        public string UsuarioNombre { get; set; }
    }
}