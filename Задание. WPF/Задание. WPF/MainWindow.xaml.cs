using System.Windows;
using System.Windows.Controls;

namespace Задание._WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chBox = (CheckBox)sender;
            MessageBox.Show(chBox.Content.ToString() + " отмечен");
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton selectedRadio = (RadioButton)sender;
            MessageBox.Show($"Выбрано животное: {selectedRadio.Content}");
        }
    }
}