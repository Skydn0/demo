using System;
using System.Windows;

namespace AutistSS
{
    public partial class EditWindow : Window
    {
        public string ItemName { get; private set; }
        public int ItemValue { get; private set; }

        public EditWindow(string name = "", int value = 0)
        {
            InitializeComponent();
            NameTextBox.Text = name;
            ValueTextBox.Text = value.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ItemName = NameTextBox.Text;
            ItemValue = int.Parse(ValueTextBox.Text);
            this.DialogResult = true;
            this.Close();
        }
    }
}
