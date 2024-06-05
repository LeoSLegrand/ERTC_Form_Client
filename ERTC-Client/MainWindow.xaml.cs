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
                StatusTextBlock.Text = "Echec de la connexion à la base de donnée.";
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                StatusTextBlock.Text = "L'email et le mot de passe ne peuvent êtres vide.";
                return;
            }

            if (dbHelper.AuthenticateUser(email, password))
            {
                StatusTextBlock.Text = "Connexion réussite!";

                // Open the Index window
                ClientIndex indexWindow = new ClientIndex();
                indexWindow.Show();
                this.Close();
            }
            else
            {
                StatusTextBlock.Text = "Mot de passe ou email invalide, ou vôtre compte n'a pas le rôle admin.";
            }
        }

    }
}
