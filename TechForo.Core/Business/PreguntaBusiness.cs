using System.Collections.Generic;
using TechForo.Data.Entidades;
using TechForo.Data.Repositorios;

namespace TechForo.Core.Business
{
    public class PreguntaBusiness
    {
        private readonly PreguntaRepository _preguntaRepository;

        public PreguntaBusiness()
        {
            _preguntaRepository = new PreguntaRepository();
        }

        public List<Pregunta> ObtenerTodas()
        {
            return _preguntaRepository.ObtenerTodas();
        }

        public Pregunta ObtenerPorId(int id)
        {
            return _preguntaRepository.ObtenerPorId(id);
        }

        public void Crear(Pregunta pregunta)
        {
            _preguntaRepository.Crear(pregunta);
        }

        public void Actualizar(Pregunta pregunta)
        {
            _preguntaRepository.Actualizar(pregunta);
        }

        public void Eliminar(int preguntaID, int usuarioID)
        {
            _preguntaRepository.Eliminar(preguntaID, usuarioID);
        }
    }
}