using System;
using System.Windows;
using Finance.ViewModels;
using Finance.Localization;

namespace Finance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = App.MainViewModel;
            DataContext = ViewModel;
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new Views.InputDialog("Entrez le nom du projet :", "Nouveau Projet");
            if (inputDialog.ShowDialog() == true)
            {
                var newProject = new Models.Project { Name = inputDialog.ResponseText };
                ViewModel.Projects.Add(newProject);
                MessageBox.Show("Projet créé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.CommandParameter is Models.Project selectedProject)
            {
                var result = MessageBox.Show(
                    TranslationManager.Instance["Dialog.DeleteProject"], 
                    TranslationManager.Instance["Dialog.DeleteProjectTitle"], 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    ViewModel.Projects.Remove(selectedProject);
                    MessageBox.Show("Projet supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ProjectCard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is System.Windows.FrameworkElement element && element.DataContext is Models.Project selectedProject)
            {
                // Create a new instance of ProjectWindow each time
                var projectViewModel = new ProjectViewModel(App.CurrencyConverter) 
                { 
                    CurrentProject = selectedProject,
                    PreferredCurrency = App.SettingsViewModel?.PreferredCurrency ?? "ILS"
                };
                var projectWindow = new ProjectWindow(projectViewModel);

                // Hide the main window
                this.Hide();

                // When the project window closes, show the main window again
                projectWindow.Closed += (s, args) => this.Show();

                // Show the project window
                projectWindow.Show();
            }
        }

        private void SideBar_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}