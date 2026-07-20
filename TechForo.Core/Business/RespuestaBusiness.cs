using System;
using System.Collections.Generic;
using TechForo.Data.Entidades;
using TechForo.Data.Repositorios;

namespace TechForo.Core.Business
{
    // DP - Service Layer: ofrece un unico punto de entrada para los casos de
    // uso de Respuestas y evita que el Controller contenga reglas de negocio.
    // SOLID - SRP: esta clase se ocupa de validar respuestas y coordinar su
    // persistencia; el acceso a SQL permanece en el Repository.
    public class RespuestaBusiness
    {
        private readonly IRespuestaRepository _respuestaRepository;

        // SOLID - DIP: RespuestaBusiness depende del contrato
        // IRespuestaRepository y no de una implementacion concreta.
        // SOLID - OCP: se puede usar otro repositorio sin modificar esta clase.
        public RespuestaBusiness(IRespuestaRepository respuestaRepository)
        {
            if (respuestaRepository == null)
                throw new ArgumentNullException(nameof(respuestaRepository));

            _respuestaRepository = respuestaRepository;
        }

        public List<Respuesta> ObtenerPorPregunta(int preguntaID)
        {
            return _respuestaRepository.ObtenerPorPregunta(preguntaID);
        }

        public Respuesta ObtenerPorId(int respuestaID)
        {
            return _respuestaRepository.ObtenerPorId(respuestaID);
        }

        public bool Crear(Respuesta respuesta, out string mensajeError)
        {
            if (!ValidarContenido(respuesta, out mensajeError))
                return false;

            _respuestaRepository.Crear(respuesta);
            return true;
        }

        public bool Actualizar(Respuesta respuesta, out string mensajeError)
        {
            if (!ValidarContenido(respuesta, out mensajeError))
                return false;

            _respuestaRepository.Actualizar(respuesta);
            return true;
        }

        public void Eliminar(int respuestaID, int usuarioID)
        {
            _respuestaRepository.Eliminar(respuestaID, usuarioID);
        }

        private bool ValidarContenido(Respuesta respuesta, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (respuesta == null)
            {
                mensajeError = "Debe indicar los datos de la respuesta.";
                return false;
            }

            // Regla de negocio: una respuesta debe aportar al menos uno de
            // estos elementos, sin importar desde que Controller se registre.
            if (string.IsNullOrWhiteSpace(respuesta.Contenido) &&
                string.IsNullOrWhiteSpace(respuesta.Codigo) &&
                string.IsNullOrWhiteSpace(respuesta.ImagenUrl))
            {
                mensajeError = "Debe escribir una respuesta, pegar un bloque de código o subir una imagen.";
                return false;
            }

            return true;
        }
    }
}
