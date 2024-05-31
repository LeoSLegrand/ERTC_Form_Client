using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using System.Data;

namespace ERTC_Client.Helper
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool AuthenticateUser(string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT password FROM users WHERE email = @Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHashedPassword = reader["password"].ToString();

                            // Verify the input password with the stored hashed password
                            if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception (log it, show message to user, etc.)
                    Console.WriteLine($"Authentication failed: {ex.Message}");
                }
            }
            return false;
        }

        public bool TestConnection()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection failed: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
