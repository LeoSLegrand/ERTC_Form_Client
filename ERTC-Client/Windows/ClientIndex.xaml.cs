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
            LoadEntreprises();
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

            var usersWithRoles = dbHelper.GetUsersWithRoles();
            UsersListBox.ItemsSource = usersWithRoles;
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

            UserWithRoles selectedUser = (UserWithRoles)UsersListBox.SelectedItem;
            ComboBoxItem selectedRole = (ComboBoxItem)RolesComboBox.SelectedItem;
            string roleName = selectedRole.Tag.ToString();

            if (dbHelper.AssignRoleToUser(selectedUser.Id, roleName))
            {
                RolesManagementStatusTextBlock.Text = "Role assigned successfully!";
                LoadUsers(); // Refresh the user list to reflect the changes
            }
            else
            {
                RolesManagementStatusTextBlock.Text = "Failed to assign role.";
            }
        }

        //REMOVE USER ROLE ---------------------------------------------------------------------------------------------------------------------------------------------
        private void RemoveRoleButton_Click(object sender, RoutedEventArgs e)
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

            UserWithRoles selectedUser = (UserWithRoles)UsersListBox.SelectedItem;
            ComboBoxItem selectedRole = (ComboBoxItem)RolesComboBox.SelectedItem;
            string roleName = selectedRole.Tag.ToString();

            if (dbHelper.RemoveRoleFromUser(selectedUser.Id, roleName))
            {
                RolesManagementStatusTextBlock.Text = "Role removed successfully!";
                LoadUsers(); // Refresh the user list to reflect the changes
            }
            else
            {
                RolesManagementStatusTextBlock.Text = "Failed to remove role.";
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

            if (dbHelper.DeleteUser(selectedUser.email))
            {
                UserManagementStatusTextBlock.Text = "User deleted successfully!";
                LoadUsers(); // Refresh the user list
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
                ProductDescriptionTextBox.Text = selectedProduct.description;

                // Iterate through ComboBox items to find and select the matching type_produit
                foreach (ComboBoxItem item in ProductTypeComboBox.Items)
                {
                    if (item.Content.ToString() == selectedProduct.type_produit)
                    {
                        ProductTypeComboBox.SelectedItem = item;
                        break;
                    }
                }

                // Load product enterprises and select the current one
                LoadProductEnterprises(selectedProduct.entreprise_id);
            }
        }

        //LOAD PRODUCT ENTREPRISE --------------------------------------------------------------------------------------------------------------------------------
        private void LoadProductEnterprises(int selectedEnterpriseId)
        {
            var entreprises = dbHelper.GetEntreprises();
            ProductEnterpriseComboBox.Items.Clear();

            foreach (var entreprise in entreprises)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = entreprise.NomEntreprise,
                    Tag = entreprise.Id
                };

                ProductEnterpriseComboBox.Items.Add(item);

                if (entreprise.Id == selectedEnterpriseId)
                {
                    ProductEnterpriseComboBox.SelectedItem = item;
                }
            }
        }

        //UPDATE A PRODUCT --------------------------------------------------------------------------------------------------------------------------------
        private void UpdateProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListBox.SelectedItem is Product selectedProduct)
            {
                selectedProduct.nom_produit = ProductNameTextBox.Text;
                selectedProduct.type_produit = (ProductTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                selectedProduct.description = ProductDescriptionTextBox.Text;
                selectedProduct.updated_at = DateTime.Now;

                // Retrieve the selected enterprise ID from the ComboBox
                if (ProductEnterpriseComboBox.SelectedItem is ComboBoxItem selectedEnterpriseItem)
                {
                    selectedProduct.entreprise_id = (int)selectedEnterpriseItem.Tag;
                }

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
                    //ProductTypeTextBox.Clear();
                    ProductDescriptionTextBox.Clear();
                }
                else
                {
                    ProductManagementStatusTextBlock.Text = "Failed to delete product.";
                }
            }
        }

        //ENTREPRISE ---------------------------------------------------------------------------------------------------------------------------------------------
        // Load Entreprises when the Entreprise tab is selected
        private void EntrepriseTabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            LoadEntreprises();
        }

        // Fetch and display the list of entreprises
        private void LoadEntreprises()
        {
            var entreprises = dbHelper.GetEntreprises();
            EntreprisesListBox.ItemsSource = entreprises;
        }

        // Create Entreprise
        private void CreateEntrepriseButton_Click(object sender, RoutedEventArgs e)
        {
            string nomEntreprise = EntrepriseNameTextBox.Text;
            string responsable = EntrepriseResponsableTextBox.Text;
            string pays = EntreprisePaysTextBox.Text;

            if (string.IsNullOrWhiteSpace(nomEntreprise) || string.IsNullOrWhiteSpace(responsable) || string.IsNullOrWhiteSpace(pays))
            {
                EntrepriseManagementStatusTextBlock.Text = "All fields are required.";
                return;
            }

            if (dbHelper.CreateEntreprise(nomEntreprise, responsable, pays))
            {
                EntrepriseManagementStatusTextBlock.Text = "Entreprise created successfully!";
                ClearEntrepriseInputFields();
                LoadEntreprises();
            }
            else
            {
                EntrepriseManagementStatusTextBlock.Text = "Failed to create entreprise.";
            }
        }

        // Update Entreprise
        private void UpdateEntrepriseButton_Click(object sender, RoutedEventArgs e)
        {
            if (EntreprisesListBox.SelectedItem is Entreprise selectedEntreprise)
            {
                string nomEntreprise = EntrepriseNameTextBox.Text;
                string responsable = EntrepriseResponsableTextBox.Text;
                string pays = EntreprisePaysTextBox.Text;

                if (string.IsNullOrWhiteSpace(nomEntreprise) || string.IsNullOrWhiteSpace(responsable) || string.IsNullOrWhiteSpace(pays))
                {
                    EntrepriseManagementStatusTextBlock.Text = "All fields are required.";
                    return;
                }

                if (dbHelper.UpdateEntreprise(selectedEntreprise.Id, nomEntreprise, responsable, pays))
                {
                    EntrepriseManagementStatusTextBlock.Text = "Entreprise updated successfully!";
                    ClearEntrepriseInputFields();
                    LoadEntreprises();
                }
                else
                {
                    EntrepriseManagementStatusTextBlock.Text = "Failed to update entreprise.";
                }
            }
            else
            {
                EntrepriseManagementStatusTextBlock.Text = "Please select an entreprise to update.";
            }
        }

        // Handle selection change in the ListBox
        private void EntreprisesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EntreprisesListBox.SelectedItem is Entreprise selectedEntreprise)
            {
                EntrepriseNameTextBox.Text = selectedEntreprise.NomEntreprise;
                EntrepriseResponsableTextBox.Text = selectedEntreprise.Responsable;
                EntreprisePaysTextBox.Text = selectedEntreprise.Pays;
            }
        }

        // Clear input fields
        private void ClearEntrepriseInputFields()
        {
            EntrepriseNameTextBox.Clear();
            EntrepriseResponsableTextBox.Clear();
            EntreprisePaysTextBox.Clear();
        }

    }
}
