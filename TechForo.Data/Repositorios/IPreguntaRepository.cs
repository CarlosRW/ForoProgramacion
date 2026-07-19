using System.Collections.Generic;
using TechForo.Data.Entidades;

namespace TechForo.Data.Repositorios
{
    // DP - Repository Pattern: define el contrato de persistencia de Preguntas
    // y mantiene las consultas SQL fuera de la capa de negocio.
    // SOLID - ISP: el contrato contiene solamente las operaciones que
    // PreguntaBusiness necesita para trabajar con preguntas.
    public interface IPreguntaRepository
    {
        List<Pregunta> ObtenerTodas();
        Pregunta ObtenerPorId(int id);
        int Crear(Pregunta pregunta);
        bool Actualizar(Pregunta pregunta);
        bool Eliminar(int preguntaID, int usuarioID);
        bool IncrementarVistas(int preguntaID);
    }
}
