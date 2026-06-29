using System;
using System.Collections.Generic;

namespace TechForo.Models.Vista_de_modelos
{
    // SOLID: Single Responsibility - este ViewModel solo transporta los datos
    // necesarios para pintar una tarjeta de pregunta en el listado de la landing
    // page. No conoce reglas de negocio ni acceso a datos.
    public class PreguntaResumenViewModel
    {
        public int PreguntaID { get; set; }
        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public string AutorNombre { get; set; }
        public string AutorIniciales { get; set; }
        public List<string> Etiquetas { get; set; }
        public int TotalRespuestas { get; set; }
        public int TotalVistas { get; set; }
        public bool Resuelta { get; set; }
        public DateTime FechaCreacion { get; set; }

        public PreguntaResumenViewModel()
        {
            Etiquetas = new List<string>();
        }

        // "Hace X tiempo" para no ensuciar la vista con calculos de fechas.
        public string TiempoTranscurrido
        {
            get
            {
                TimeSpan transcurrido = DateTime.Now - FechaCreacion;

                if (transcurrido.TotalMinutes < 1)
                    return "justo ahora";
                if (transcurrido.TotalMinutes < 60)
                    return $"hace {(int)transcurrido.TotalMinutes} min";
                if (transcurrido.TotalHours < 24)
                    return $"hace {(int)transcurrido.TotalHours} h";

                return $"hace {(int)transcurrido.TotalDays} d";
            }
        }
    }
}