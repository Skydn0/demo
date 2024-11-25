using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace AutistSS
{
    public partial class DataWindow : Window
    {
        private string connectionString = "Data Source=KOPTEV;Initial Catalog=Aut;Integrated Security=True";
        private bool isAdmin;
        private int? selectedItemId; // Используем nullable тип для selectedItemId
        private MainWindow mainWindow;

        public DataWindow(bool isAdmin, MainWindow mainWindow)
        {
            InitializeComponent();
            this.isAdmin = isAdmin;
            this.mainWindow = mainWindow;
            if (isAdmin)
            {
                AddButton.Visibility = Visibility.Visible;
                EditButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
            }
            LoadData("SELECT * FROM Items");
        }

        // Метод для загрузки данных из базы данных
        private void LoadData(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                DataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        // Обработчик события для сортировки данных
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string query = "SELECT * FROM Items";

            if (AlphabeticalRadioButton.IsChecked == true)
            {
                query += " ORDER BY Name";
            }
            else if (AscendingRadioButton.IsChecked == true)
            {
                query += " ORDER BY Value ASC";
            }
            else if (DescendingRadioButton.IsChecked == true)
            {
                query += " ORDER BY Value DESC";
            }

            LoadData(query);
        }

        // Обработчик события для выбора элемента в DataGrid
        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)DataGrid.SelectedItem;
                selectedItemId = (int)selectedRow["Id"];
            }
        }

        // Обработчик события для добавления нового элемента
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditWindow editWindow = new EditWindow();
            if (editWindow.ShowDialog() == true)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Items (Name, Value) VALUES (@Name, @Value)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Name", editWindow.ItemName);
                    cmd.Parameters.AddWithValue("@Value", editWindow.ItemValue);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadData("SELECT * FROM Items");
            }
        }

        // Обработчик события для редактирования элемента
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItemId.HasValue)
            {
                DataRowView selectedRow = (DataRowView)DataGrid.SelectedItem;
                string itemName = (string)selectedRow["Name"];
                int itemValue = (int)selectedRow["Value"];

                EditWindow editWindow = new EditWindow(itemName, itemValue);
                if (editWindow.ShowDialog() == true)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Items SET Name = @Name, Value = @Value WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@Name", editWindow.ItemName);
                        cmd.Parameters.AddWithValue("@Value", editWindow.ItemValue);
                        cmd.Parameters.AddWithValue("@Id", selectedItemId.Value);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }

                    LoadData("SELECT * FROM Items");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        // Обработчик события для удаления элемента
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItemId.HasValue)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Items WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", selectedItemId.Value);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadData("SELECT * FROM Items");
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для удаления.");
            }
        }

        // Обработчик события для кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            this.Close();
        }

        // Обработчик события для кнопки "Выход"
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
