using System.Collections.Generic;
using TechForo.Data.Entidades;

namespace TechForo.Data.Repositorios
{
    // DP - Repository Pattern: define el contrato de persistencia de Respuestas
    // y mantiene las consultas SQL fuera de la capa de negocio.
    // SOLID - ISP: el contrato contiene solamente las operaciones que
    // RespuestaBusiness necesita para trabajar con respuestas.
    public interface IRespuestaRepository
    {
        List<Respuesta> ObtenerPorPregunta(int preguntaID);
        Respuesta ObtenerPorId(int respuestaID);
        void Crear(Respuesta respuesta);
        void Actualizar(Respuesta respuesta);
        void Eliminar(int respuestaID, int usuarioID);
    }
}
