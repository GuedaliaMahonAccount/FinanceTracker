using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Finance.Models;
using Finance.Utils;
using Finance.Views;
using Microsoft.Win32;
using System.Windows.Forms; // Correct

namespace Finance.ViewModels
{
    public class ProjectViewModel : INotifyPropertyChanged
    {
        private Project _currentProject;
        private string _selectedTransactionType = "Tous";
        private int? _selectedYear;
        private string _selectedLabel = "Tous";

        public Project CurrentProject
        {
            get => _currentProject;
            set
            {
                if (_currentProject != value)
                {
                    // Se desabonner de l'ancien projet
                    if (_currentProject != null)
                    {
                        _currentProject.PropertyChanged -= CurrentProject_PropertyChanged;
                        _currentProject.Transactions.CollectionChanged -= Transactions_CollectionChanged;
                    }

                    _currentProject = value;

                    // S'abonner au nouveau projet
                    if (_currentProject != null)
                    {
                        _currentProject.PropertyChanged += CurrentProject_PropertyChanged;
                        _currentProject.Transactions.CollectionChanged += Transactions_CollectionChanged;
                    }

                    OnPropertyChanged(nameof(CurrentProject));
                    UpdateAvailableFilters();
                    ApplyFilters();
                    UpdateTotals();
                }
            }
        }

        public ObservableCollection<Transaction> FilteredTransactions { get; set; } = new ObservableCollection<Transaction>();

        // Proprietes pour les filtres
        public string SelectedTransactionType
        {
            get => _selectedTransactionType;
            set
            {
                if (_selectedTransactionType != value)
                {
                    _selectedTransactionType = value;
                    OnPropertyChanged(nameof(SelectedTransactionType));
                    ApplyFilters();
                }
            }
        }

