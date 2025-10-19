using System.Windows;
using System.Windows.Controls;
using Finance.Localization;

namespace Finance
{
    public partial class SideBar : UserControl
    {
        public SideBar()
        {
            InitializeComponent();
        }

        private void CloseAppButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                TranslationManager.Instance["Dialog.CloseApp"],
                TranslationManager.Instance["Dialog.CloseAppTitle"],
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Window.GetWindow(this)?.Close();
        }

        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navigation vers Projets", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TransactionsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navigation vers Transactions", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navigation vers Rapports", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                TranslationManager.Instance["Dialog.Help"], 
                TranslationManager.Instance["Dialog.HelpTitle"], 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                TranslationManager.Instance["Dialog.Logout"], 
                TranslationManager.Instance["Dialog.LogoutTitle"], 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            Window.GetWindow(this)?.Close();
        }
    }
}
