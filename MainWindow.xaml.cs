using System;
using System.Data.SqlClient;
using System.Windows;

namespace AutistSS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Обработчик события для кнопки входа
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (AuthenticateUser(username, password, out bool isAdmin))
            {
                MessageBox.Show("Вход выполнен успешно!");
                DataWindow dataWindow = new DataWindow(isAdmin, this);
                dataWindow.Show();
                this.Hide(); // Скрываем главное окно вместо закрытия
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.");
            }
        }

        // Метод для аутентификации пользователя
        private bool AuthenticateUser(string username, string password, out bool isAdmin)
        {
            string connectionString = "Data Source=KOPTEV;Initial Catalog=Aut;Integrated Security=True";
            isAdmin = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Role FROM Users WHERE Username=@Username AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                connection.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    isAdmin = (string)result == "Admin";
                    return true;
                }

                return false;
            }
        }
    }
}
