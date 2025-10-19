using System.Windows;

namespace Finance
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = App.SettingsViewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            App.SettingsViewModel.SaveSettings();
            MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
