using System.Configuration;
using System.Data.SqlClient;

namespace TechForo.Data.Conexion
{
    // Centraliza la creación de conexiones
    public class ConexionDB
    {
        public static SqlConnection ObtenerConexion()
        {
            string cadena = ConfigurationManager.ConnectionStrings["ForoConnectionString"].ConnectionString;
            return new SqlConnection(cadena);
        }
    }
}