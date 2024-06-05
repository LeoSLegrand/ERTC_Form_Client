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
                RolesManagementStatusTextBlock.Text = "Veuiller Choisir un Compte.";
                return;
            }

            if (RolesComboBox.SelectedItem == null)
            {
                RolesManagementStatusTextBlock.Text = "Veuiller Choisir un Rôle à Attribuer.";
                return;
            }

            UserWithRoles selectedUser = (UserWithRoles)UsersListBox.SelectedItem;
            ComboBoxItem selectedRole = (ComboBoxItem)RolesComboBox.SelectedItem;
            string roleName = selectedRole.Tag.ToString();

            if (dbHelper.AssignRoleToUser(selectedUser.Id, roleName))
            {
                RolesManagementStatusTextBlock.Text = "Rôle Assigner avec Succès!";
                LoadUsers(); // Refresh the user list to reflect the changes
            }
            else
            {
                RolesManagementStatusTextBlock.Text = "Echec de l'Assignement du Rôle.";
            }
        }

        //REMOVE USER ROLE ---------------------------------------------------------------------------------------------------------------------------------------------
        private void RemoveRoleButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedItem == null)
            {
                RolesManagementStatusTextBlock.Text = "Veuiller Choisir un Compte.";
                return;
            }

            if (RolesComboBox.SelectedItem == null)
            {
                RolesManagementStatusTextBlock.Text = "Veuiller Choisir un Rôle à Retirer.";
                return;
            }

            UserWithRoles selectedUser = (UserWithRoles)UsersListBox.SelectedItem;
            ComboBoxItem selectedRole = (ComboBoxItem)RolesComboBox.SelectedItem;
            string roleName = selectedRole.Tag.ToString();

            if (dbHelper.RemoveRoleFromUser(selectedUser.Id, roleName))
            {
                RolesManagementStatusTextBlock.Text = "Rôle Retiré avec Succès!";
                LoadUsers(); // Refresh the user list to reflect the changes
            }
            else
            {
                RolesManagementStatusTextBlock.Text = "Echec de la Suppression du Rôle.";
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
                UserManagementStatusTextBlock.Text = "Les champs: Nom, Email, et Mot de passe ne peuvent êtres vide.";
                return;
            }

            if (dbHelper.CreateUser(name, email, password))
            {
                UserManagementStatusTextBlock.Text = "Utilisateur créé avec succès!";
                // Clear the input fields
                NewUserNameTextBox.Text = string.Empty;
                NewUserEmailTextBox.Text = string.Empty;
                NewUserPasswordBox.Password = string.Empty;
                LoadUsers(); // Refresh the users list after creation
            }
            else
            {
                UserManagementStatusTextBlock.Text = "Echec de création de l'utilisateur.";
            }
        }


        //DELETE USERS ---------------------------------------------------------------------------------------------------------------------------------------------
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {

            if (UsersListBoxForDelete.SelectedItem == null)
            {
                UserManagementStatusTextBlock.Text = "Veuillez sélectionner l'utilisateur à supprimer.";
                return;
            }

            User selectedUser = (User)UsersListBoxForDelete.SelectedItem;

            if (dbHelper.DeleteUser(selectedUser.email))
            {
                UserManagementStatusTextBlock.Text = "Utilisateur supprimé avec succès!";
                LoadUsers(); // Refresh the user list
            }
            else
            {
                UserManagementStatusTextBlock.Text = "Echec lors de la Suppression de l'Utilisateur.";
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
                    ProductManagementStatusTextBlock.Text = "Produit Modifié avec Succès!";
                    LoadProducts();
                }
                else
                {
                    ProductManagementStatusTextBlock.Text = "Echec de la Mise à Jour du Produit.";
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
                    ProductManagementStatusTextBlock.Text = "Produit Supprimé avec Succès!";
                    LoadProducts();
                    ProductNameTextBox.Clear();
                    //ProductTypeTextBox.Clear();
                    ProductDescriptionTextBox.Clear();
                }
                else
                {
                    ProductManagementStatusTextBlock.Text = "Echec lors de la Suppression du Produit.";
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
                EntrepriseManagementStatusTextBlock.Text = "Tous les champs doivent êtres remplit.";
                return;
            }

            if (dbHelper.CreateEntreprise(nomEntreprise, responsable, pays))
            {
                EntrepriseManagementStatusTextBlock.Text = "Entreprise Créé avec Succès!";
                ClearEntrepriseInputFields();
                LoadEntreprises();
            }
            else
            {
                EntrepriseManagementStatusTextBlock.Text = "Echec de la Création de l'Entreprise.";
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
                    EntrepriseManagementStatusTextBlock.Text = "Tous les champs doivent êtres remplit";
                    return;
                }

                if (dbHelper.UpdateEntreprise(selectedEntreprise.Id, nomEntreprise, responsable, pays))
                {
                    EntrepriseManagementStatusTextBlock.Text = "Entreprise Modifié avec Succès!";
                    ClearEntrepriseInputFields();
                    LoadEntreprises();
                }
                else
                {
                    EntrepriseManagementStatusTextBlock.Text = "Eched de la Modification de l'Entreprie.";
                }
            }
            else
            {
                EntrepriseManagementStatusTextBlock.Text = "Veulliez Sélectionner une Entreprise à Modifier.";
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
