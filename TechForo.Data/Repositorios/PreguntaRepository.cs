using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TechForo.Data.Conexion;
using TechForo.Data.Entidades;

namespace TechForo.Data.Repositorios
{
    public class PreguntaRepository
    {
        public List<Pregunta> ObtenerTodas()
        {
            List<Pregunta> preguntas = new List<Pregunta>();

            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"SELECT P.PreguntaID, P.Titulo, P.Descripcion, P.Codigo, P.ImagenUrl,
                                P.FechaCreacion, P.UsuarioID, U.Nombre AS UsuarioNombre
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
                                P.FechaCreacion, P.UsuarioID, U.Nombre AS UsuarioNombre
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

        public void Crear(Pregunta pregunta)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"INSERT INTO Preguntas (Titulo, Descripcion, Codigo, ImagenUrl, UsuarioID)
                                VALUES (@Titulo, @Descripcion, @Codigo, @ImagenUrl, @UsuarioID)";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Titulo", pregunta.Titulo);
                    comando.Parameters.AddWithValue("@Descripcion", pregunta.Descripcion);
                    comando.Parameters.AddWithValue("@Codigo", (object)pregunta.Codigo ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@ImagenUrl", (object)pregunta.ImagenUrl ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@UsuarioID", pregunta.UsuarioID);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Pregunta pregunta)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE Preguntas
                                SET Titulo = @Titulo,
                                    Descripcion = @Descripcion,
                                    Codigo = @Codigo,
                                    ImagenUrl = @ImagenUrl
                                WHERE PreguntaID = @PreguntaID AND UsuarioID = @UsuarioID";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@PreguntaID", pregunta.PreguntaID);
                    comando.Parameters.AddWithValue("@Titulo", pregunta.Titulo);
                    comando.Parameters.AddWithValue("@Descripcion", pregunta.Descripcion);
                    comando.Parameters.AddWithValue("@Codigo", (object)pregunta.Codigo ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@ImagenUrl", (object)pregunta.ImagenUrl ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@UsuarioID", pregunta.UsuarioID);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int preguntaID, int usuarioID)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                conexion.Open();

                using (SqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
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

                        using (SqlCommand comandoPregunta = new SqlCommand(queryPregunta, conexion, transaccion))
                        {
                            comandoPregunta.Parameters.AddWithValue("@PreguntaID", preguntaID);
                            comandoPregunta.Parameters.AddWithValue("@UsuarioID", usuarioID);
                            comandoPregunta.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

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
                UsuarioNombre = lector["UsuarioNombre"].ToString()
            };
        }
    }
}
