using System;
using Microsoft.Data.SqlClient;

namespace BibliotecaMVC
{
    public static class DatabaseConnection
    {
        // Cadena de conexión centralizada
        private static readonly string connectionString = "Server=localhost;Database=BibliotecaDB;Trusted_Connection=True;";

        /// <summary>
        /// Obtiene una conexión abierta a la base de datos.
        /// </summary>
        /// <returns>Una instancia de SqlConnection abierta.</returns>
        public static SqlConnection GetConnection()
        {
            try
            {
                var connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al conectar con la base de datos: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw;
            }
        }
    }
}