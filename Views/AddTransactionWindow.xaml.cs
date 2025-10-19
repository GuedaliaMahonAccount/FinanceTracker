using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Finance.Models;
using Finance.Utils;

namespace Finance.Views
{
    public partial class AddTransactionWindow : Window
    {
        public Transaction Transaction { get; private set; }
        public List<string> SelectedFiles { get; set; } = new List<string>();

        public AddTransactionWindow()
        {
            InitializeComponent();
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permettre uniquement les chiffres, la virgule et le point
            Regex regex = new Regex(@"^[0-9.,]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void SelectIncome(object sender, MouseButtonEventArgs e)
        {
            IncomeRadioButton.IsChecked = true;
        }

        private void SelectExpense(object sender, MouseButtonEventArgs e)
        {
            ExpenseRadioButton.IsChecked = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Parser le montant en supportant ? la fois virgule et point
            string amountText = AmountTextBox.Text.Trim().Replace(",", ".");
            
            // Obtenir le code ISO de la devise depuis le Tag du ComboBoxItem s?lectionn?
            string currencyCode = CurrencyComboBox.SelectedValue?.ToString() ?? "ILS";
            
            Transaction = new Transaction
            {
                Name = NameTextBox.Text,
                Description = DescriptionTextBox.Text,
                IsIncome = IncomeRadioButton.IsChecked == true,
                Amount = decimal.TryParse(amountText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var amount) ? amount : 0,
                Currency = currencyCode,
                Date = DatePicker.SelectedDate ?? DateTime.Now,
                Label = LabelTextBox.Text,
                FilePaths = new List<string>(SelectedFiles)
            };

            DialogResult = true;
        }

        private void AddFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "All Files|*.*|Images|*.jpg;*.jpeg;*.png|PDF Files|*.pdf"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                int successCount = 0;
                int errorCount = 0;

                foreach (var filePath in openFileDialog.FileNames)
                {
                    try
                    {
                        // Copier le fichier dans le dossier interne de l'application
                        string internalPath = FileHandler.CopyFileToInternalStorage(filePath);
                        SelectedFiles.Add(internalPath);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        MessageBox.Show($"Erreur lors de la copie du fichier {filePath}: {ex.Message}", 
                                        "Erreur", 
                                        MessageBoxButton.OK, 
                                        MessageBoxImage.Error);
                    }
                }

                if (successCount > 0)
                {
                    MessageBox.Show($"{successCount} fichier(s) ajoute(s) avec succes.", 
                                    "Fichiers ajoutes", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Information);
                }
            }
        }
    }
}