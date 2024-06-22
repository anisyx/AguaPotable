using System;
using System.Data;
using System.Data.SqlClient;

namespace AguaPotable
{
    internal class DBManager
    {
        private static string connectionString = "Data Source=DESKTOP-979S3GS;Initial Catalog=AguaPotable;Integrated Security=True";

        public static DataTable BuscarCliente(string searchTerm)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Clientes WHERE Nombre LIKE @SearchTerm";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                        connection.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción o relanzarla según sea necesario
                throw new Exception("Error al buscar cliente en la base de datos: " + ex.Message);
            }

            return dataTable;
        }

        public static DataTable ObtenerTodosLosClientes()
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Clientes";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción o relanzarla según sea necesario
                throw new Exception("Error al obtener todos los clientes: " + ex.Message);
            }

            return dataTable;
        }

        public static DataTable ObtenerPagosCliente(int clienteId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT Mes, Anio FROM Pagos WHERE ClienteID = @ClienteID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClienteID", clienteId);

                        connection.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción o relanzarla según sea necesario
                throw new Exception("Error al obtener pagos del cliente: " + ex.Message);
            }

            return dataTable;
        }

        public static DataTable ObtenerPagosClienteImprimir(int clienteId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT Mes,Anio, Monto, Mora, FechaPago, Nombre, Direccion, Canton, Telefono, (SELECT SUM(Monto) + SUM(COALESCE(Mora, 0)) FROM Pagos WHERE Pagos.ClienteID = Clientes.ClienteID and CAST(FechaPago AS DATE) = CAST(GETDATE() AS DATE) AND Pagos.ClienteID = @ClienteID) as Cuota, (SELECT SUM(Monto) FROM Pagos WHERE Pagos.ClienteID = Clientes.ClienteID and CAST(FechaPago AS DATE) = CAST(GETDATE() AS DATE) AND Pagos.ClienteID = @ClienteID) as SubTotal, (SELECT SUM(Mora) FROM Pagos WHERE Pagos.ClienteID = Clientes.ClienteID and CAST(FechaPago AS DATE) = CAST(GETDATE() AS DATE) AND Pagos.ClienteID = @ClienteID) as TotMora FROM Pagos  INNER JOIN Clientes ON Pagos.ClienteID = Clientes.ClienteID WHERE CAST(FechaPago AS DATE) = CAST(GETDATE() AS DATE) AND Pagos.ClienteID = @ClienteID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClienteID", clienteId);

                        connection.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción o relanzarla según sea necesario
                throw new Exception("Error al obtener pagos para imprimir: " + ex.Message);
            }

            return dataTable;
        }

        public static void GuardarPago(int IdCliente, int mes, int anio, decimal monto, decimal mora, DateTime fechaPago)
        {
            string query = "INSERT INTO Pagos (ClienteID, Mes, Anio, Monto, Mora, FechaPago) VALUES (@ClienteID, @Mes, @Anio, @Monto, @Mora, @FechaPago)";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ClienteID", IdCliente);
                    command.Parameters.AddWithValue("@Mes", mes);
                    command.Parameters.AddWithValue("@Anio", anio);
                    command.Parameters.AddWithValue("@Monto", monto);
                    command.Parameters.AddWithValue("@Mora", mora);
                    command.Parameters.AddWithValue("@FechaPago", fechaPago);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el pago en la base de datos: " + ex.Message);
            }
        }

        public static void EliminarPago(int clienteId, int mes, int anio)
        {
            // Verificar si existe un registro de pago para el cliente, mes y año especificados
            if (ExisteRegistroPago(clienteId, mes, anio))
            {
                DateTime fechaPago = ObtenerFechaPago(clienteId, mes, anio);

                // Verificar si la fecha de pago es la misma que la fecha actual
                if (fechaPago.Date == DateTime.Today.Date)
                {
                    // Eliminar el registro de pago
                    string query = "DELETE FROM Pagos WHERE ClienteID = @ClienteID AND Mes = @Mes AND Anio = @Anio";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ClienteID", clienteId);
                        command.Parameters.AddWithValue("@Mes", mes);
                        command.Parameters.AddWithValue("@Anio", anio);

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error al eliminar el registro de pago: " + ex.Message);
                        }
                    }
                }
                else
                {
                    throw new Exception("No se puede eliminar el registro de pago porque la fecha de pago no coincide con la fecha actual.");
                }
            }
            else
            {
                throw new Exception("No existe un registro de pago para el cliente, mes y año especificados.");
            }
        }

        // Método para verificar si existe un registro de pago para el cliente, mes y año especificados
        public static bool ExisteRegistroPago(int clienteId, int mes, int anio)
        {
            string query = "SELECT COUNT(*) FROM Pagos WHERE ClienteID = @ClienteID AND Mes = @Mes AND Anio = @Anio";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClienteID", clienteId);
                command.Parameters.AddWithValue("@Mes", mes);
                command.Parameters.AddWithValue("@Anio", anio);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        // Método para obtener la fecha de pago para el cliente, mes y año especificados
        public static DateTime ObtenerFechaPago(int clienteId, int mes, int anio)
        {
            string query = "SELECT FechaPago FROM Pagos WHERE ClienteID = @ClienteID AND Mes = @Mes AND Anio = @Anio";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClienteID", clienteId);
                command.Parameters.AddWithValue("@Mes", mes);
                command.Parameters.AddWithValue("@Anio", anio);

                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return (DateTime)result;
                }
                else
                {
                    throw new Exception("No se encontró la fecha de pago para el cliente, mes y año especificados.");
                }
            }
        }
        public static decimal ObtenerCuotaCliente(int clienteId)
        {
            decimal cuota = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Cuota FROM Clientes WHERE ClienteID = @ClienteID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClienteID", clienteId);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        cuota = Convert.ToDecimal(result);
                    }
                }
                catch (Exception ex)
                {
                    // Manejar excepciones según sea necesario
                    Console.WriteLine("Error al obtener la cuota del cliente: " + ex.Message);
                }
            }

            return cuota;
        }

        public static DataTable ObtenerClientes()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ClienteID, Nombre, Direccion, Canton, Telefono, Cuota FROM Clientes";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
            }
            return dt;
        }

        // Insertar un nuevo cliente
        public static void InsertarCliente(string nombre, string direccion, string canton, string telefono, decimal cuota)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Clientes (Nombre, Direccion, Canton, Telefono, Cuota) VALUES (@Nombre, @Direccion, @Canton, @Telefono, @Cuota)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Direccion", direccion);
                cmd.Parameters.AddWithValue("@Canton", canton);
                cmd.Parameters.AddWithValue("@Telefono", telefono);
                cmd.Parameters.AddWithValue("@Cuota", cuota);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Actualizar un cliente existente
       

        // Eliminar un cliente
        public static void EliminarCliente(int clienteID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Clientes WHERE ClienteID = @ClienteID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClienteID", clienteID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public static DataRow ObtenerClientePorID(int clienteID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Clientes WHERE ClienteID = @ClienteID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClienteID", clienteID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public static void EditarCliente(int clienteID, string nombre, string direccion, string canton, string telefono, decimal cuota)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Clientes SET Nombre = @Nombre, Direccion = @Direccion, Canton = @Canton, Telefono = @Telefono, Cuota = @Cuota WHERE ClienteID = @ClienteID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClienteID", clienteID);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Direccion", direccion);
                cmd.Parameters.AddWithValue("@Canton", canton);
                cmd.Parameters.AddWithValue("@Telefono", telefono);
                cmd.Parameters.AddWithValue("@Cuota", cuota);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


    }
}
