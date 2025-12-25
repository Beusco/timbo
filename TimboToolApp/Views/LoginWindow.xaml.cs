using System.Windows;

namespace TimboToolApp.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameBox.Text == "Barry" && PasswordBox.Password == "@123456")
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ErrorText.Text = "Invalid username or password.";
            }
        }
    }
}
