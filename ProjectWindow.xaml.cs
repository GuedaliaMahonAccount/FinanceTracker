using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Finance.ViewModels;
using Finance.Views;
using Finance.Models;

namespace Finance
{
    public partial class ProjectWindow : Window
    {
        public ProjectViewModel ViewModel { get; set; }

        public ProjectWindow()
        {
            InitializeComponent();
        }

        public ProjectWindow(ProjectViewModel viewModel) : this()
        {
            ViewModel = viewModel;
            DataContext = viewModel;
        }

        private void YearComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ViewModel != null && YearComboBox.SelectedItem != null)
            {
                string selectedYearString = YearComboBox.SelectedItem.ToString();
                
                if (selectedYearString == "Toutes" || string.IsNullOrEmpty(selectedYearString))
                {
                    ViewModel.SelectedYear = null;
                }
                else if (int.TryParse(selectedYearString, out int year))
                {
                    ViewModel.SelectedYear = year;
                }
            }
        }

        private void AddTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Assurez-vous que le ViewModel est defini a partir du DataContext si necessaire
                if (ViewModel == null && DataContext is ProjectViewModel viewModel)
                {
                    ViewModel = viewModel;
                }

                if (ViewModel == null)
                {
                    MessageBox.Show("Le ViewModel est null. Assurez-vous que le DataContext est correctement defini.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (ViewModel.CurrentProject == null)
                {
                    MessageBox.Show("Aucun projet n'est selectionne.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var addTransactionWindow = new AddTransactionWindow();
                var result = addTransactionWindow.ShowDialog();

                if (result == true && addTransactionWindow.Transaction != null)
                {
                    var newTransaction = addTransactionWindow.Transaction;
                    ViewModel.CurrentProject.Transactions.Add(newTransaction);
                    MessageBox.Show("Transaction ajoutee avec succes !", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("La fenetre d'ajout de transaction a ete fermee sans sauvegarde.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            // Recuperer la transaction selectionnee (exemple)
            // Vous devrez adapter cette methode selon votre interface utilisateur
            if (sender is FrameworkElement element && element.DataContext is Transaction selectedTransaction)
            {
                if (ViewModel?.EditTransactionCommand?.CanExecute(selectedTransaction) == true)
                {
                    ViewModel.EditTransactionCommand.Execute(selectedTransaction);
                }
            }
        }

        private void DeleteTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            // Recuperer la transaction selectionnee (exemple)
            // Vous devrez adapter cette methode selon votre interface utilisateur
            if (sender is FrameworkElement element && element.DataContext is Transaction selectedTransaction)
            {
                if (ViewModel?.DeleteTransactionCommand?.CanExecute(selectedTransaction) == true)
                {
                    ViewModel.DeleteTransactionCommand.Execute(selectedTransaction);
                }
            }
        }
    }

    // Convertisseur pour l'annee
    public class YearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "Toutes";
            
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.ToString() == "Toutes")
                return null;
            
            if (int.TryParse(value.ToString(), out int year))
                return year;
            
            return null;
        }
    }

    // Convertisseur pour le type de transaction
    public class TransactionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isIncome)
            {
                return isIncome ? "Revenu" : "Depense";
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}