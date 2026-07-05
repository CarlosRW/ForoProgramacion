using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TechForo.Data.Conexion;
using TechForo.Data.Entidades;

namespace TechForo.Data.Repositorios
{
    public class RespuestaRepository
    {
        public List<Respuesta> ObtenerPorPregunta(int preguntaID)
        {
            List<Respuesta> respuestas = new List<Respuesta>();

            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"SELECT R.RespuestaID, R.Contenido, R.Codigo, R.ImagenUrl, R.FechaCreacion,
                                R.UsuarioID, R.PreguntaID, U.Nombre AS UsuarioNombre
                                FROM Respuestas R
                                INNER JOIN Usuarios U ON R.UsuarioID = U.UsuarioID
                                WHERE R.PreguntaID = @PreguntaID
                                ORDER BY R.FechaCreacion ASC";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@PreguntaID", preguntaID);

                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            respuestas.Add(MapearRespuesta(lector));
                        }
                    }
                }
            }

            return respuestas;
        }

        public Respuesta ObtenerPorId(int respuestaID)
        {
            Respuesta respuesta = null;

            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"SELECT R.RespuestaID, R.Contenido, R.Codigo, R.ImagenUrl, R.FechaCreacion,
                                R.UsuarioID, R.PreguntaID, U.Nombre AS UsuarioNombre
                                FROM Respuestas R
                                INNER JOIN Usuarios U ON R.UsuarioID = U.UsuarioID
                                WHERE R.RespuestaID = @RespuestaID";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@RespuestaID", respuestaID);

                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            respuesta = MapearRespuesta(lector);
                        }
                    }
                }
            }

            return respuesta;
        }

        public void Crear(Respuesta respuesta)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"INSERT INTO Respuestas (Contenido, Codigo, ImagenUrl, UsuarioID, PreguntaID)
                                VALUES (@Contenido, @Codigo, @ImagenUrl, @UsuarioID, @PreguntaID)";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Contenido", string.IsNullOrWhiteSpace(respuesta.Contenido) ? "" : respuesta.Contenido);
                    comando.Parameters.AddWithValue("@Codigo", string.IsNullOrWhiteSpace(respuesta.Codigo) ? (object)DBNull.Value : respuesta.Codigo);
                    comando.Parameters.AddWithValue("@ImagenUrl", string.IsNullOrWhiteSpace(respuesta.ImagenUrl) ? (object)DBNull.Value : respuesta.ImagenUrl);
                    comando.Parameters.AddWithValue("@UsuarioID", respuesta.UsuarioID);
                    comando.Parameters.AddWithValue("@PreguntaID", respuesta.PreguntaID);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Respuesta respuesta)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE Respuestas
                                SET Contenido = @Contenido,
                                    Codigo = @Codigo,
                                    ImagenUrl = @ImagenUrl
                                WHERE RespuestaID = @RespuestaID
                                AND UsuarioID = @UsuarioID";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@RespuestaID", respuesta.RespuestaID);
                    comando.Parameters.AddWithValue("@Contenido", string.IsNullOrWhiteSpace(respuesta.Contenido) ? "" : respuesta.Contenido);
                    comando.Parameters.AddWithValue("@Codigo", string.IsNullOrWhiteSpace(respuesta.Codigo) ? (object)DBNull.Value : respuesta.Codigo);
                    comando.Parameters.AddWithValue("@ImagenUrl", string.IsNullOrWhiteSpace(respuesta.ImagenUrl) ? (object)DBNull.Value : respuesta.ImagenUrl);
                    comando.Parameters.AddWithValue("@UsuarioID", respuesta.UsuarioID);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int respuestaID, int usuarioID)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"DELETE FROM Respuestas
                                WHERE RespuestaID = @RespuestaID
                                AND UsuarioID = @UsuarioID";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@RespuestaID", respuestaID);
                    comando.Parameters.AddWithValue("@UsuarioID", usuarioID);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        private Respuesta MapearRespuesta(SqlDataReader lector)
        {
            return new Respuesta
            {
                RespuestaID = Convert.ToInt32(lector["RespuestaID"]),
                Contenido = lector["Contenido"].ToString(),
                Codigo = lector["Codigo"] == DBNull.Value ? "" : lector["Codigo"].ToString(),
                ImagenUrl = lector["ImagenUrl"] == DBNull.Value ? "" : lector["ImagenUrl"].ToString(),
                FechaCreacion = Convert.ToDateTime(lector["FechaCreacion"]),
                UsuarioID = Convert.ToInt32(lector["UsuarioID"]),
                PreguntaID = Convert.ToInt32(lector["PreguntaID"]),
                UsuarioNombre = lector["UsuarioNombre"].ToString()
            };
        }
    }
}