        public int? SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (_selectedYear != value)
                {
                    _selectedYear = value;
                    OnPropertyChanged(nameof(SelectedYear));
                    ApplyFilters();
                }
            }
        }

        public string SelectedLabel
        {
            get => _selectedLabel;
            set
            {
                if (_selectedLabel != value)
                {
                    _selectedLabel = value;
                    OnPropertyChanged(nameof(SelectedLabel));
                    ApplyFilters();
                }
            }
        }

        // Collections pour les ComboBox
        public ObservableCollection<string> TransactionTypes { get; set; } = new ObservableCollection<string>
        {
            "Tous",
            "Revenus",
            "Depenses"
        };

        public ObservableCollection<string> AvailableYears { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableLabels { get; set; } = new ObservableCollection<string>();

        public decimal TotalIncome => CurrentProject != null ? CalculateConvertedTotal(true) : 0;
        public decimal TotalExpenses => CurrentProject != null ? CalculateConvertedTotal(false) : 0;
        public decimal Total => TotalIncome - TotalExpenses;

        public string PreferredCurrency { get; set; } = "ILS";
        private CurrencyConverter _currencyConverter;

        public ICommand DownloadFilesCommand { get; }
        public ICommand DeleteTransactionCommand { get; }
        public ICommand EditTransactionCommand { get; }
        public ICommand ResetFiltersCommand { get; }

        public ProjectViewModel(CurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
            DownloadFilesCommand = new RelayCommand<Transaction>(DownloadFiles);
            DeleteTransactionCommand = new RelayCommand<Transaction>(DeleteTransaction, CanDeleteTransaction);
            EditTransactionCommand = new RelayCommand<Transaction>(EditTransaction, CanEditTransaction);
            ResetFiltersCommand = new RelayCommand(ResetFilters);
        }

        private void Transactions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateAvailableFilters();
            ApplyFilters();
        }

        private void UpdateAvailableFilters()
        {
            if (CurrentProject == null || CurrentProject.Transactions == null)
                return;

            // Mettre a jour les annees disponibles
            var years = CurrentProject.Transactions
                .Select(t => t.Date.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .Select(y => y.ToString())
                .ToList();

            AvailableYears.Clear();
            AvailableYears.Add("Toutes");
            foreach (var year in years)
            {
                AvailableYears.Add(year);
            }

            // Mettre a jour les labels disponibles
            var labels = CurrentProject.Transactions
                .Where(t => !string.IsNullOrWhiteSpace(t.Label))
                .Select(t => t.Label)
                .Distinct()
                .OrderBy(l => l)
                .ToList();

            AvailableLabels.Clear();
            AvailableLabels.Add("Tous");
            foreach (var label in labels)
            {
                AvailableLabels.Add(label);
            }

            OnPropertyChanged(nameof(AvailableYears));
            OnPropertyChanged(nameof(AvailableLabels));
        }

        private void ApplyFilters()
        {
            FilteredTransactions.Clear();

            if (CurrentProject == null || CurrentProject.Transactions == null)
                return;

            var filteredList = CurrentProject.Transactions.AsEnumerable();

            // Filtrer par type de transaction
            if (SelectedTransactionType == "Revenus")
            {
                filteredList = filteredList.Where(t => t.IsIncome);
            }
            else if (SelectedTransactionType == "Depenses")
            {
                filteredList = filteredList.Where(t => !t.IsIncome);
            }

            // Filtrer par annee
            if (SelectedYear.HasValue)
            {
                filteredList = filteredList.Where(t => t.Date.Year == SelectedYear.Value);
            }

            // Filtrer par label
            if (!string.IsNullOrEmpty(SelectedLabel) && SelectedLabel != "Tous")
            {
                filteredList = filteredList.Where(t => t.Label == SelectedLabel);
            }

            foreach (var transaction in filteredList)
            {
                FilteredTransactions.Add(transaction);
            }

            OnPropertyChanged(nameof(FilteredTransactions));
        }

        private void ResetFilters()
        {
            SelectedTransactionType = "Tous";
            SelectedYear = null;
            SelectedLabel = "Tous";
        }

        private void CurrentProject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Mettre a jour les totaux convertis quand le projet change
            if (e.PropertyName == nameof(Project.TotalIncome) ||
                e.PropertyName == nameof(Project.TotalExpenses) ||
                e.PropertyName == nameof(Project.Total))
            {
                UpdateTotals();
            }
        }

        private void UpdateTotals()
        {
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpenses));
            OnPropertyChanged(nameof(Total));
        }

        private decimal CalculateConvertedTotal(bool isIncome)
        {
            if (CurrentProject == null || CurrentProject.Transactions == null)
                return 0;

            decimal total = 0;
            foreach (var transaction in CurrentProject.Transactions)
            {
                if (transaction.IsIncome == isIncome)
                {
                    // Convertir depuis la devise reelle de la transaction vers la devise preferee
                    decimal convertedAmount = _currencyConverter.Convert(
                        transaction.Amount, 
                        transaction.Currency, 
                        PreferredCurrency, 
                        transaction.Date);
                    total += convertedAmount;
                }
            }
            return total;
        }

        private void DownloadFiles(Transaction transaction)
        {
            if (transaction?.FilePaths == null || transaction.FilePaths.Count == 0)
            {
                System.Windows.MessageBox.Show("Aucun fichier a telecharger pour cette transaction.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Selectionnez un dossier pour telecharger les fichiers";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string destinationFolder = dialog.SelectedPath;
                    int successCount = 0;
                    int errorCount = 0;

                    foreach (var filePath in transaction.FilePaths)
                    {
                        try
                        {
                            // Verifier si le fichier existe dans le stockage interne
                            if (!File.Exists(filePath))
                            {
                                System.Windows.MessageBox.Show($"Le fichier {Path.GetFileName(filePath)} n'existe plus dans le stockage interne.", 
                                                              "Fichier introuvable", 
                                                              MessageBoxButton.OK, 
                                                              MessageBoxImage.Warning);
                                errorCount++;
                                continue;
                            }

                            string fileName = Path.GetFileName(filePath);
                            string destinationPath = Path.Combine(destinationFolder, fileName);

                            // Generer un nom unique si le fichier existe deja dans la destination
                            int counter = 1;
                            while (File.Exists(destinationPath))
                            {
                                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                                string extension = Path.GetExtension(fileName);
                                string newFileName = $"{fileNameWithoutExtension}_{counter}{extension}";
                                destinationPath = Path.Combine(destinationFolder, newFileName);
                                counter++;
                            }

                            File.Copy(filePath, destinationPath, false);
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            System.Windows.MessageBox.Show($"Erreur lors du telechargement du fichier {Path.GetFileName(filePath)}: {ex.Message}", 
                                                          "Erreur", 
                                                          MessageBoxButton.OK, 
                                                          MessageBoxImage.Error);
                        }
                    }

                    if (successCount > 0)
                    {
                        System.Windows.MessageBox.Show($"{successCount} fichier(s) telecharge(s) avec succes.", 
                                                      "Succes", 
                                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private bool CanDeleteTransaction(Transaction transaction)
        {
            return transaction != null && CurrentProject != null;
        }

        private void DeleteTransaction(Transaction transaction)
        {
            if (transaction == null || CurrentProject == null)
                return;

            var result = System.Windows.MessageBox.Show(
                $"Etes-vous sur de vouloir supprimer la transaction '{transaction.Name}' ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Supprimer les fichiers associes du stockage interne
                    if (transaction.FilePaths != null && transaction.FilePaths.Count > 0)
                    {
                        foreach (var filePath in transaction.FilePaths)
                        {
                            try
                            {
                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Logger l'erreur mais continuer la suppression de la transaction
                                System.Diagnostics.Debug.WriteLine($"Erreur lors de la suppression du fichier {filePath}: {ex.Message}");
                            }
                        }
                    }

                    // Supprimer la transaction de la collection
                    CurrentProject.Transactions.Remove(transaction);

                    // Mettre a jour les transactions filtrees si necessaire
                    if (FilteredTransactions.Contains(transaction))
                    {
                        FilteredTransactions.Remove(transaction);
                    }

                    System.Windows.MessageBox.Show(
                        "Transaction supprimee avec succes.",
                        "Succes",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        $"Erreur lors de la suppression de la transaction: {ex.Message}",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private bool CanEditTransaction(Transaction transaction)
        {
            return transaction != null && CurrentProject != null;
        }

        private void EditTransaction(Transaction transaction)
        {
            if (transaction == null || CurrentProject == null)
                return;

            try
            {
                var editWindow = new AddTransactionWindow();
                
                // Pre-remplir la fenetre avec les donnees existantes
                editWindow.Title = "Modifier la transaction";
                editWindow.NameTextBox.Text = transaction.Name;
                editWindow.DescriptionTextBox.Text = transaction.Description;
                editWindow.AmountTextBox.Text = transaction.Amount.ToString();
                
                // Sélectionner la devise correcte dans le ComboBox en utilisant le code ISO
                switch (transaction.Currency)
                {
                    case "ILS":
                        editWindow.CurrencyComboBox.SelectedIndex = 0; // ₪ Shekel
                        break;
                    case "USD":
                        editWindow.CurrencyComboBox.SelectedIndex = 1; // $ Dollar
                        break;
                    case "EUR":
                        editWindow.CurrencyComboBox.SelectedIndex = 2; // € Euro
                        break;
                    default:
                        editWindow.CurrencyComboBox.SelectedIndex = 0; // Par défaut ILS
                        break;
                }
                
                editWindow.DatePicker.SelectedDate = transaction.Date;
                editWindow.LabelTextBox.Text = transaction.Label;
                
                if (transaction.IsIncome)
                {
                    editWindow.IncomeRadioButton.IsChecked = true;
                }
                else
                {
                    editWindow.ExpenseRadioButton.IsChecked = true;
                }

                // Copier les fichiers existants
                if (transaction.FilePaths != null)
                {
                    editWindow.SelectedFiles = new List<string>(transaction.FilePaths);
                }

                var result = editWindow.ShowDialog();

                if (result == true && editWindow.Transaction != null)
                {
                    // Sauvegarder les anciens fichiers pour suppression eventuelle
                    var oldFilePaths = new List<string>(transaction.FilePaths ?? new List<string>());

                    // Mettre a jour les proprietes de la transaction
                    // Les setters vont automatiquement notifier les changements
                    transaction.Name = editWindow.Transaction.Name;
                    transaction.Description = editWindow.Transaction.Description;
                    transaction.IsIncome = editWindow.Transaction.IsIncome;
                    transaction.Amount = editWindow.Transaction.Amount;
                    transaction.Currency = editWindow.Transaction.Currency;
                    transaction.Date = editWindow.Transaction.Date;
                    transaction.Label = editWindow.Transaction.Label;
                    transaction.FilePaths = editWindow.Transaction.FilePaths;

                    // Supprimer les anciens fichiers qui ne sont plus utilises
                    foreach (var oldFilePath in oldFilePaths)
                    {
                        if (!transaction.FilePaths.Contains(oldFilePath))
                        {
                            try
                            {
                                if (File.Exists(oldFilePath))
                                {
                                    File.Delete(oldFilePath);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Erreur lors de la suppression de l'ancien fichier {oldFilePath}: {ex.Message}");
                            }
                        }
                    }

                    System.Windows.MessageBox.Show(
                        "Transaction modifiee avec succes.",
                        "Succes",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Erreur lors de la modification de la transaction: {ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void FilterTransactions(string filterCriteria)
        {
            // Cette methode est maintenant geree par ApplyFilters()
            ApplyFilters();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}