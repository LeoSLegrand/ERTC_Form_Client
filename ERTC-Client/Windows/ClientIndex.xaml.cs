﻿using System;
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
        }

        //FETCH USERS LIST ---------------------------------------------------------------------------------------------------------------------------------------------
        private void LoadUsers()
        {
            List<User> users = dbHelper.GetUsers();
            UsersListBox.ItemsSource = users;
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

        //ADD USERS ---------------------------------------------------------------------------------------------------------------------------------------------
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
            }
            else
            {
                UserManagementStatusTextBlock.Text = "Failed to create user.";
            }
        }

        //DELETE USERS ---------------------------------------------------------------------------------------------------------------------------------------------
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            string email = DeleteUserEmailTextBox.Text;

            if (string.IsNullOrWhiteSpace(email))
            {
                UserManagementStatusTextBlock.Text = "Email cannot be empty.";
                return;
            }

            if (dbHelper.DeleteUser(email))
            {
                UserManagementStatusTextBlock.Text = "User deleted successfully!";
                // Clear the input field
                DeleteUserEmailTextBox.Text = string.Empty;
            }
            else
            {
                UserManagementStatusTextBlock.Text = "Failed to delete user.";
            }
        }

        //TODO OR MAYBE DELETE --------------------------------------------------------------------------------------------------------------------------------
        private void UsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle the event if necessary
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
