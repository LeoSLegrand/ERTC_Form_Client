using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ERTC_Client.Helper;
using ERTC_Client.Windows;
using BCrypt.Net;

namespace ERTC_Client
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper;

        public MainWindow()
        {
            InitializeComponent();

            string connectionString = "Server=192.168.56.56;Database=ertc;Uid=homestead;Pwd=secret;";
            dbHelper = new DatabaseHelper(connectionString);

            if (!dbHelper.TestConnection())
            {
                StatusTextBlock.Text = "Failed to connect to database.";
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (dbHelper.AuthenticateUser(email, password))
            {
                StatusTextBlock.Text = "Login successful!";

                // Open the Index window
                ClientIndex indexWindow = new ClientIndex();
                indexWindow.Show();

                // Close the current window
                this.Close();
            }
            else
            {
                StatusTextBlock.Text = "Login failed. Please check your credentials.";
            }
        }
    }
}
