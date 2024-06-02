using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using System.Data;
using System.Xml.Linq;
using System.Windows.Controls;

namespace ERTC_Client.Helper
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //LOGIN --------------------------------------------------------------------------------------------------------------------------------
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

        //USERS CREATION --------------------------------------------------------------------------------------------------------------------------------
        public bool CreateUser(string name, string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Generate a salt with the default prefix
                    string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                    // Manually replace the prefix with $2y$
                    salt = "$2y$" + salt.Substring(4);

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

                    string query = "INSERT INTO users (name, email, password, created_at, updated_at) VALUES (@Name, @Email, @Password, @CreatedAt, @UpdatedAt)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"User creation failed: {ex.Message}");
                }
            }
            return false;
        }


        //DELETE USERS --------------------------------------------------------------------------------------------------------------------------------
        public bool DeleteUser(string email)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "DELETE FROM users WHERE email = @Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"User deletion failed: {ex.Message}");
                }
            }
            return false;
        }


        //FETCH USERS LIST --------------------------------------------------------------------------------------------------------------------------------
        public List<User> GetUsers()
        {
            var users = new List<User>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id, name, email, password, created_at, updated_at FROM users";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                Id = reader.GetInt32("id"),
                                name = reader.GetString("name"),
                                email = reader.GetString("email"),
                                password = reader.GetString("password"),
                                created_at = reader.GetDateTime("created_at"),
                                updated_at = reader.GetDateTime("updated_at")
                            };
                            users.Add(user);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to retrieve users: {ex.Message}");
                }
            }

            return users;
        }


        //ASSIGN ROLE TO USER --------------------------------------------------------------------------------------------------------------------------------
        public bool AssignRoleToUser(int userId, string roleName)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Get the role ID based on the role name
                    string roleQuery = "SELECT id FROM roles WHERE name = @RoleName";
                    MySqlCommand roleCmd = new MySqlCommand(roleQuery, conn);
                    roleCmd.Parameters.AddWithValue("@RoleName", roleName);

                    int roleId;
                    using (MySqlDataReader roleReader = roleCmd.ExecuteReader())
                    {
                        if (roleReader.Read())
                        {
                            roleId = roleReader.GetInt32("id");
                        }
                        else
                        {
                            Console.WriteLine("Role not found");
                            return false; // Role not found
                        }
                    }

                    // Assign the role to the user
                    string assignRoleQuery = "INSERT INTO assigned_roles (role_id, entity_id, entity_type) VALUES (@RoleId, @UserId, @EntityType)";
                    MySqlCommand assignRoleCmd = new MySqlCommand(assignRoleQuery, conn);
                    assignRoleCmd.Parameters.AddWithValue("@RoleId", roleId);
                    assignRoleCmd.Parameters.AddWithValue("@UserId", userId);
                    assignRoleCmd.Parameters.AddWithValue("@EntityType", "App\\Models\\User");

                    int result = assignRoleCmd.ExecuteNonQuery();
                    return result > 0;
                }
                catch (MySqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.Message}");
                }
            }
            return false;
        }

        //FETCH PRODUCT LIST --------------------------------------------------------------------------------------------------------------------------------
        public List<Product> GetProducts()
        {
            var products = new List<Product>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM produits";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Product
                            {
                                Id = reader.GetInt32("id"),
                                nom_produit = reader.GetString("nom_produit"),
                                type_produit = reader.GetString("type_produit"),
                                description = reader.GetString("description"),
                                created_at = reader.GetDateTime("created_at"),
                                updated_at = reader.GetDateTime("updated_at"),
                                entreprise_id = reader.GetInt32("entreprise_id")
                            };
                            products.Add(product);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to retrieve products: {ex.Message}");
                }
            }

            return products;
        }

        //UPDATE PRODUCT --------------------------------------------------------------------------------------------------------------------------------
        public bool UpdateProduct(Product product)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE produits SET nom_produit = @NomProduit, type_produit = @TypeProduit, description = @Description, updated_at = @UpdatedAt WHERE id = @Id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NomProduit", product.nom_produit);
                    cmd.Parameters.AddWithValue("@TypeProduit", product.type_produit);
                    cmd.Parameters.AddWithValue("@Description", product.description);
                    cmd.Parameters.AddWithValue("@UpdatedAt", product.updated_at);
                    cmd.Parameters.AddWithValue("@Id", product.Id);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to update product: {ex.Message}");
                }
            }
            return false;
        }

        //DELETE PRODUCT --------------------------------------------------------------------------------------------------------------------------------
        public bool DeleteProduct(int productId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "DELETE FROM produits WHERE id = @Id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", productId);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete product: {ex.Message}");
                }
            }
            return false;
        }

        //TEST CONNECTION --------------------------------------------------------------------------------------------------------------------------------
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
