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
        public string Etiquetas { get; set; }
        public int TotalVistas { get; set; }
        public int TotalRespuestas { get; set; }
        public bool Resuelta { get; set; }
    }
}
