using System;
using System.Data.SqlClient;
using TechForo.Data.Conexion;
using TechForo.Data.Entidades;

namespace TechForo.Data.Repositorios
{
    public class UsuarioRepository
    {
        public Usuario ObtenerPorCorreo(string correo)
        {
            Usuario usuario = null;

            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = "SELECT UsuarioID, Nombre, Correo, Password FROM Usuarios WHERE Correo = @Correo";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Correo", correo);
                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            usuario = new Usuario
                            {
                                UsuarioID = Convert.ToInt32(lector["UsuarioID"]),
                                Nombre = lector["Nombre"].ToString(),
                                Correo = lector["Correo"].ToString(),
                                Password = lector["Password"].ToString()
                            };
                        }
                    }
                }
            }

            return usuario;
        }

        public bool ExisteCorreo(string correo)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = "SELECT COUNT(1) FROM Usuarios WHERE Correo = @Correo";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Correo", correo);
                    conexion.Open();
                    int total = (int)comando.ExecuteScalar();
                    return total > 0;
                }
            }
        }

        public void Crear(Usuario usuario)
        {
            using (SqlConnection conexion = ConexionDB.ObtenerConexion())
            {
                string query = "INSERT INTO Usuarios (Nombre, Correo, Password) VALUES (@Nombre, @Correo, @Password)";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    comando.Parameters.AddWithValue("@Correo", usuario.Correo);
                    comando.Parameters.AddWithValue("@Password", usuario.Password);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }
    }
}