using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using ERTC_Client.Helper;

namespace ERTC_Client.Windows
{
    /// <summary>
    /// Interaction logic for ClientIndex.xaml
    /// </summary>
    public partial class ClientIndex : Window
    {
        private DatabaseHelper dbHelper;

        //CONNECT TO THE DATABASE ---------------------------------------------------------------------------------------------------------------------------------------------
        public ClientIndex()
        {

            InitializeComponent();
            string connectionString = "Server=192.168.56.56;Database=ertc;Uid=homestead;Pwd=secret;";
            dbHelper = new DatabaseHelper(connectionString);

            LoadUsers();
            LoadProducts();
        }

        //REFRESH FETCHED DATA ---------------------------------------------------------------------------------------------------------------------------------------------
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
            LoadProducts();
        }

        //FETCH USERS LIST ---------------------------------------------------------------------------------------------------------------------------------------------
        private void LoadUsers()
        {
            var users = dbHelper.GetUsers();
            UsersListBox.ItemsSource = users;
            UsersListBoxForDelete.ItemsSource = users;

        }

        //ASSIGN ROLE TO USER ---------------------------------------------------------------------------------------------------------------------------------------------
        private void AssignRoleButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedItem == null)
            {
                RolesManagementStatusTextBlock.Text = "Please select a user.";
                return;
            }

            if (RolesComboBox.SelectedItem == null)
            {
                RolesManagementStatusTextBlock.Text = "Please select a role.";
                return;
            }

            User selectedUser = (User)UsersListBox.SelectedItem;
            ComboBoxItem selectedRole = (ComboBoxItem)RolesComboBox.SelectedItem;
            string roleName = selectedRole.Tag.ToString();

            if (dbHelper.AssignRoleToUser(selectedUser.Id, roleName))
            {
                RolesManagementStatusTextBlock.Text = "Role assigned successfully!";
            }
            else
            {
                RolesManagementStatusTextBlock.Text = "Failed to assign role.";
            }
        }

        //SELECT USER FROM THE LIST --------------------------------------------------------------------------------------------------------------------------------
        private void UsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersListBox.SelectedItem is User selectedUser)
            {
                // Optionally, can display some information about the selected user.
                // e.g., RolesManagementStatusTextBlock.Text = $"Selected: {selectedUser.name} ({selectedUser.email})";
            }
        }


        //CREATE USERS ---------------------------------------------------------------------------------------------------------------------------------------------
        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NewUserNameTextBox.Text;
            string email = NewUserEmailTextBox.Text;
            string password = NewUserPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                UserManagementStatusTextBlock.Text = "Name, email, and password cannot be empty.";
                return;
            }

            if (dbHelper.CreateUser(name, email, password))
            {
                UserManagementStatusTextBlock.Text = "User created successfully!";
                // Clear the input fields
                NewUserNameTextBox.Text = string.Empty;
                NewUserEmailTextBox.Text = string.Empty;
                NewUserPasswordBox.Password = string.Empty;
                LoadUsers(); // Refresh the users list after creation
            }
            else
            {
                UserManagementStatusTextBlock.Text = "Failed to create user.";
            }
        }


        //DELETE USERS ---------------------------------------------------------------------------------------------------------------------------------------------
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBoxForDelete.SelectedItem == null)
            {
                UserManagementStatusTextBlock.Text = "Please select a user to delete.";
                return;
            }

            User selectedUser = (User)UsersListBoxForDelete.SelectedItem;

            if (dbHelper.DeleteUser(selectedUser.Id))
            {
                UserManagementStatusTextBlock.Text = "User deleted successfully!";
                LoadUsers(); // Refresh the users list after deletion
            }
            else
            {
                UserManagementStatusTextBlock.Text = "Failed to delete user.";
            }
        }


        //FETCH PRODUCT LIST --------------------------------------------------------------------------------------------------------------------------------
        private void LoadProducts()
        {
            var products = dbHelper.GetProducts();
            ProductsListBox.ItemsSource = products;
        }

        //SELECT PRODUCT FROM THE LIST --------------------------------------------------------------------------------------------------------------------------------
        private void ProductsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsListBox.SelectedItem is Product selectedProduct)
            {
                ProductNameTextBox.Text = selectedProduct.nom_produit;
                ProductTypeTextBox.Text = selectedProduct.type_produit;
                ProductDescriptionTextBox.Text = selectedProduct.description;
            }
        }


        //UPDATE A PRODUCT --------------------------------------------------------------------------------------------------------------------------------
        private void UpdateProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListBox.SelectedItem is Product selectedProduct)
            {
                selectedProduct.nom_produit = ProductNameTextBox.Text;
                selectedProduct.type_produit = ProductTypeTextBox.Text;
                selectedProduct.description = ProductDescriptionTextBox.Text;
                selectedProduct.updated_at = DateTime.Now;

                if (dbHelper.UpdateProduct(selectedProduct))
                {
                    ProductManagementStatusTextBlock.Text = "Product updated successfully!";
                    LoadProducts();
                }
                else
                {
                    ProductManagementStatusTextBlock.Text = "Failed to update product.";
                }
            }
        }

        //DELETE A PRODUCT --------------------------------------------------------------------------------------------------------------------------------
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListBox.SelectedItem is Product selectedProduct)
            {
                if (dbHelper.DeleteProduct(selectedProduct.Id))
                {
                    ProductManagementStatusTextBlock.Text = "Product deleted successfully!";
                    LoadProducts();
                    ProductNameTextBox.Clear();
                    ProductTypeTextBox.Clear();
                    ProductDescriptionTextBox.Clear();
                }
                else
                {
                    ProductManagementStatusTextBlock.Text = "Failed to delete product.";
                }
            }
        }

        //LOGOUT ---------------------------------------------------------------------------------------------------------------------------------------------
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the MainWindow (Login window)
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();

            // Close the current window
            this.Close();
        }
    }
}
