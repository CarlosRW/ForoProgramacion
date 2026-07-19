using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TechForo.Data.Conexion;
using TechForo.Data.Entidades;

namespace TechForo.Data.Repositorios
{
    // DP - Repository Pattern: esta clase concentra exclusivamente el acceso
    // a SQL Server de la entidad Pregunta.
    public class PreguntaRepository : IPreguntaRepository
    {
        public List<Pregunta> ObtenerTodas()
        {
            List<Pregunta> preguntas = new List<Pregunta>();

            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"SELECT P.PreguntaID, P.Titulo, P.Descripcion, P.Codigo, P.ImagenUrl,
                    P.FechaCreacion, P.UsuarioID, U.Nombre AS UsuarioNombre,
                    P.Etiquetas, P.TotalVistas, P.Resuelta,
                    (SELECT COUNT(1) FROM Respuestas R
                     WHERE R.PreguntaID = P.PreguntaID) AS TotalRespuestas
                    FROM Preguntas P
                    INNER JOIN Usuarios U ON P.UsuarioID = U.UsuarioID
                    ORDER BY P.FechaCreacion DESC";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            preguntas.Add(MapearPregunta(lector));
                        }
                    }
                }
            }

            return preguntas;
        }

        public Pregunta ObtenerPorId(int id)
        {
            Pregunta pregunta = null;

            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"SELECT P.PreguntaID, P.Titulo, P.Descripcion, P.Codigo, P.ImagenUrl,
                    P.FechaCreacion, P.UsuarioID, U.Nombre AS UsuarioNombre,
                    P.Etiquetas, P.TotalVistas, P.Resuelta,
                    (SELECT COUNT(1) FROM Respuestas R
                     WHERE R.PreguntaID = P.PreguntaID) AS TotalRespuestas
                    FROM Preguntas P
                    INNER JOIN Usuarios U ON P.UsuarioID = U.UsuarioID
                    WHERE P.PreguntaID = @PreguntaID";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@PreguntaID", id);
                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            pregunta = MapearPregunta(lector);
                        }
                    }
                }
            }

            return pregunta;
        }

        public int Crear(Pregunta pregunta)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"INSERT INTO Preguntas
                                (Titulo, Descripcion, Codigo, ImagenUrl, UsuarioID, Etiquetas, Resuelta)
                                VALUES
                                (@Titulo, @Descripcion, @Codigo, @ImagenUrl, @UsuarioID, @Etiquetas, @Resuelta);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Titulo", pregunta.Titulo);
                    comando.Parameters.AddWithValue("@Descripcion", pregunta.Descripcion);
                    comando.Parameters.AddWithValue("@Codigo", (object)pregunta.Codigo ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@ImagenUrl", (object)pregunta.ImagenUrl ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@UsuarioID", pregunta.UsuarioID);
                    comando.Parameters.AddWithValue("@Etiquetas", (object)pregunta.Etiquetas ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@Resuelta", pregunta.Resuelta);

                    conexion.Open();
                    return Convert.ToInt32(comando.ExecuteScalar());
                }
            }
        }

        public bool Actualizar(Pregunta pregunta)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE Preguntas
                                SET Titulo = @Titulo,
                                    Descripcion = @Descripcion,
                                    Codigo = @Codigo,
                                    ImagenUrl = @ImagenUrl,
                                    Etiquetas = @Etiquetas,
                                    Resuelta = @Resuelta
                                WHERE PreguntaID = @PreguntaID AND UsuarioID = @UsuarioID";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@PreguntaID", pregunta.PreguntaID);
                    comando.Parameters.AddWithValue("@Titulo", pregunta.Titulo);
                    comando.Parameters.AddWithValue("@Descripcion", pregunta.Descripcion);
                    comando.Parameters.AddWithValue("@Codigo", (object)pregunta.Codigo ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@ImagenUrl", (object)pregunta.ImagenUrl ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@UsuarioID", pregunta.UsuarioID);
                    comando.Parameters.AddWithValue("@Etiquetas", (object)pregunta.Etiquetas ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@Resuelta", pregunta.Resuelta);

                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Eliminar(int preguntaID, int usuarioID)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                conexion.Open();

                using (SqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        // Se valida el propietario dentro de la misma transacción.
                        // Así nunca se borran respuestas de una pregunta ajena.
                        string queryPropietario = @"SELECT COUNT(1)
                                                   FROM Preguntas
                                                   WHERE PreguntaID = @PreguntaID
                                                   AND UsuarioID = @UsuarioID";

                        using (SqlCommand comandoPropietario = new SqlCommand(queryPropietario, conexion, transaccion))
                        {
                            comandoPropietario.Parameters.AddWithValue("@PreguntaID", preguntaID);
                            comandoPropietario.Parameters.AddWithValue("@UsuarioID", usuarioID);

                            if (Convert.ToInt32(comandoPropietario.ExecuteScalar()) == 0)
                            {
                                transaccion.Commit();
                                return false;
                            }
                        }

                        string queryRespuestas = @"DELETE FROM Respuestas
                                                   WHERE PreguntaID = @PreguntaID";

                        using (SqlCommand comandoRespuestas = new SqlCommand(queryRespuestas, conexion, transaccion))
                        {
                            comandoRespuestas.Parameters.AddWithValue("@PreguntaID", preguntaID);
                            comandoRespuestas.ExecuteNonQuery();
                        }

                        string queryPregunta = @"DELETE FROM Preguntas
                                                 WHERE PreguntaID = @PreguntaID
                                                 AND UsuarioID = @UsuarioID";

                        int filasAfectadas;

                        using (SqlCommand comandoPregunta = new SqlCommand(queryPregunta, conexion, transaccion))
                        {
                            comandoPregunta.Parameters.AddWithValue("@PreguntaID", preguntaID);
                            comandoPregunta.Parameters.AddWithValue("@UsuarioID", usuarioID);
                            filasAfectadas = comandoPregunta.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        return filasAfectadas > 0;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool IncrementarVistas(int preguntaID)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE Preguntas
                                 SET TotalVistas = TotalVistas + 1
                                 WHERE PreguntaID = @PreguntaID";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@PreguntaID", preguntaID);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // DP - Data Mapper: convierte una fila de SQL en una entidad Pregunta
        // en un único lugar, evitando repetir el mapeo en cada consulta.
        private Pregunta MapearPregunta(SqlDataReader lector)
        {
            return new Pregunta
            {
                PreguntaID = Convert.ToInt32(lector["PreguntaID"]),
                Titulo = lector["Titulo"].ToString(),
                Descripcion = lector["Descripcion"].ToString(),
                Codigo = lector["Codigo"] == DBNull.Value ? "" : lector["Codigo"].ToString(),
                ImagenUrl = lector["ImagenUrl"] == DBNull.Value ? "" : lector["ImagenUrl"].ToString(),
                FechaCreacion = Convert.ToDateTime(lector["FechaCreacion"]),
                UsuarioID = Convert.ToInt32(lector["UsuarioID"]),
                UsuarioNombre = lector["UsuarioNombre"].ToString(),
                Etiquetas = lector["Etiquetas"] == DBNull.Value ? "" : lector["Etiquetas"].ToString(),
                TotalVistas = Convert.ToInt32(lector["TotalVistas"]),
                TotalRespuestas = Convert.ToInt32(lector["TotalRespuestas"]),
                Resuelta = Convert.ToBoolean(lector["Resuelta"])
            };
        }
    }
}
