using System.Collections.Generic;
using TechForo.Data.Entidades;
using TechForo.Data.Repositorios;

namespace TechForo.Core.Business
{
    public class RespuestaBusiness
    {
        private readonly RespuestaRepository _respuestaRepository;

        public RespuestaBusiness()
        {
            _respuestaRepository = new RespuestaRepository();
        }

        public List<Respuesta> ObtenerPorPregunta(int preguntaID)
        {
            return _respuestaRepository.ObtenerPorPregunta(preguntaID);
        }

        public Respuesta ObtenerPorId(int respuestaID)
        {
            return _respuestaRepository.ObtenerPorId(respuestaID);
        }

        public void Crear(Respuesta respuesta)
        {
            _respuestaRepository.Crear(respuesta);
        }

        public void Actualizar(Respuesta respuesta)
        {
            _respuestaRepository.Actualizar(respuesta);
        }

        public void Eliminar(int respuestaID, int usuarioID)
        {
            _respuestaRepository.Eliminar(respuestaID, usuarioID);
        }
    }
}