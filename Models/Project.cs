using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System;
using System.Linq;

namespace Finance.Models
{
    public class Project : INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<Transaction> _transactions = new ObservableCollection<Transaction>();

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set
            {
                if (_transactions != value)
                {
                    // Se desabonner de l'ancienne collection
                    if (_transactions != null)
                    {
                        _transactions.CollectionChanged -= Transactions_CollectionChanged;
                        foreach (var transaction in _transactions)
                        {
                            transaction.PropertyChanged -= Transaction_PropertyChanged;
                        }
                    }

                    _transactions = value;

                    // S'abonner a la nouvelle collection
                    if (_transactions != null)
                    {
                        _transactions.CollectionChanged += Transactions_CollectionChanged;
                        foreach (var transaction in _transactions)
                        {
                            transaction.PropertyChanged += Transaction_PropertyChanged;
                        }
                    }

                    OnPropertyChanged(nameof(Transactions));
                    UpdateTotals();
                }
            }
        }

        public decimal TotalIncome => CalculateTotal(true);
        public decimal TotalExpenses => CalculateTotal(false);
        public decimal Total => TotalIncome - TotalExpenses;

        public Project()
        {
            _transactions.CollectionChanged += Transactions_CollectionChanged;
        }

        private void Transactions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // S'abonner aux nouvelles transactions
            if (e.NewItems != null)
            {
                foreach (Transaction transaction in e.NewItems)
                {
                    transaction.PropertyChanged += Transaction_PropertyChanged;
                }
            }

            // Se desabonner des anciennes transactions
            if (e.OldItems != null)
            {
                foreach (Transaction transaction in e.OldItems)
                {
                    transaction.PropertyChanged -= Transaction_PropertyChanged;
                }
            }

            UpdateTotals();
        }

        private void Transaction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Mettre a jour les totaux quand une propriete d'une transaction change
            if (e.PropertyName == nameof(Transaction.Amount) ||
                e.PropertyName == nameof(Transaction.IsIncome) ||
                e.PropertyName == nameof(Transaction.Currency))
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

        private decimal CalculateTotal(bool isIncome)
        {
            decimal total = 0;
            var preferredCurrency = App.SettingsViewModel?.PreferredCurrency ?? "ILS";
            
            foreach (var transaction in Transactions)
            {
                if (transaction.IsIncome == isIncome)
                {
                    try
                    {
                        var convertedAmount = App.CurrencyConverter.Convert(
                            transaction.Amount,
                            transaction.Currency,
                            preferredCurrency,
                            transaction.Date
                        );
                        total += convertedAmount;
                    }
                    catch
                    {
                        // Si la conversion echoue, utiliser le montant original
                        total += transaction.Amount;
                    }
                }
            }
            return total;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}