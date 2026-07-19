using System;
using System.Collections.Generic;
using TechForo.Data.Entidades;
using TechForo.Data.Repositorios;

namespace TechForo.Core.Business
{
    // DP - Service Layer: ofrece un unico punto de entrada para los casos de
    // uso de Preguntas y evita que el Controller conozca reglas o consultas SQL.
    // SOLID - SRP: esta clase se ocupa unicamente de las reglas de negocio y de
    // coordinar la persistencia de Preguntas.
    public class PreguntaBusiness
    {
        private readonly IPreguntaRepository _preguntaRepository;

        // SOLID - DIP: PreguntaBusiness depende de IPreguntaRepository y no de
        // una implementacion concreta. Esto tambien permite inyectar un repositorio
        // falso en pruebas sin cambiar la logica de negocio.
        // SOLID - OCP: se puede agregar otra implementacion del repositorio sin
        // modificar esta clase.
        public PreguntaBusiness(IPreguntaRepository preguntaRepository)
        {
            if (preguntaRepository == null)
                throw new ArgumentNullException(nameof(preguntaRepository));

            _preguntaRepository = preguntaRepository;
        }

        public List<Pregunta> ObtenerTodas()
        {
            return _preguntaRepository.ObtenerTodas();
        }

        public List<Pregunta> Buscar(string termino)
        {
            List<Pregunta> preguntas = _preguntaRepository.ObtenerTodas();

            if (string.IsNullOrWhiteSpace(termino))
                return preguntas;

            string textoBuscado = termino.Trim();

            return preguntas.FindAll(pregunta =>
                Contiene(pregunta.Titulo, textoBuscado) ||
                Contiene(pregunta.Descripcion, textoBuscado) ||
                Contiene(pregunta.UsuarioNombre, textoBuscado) ||
                Contiene(pregunta.Etiquetas, textoBuscado));
        }

        public Pregunta ObtenerPorId(int id)
        {
            if (id <= 0)
                return null;

            return _preguntaRepository.ObtenerPorId(id);
        }

        public Pregunta ObtenerDetalle(int id)
        {
            if (id <= 0)
                return null;

            Pregunta pregunta = _preguntaRepository.ObtenerPorId(id);

            if (pregunta == null)
                return null;

            if (_preguntaRepository.IncrementarVistas(id))
                pregunta.TotalVistas++;

            return pregunta;
        }

        public bool Crear(Pregunta pregunta, out string mensajeError)
        {
            if (!PrepararYValidar(pregunta, false, out mensajeError))
                return false;

            // Una pregunta nueva no puede iniciar marcada como resuelta.
            pregunta.Resuelta = false;
            pregunta.PreguntaID = _preguntaRepository.Crear(pregunta);

            if (pregunta.PreguntaID <= 0)
            {
                mensajeError = "No fue posible crear la pregunta.";
                return false;
            }

            return true;
        }

        public bool Actualizar(Pregunta pregunta, out string mensajeError)
        {
            if (!PrepararYValidar(pregunta, true, out mensajeError))
                return false;

            Pregunta preguntaActual = _preguntaRepository.ObtenerPorId(pregunta.PreguntaID);

            if (preguntaActual == null)
            {
                mensajeError = "La pregunta indicada no existe.";
                return false;
            }

            if (preguntaActual.UsuarioID != pregunta.UsuarioID)
            {
                mensajeError = "No tiene permiso para editar esta pregunta.";
                return false;
            }

            if (pregunta.Resuelta && preguntaActual.TotalRespuestas == 0)
            {
                mensajeError = "No puede marcar la pregunta como resuelta porque todavía no tiene respuestas.";
                return false;
            }

            if (!_preguntaRepository.Actualizar(pregunta))
            {
                mensajeError = "No fue posible actualizar la pregunta.";
                return false;
            }

            return true;
        }

        public bool Eliminar(int preguntaID, int usuarioID, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (preguntaID <= 0 || usuarioID <= 0)
            {
                mensajeError = "Los datos de la pregunta no son válidos.";
                return false;
            }

            Pregunta preguntaActual = _preguntaRepository.ObtenerPorId(preguntaID);

            if (preguntaActual == null)
            {
                mensajeError = "La pregunta indicada no existe.";
                return false;
            }

            if (preguntaActual.UsuarioID != usuarioID)
            {
                mensajeError = "No tiene permiso para eliminar esta pregunta.";
                return false;
            }

            if (!_preguntaRepository.Eliminar(preguntaID, usuarioID))
            {
                mensajeError = "No fue posible eliminar la pregunta.";
                return false;
            }

            return true;
        }

        private bool PrepararYValidar(Pregunta pregunta, bool requiereId, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (pregunta == null)
            {
                mensajeError = "Debe indicar los datos de la pregunta.";
                return false;
            }

            if (requiereId && pregunta.PreguntaID <= 0)
            {
                mensajeError = "La pregunta indicada no es válida.";
                return false;
            }

            if (pregunta.UsuarioID <= 0)
            {
                mensajeError = "No se pudo identificar al usuario de la pregunta.";
                return false;
            }

            pregunta.Titulo = pregunta.Titulo == null ? "" : pregunta.Titulo.Trim();
            pregunta.Descripcion = pregunta.Descripcion == null ? "" : pregunta.Descripcion.Trim();
            pregunta.Etiquetas = NormalizarEtiquetas(pregunta.Etiquetas);
            pregunta.ImagenUrl = string.IsNullOrWhiteSpace(pregunta.ImagenUrl)
                ? null
                : pregunta.ImagenUrl.Trim();

            if (pregunta.Titulo.Length == 0)
            {
                mensajeError = "El título es obligatorio.";
                return false;
            }

            if (pregunta.Titulo.Length > 200)
            {
                mensajeError = "El título debe tener máximo 200 caracteres.";
                return false;
            }

            if (pregunta.Descripcion.Length == 0)
            {
                mensajeError = "La descripción es obligatoria.";
                return false;
            }

            if (pregunta.Etiquetas != null && pregunta.Etiquetas.Length > 300)
            {
                mensajeError = "Las etiquetas deben tener máximo 300 caracteres.";
                return false;
            }

            if (pregunta.ImagenUrl != null && pregunta.ImagenUrl.Length > 300)
            {
                mensajeError = "La ruta de la imagen no es válida.";
                return false;
            }

            return true;
        }

        private string NormalizarEtiquetas(string etiquetas)
        {
            if (string.IsNullOrWhiteSpace(etiquetas))
                return null;

            List<string> resultado = new List<string>();
            HashSet<string> existentes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string etiqueta in etiquetas.Split(','))
            {
                string etiquetaLimpia = etiqueta.Trim();

                if (etiquetaLimpia.Length > 0 && existentes.Add(etiquetaLimpia))
                    resultado.Add(etiquetaLimpia);
            }

            return resultado.Count == 0 ? null : string.Join(", ", resultado);
        }

        private bool Contiene(string valor, string textoBuscado)
        {
            return !string.IsNullOrWhiteSpace(valor) &&
                   valor.IndexOf(textoBuscado, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
