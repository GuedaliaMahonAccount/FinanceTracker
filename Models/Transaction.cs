using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Finance.Models
{
    public class Transaction : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private bool _isIncome;
        private decimal _amount;
        private string _currency;
        private DateTime _date;
        private string _label;
        private List<string> _filePaths = new List<string>();

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

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public bool IsIncome
        {
            get => _isIncome;
            set
            {
                if (_isIncome != value)
                {
                    _isIncome = value;
                    OnPropertyChanged(nameof(IsIncome));
                }
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public string Currency
        {
            get => _currency;
            set
            {
                if (_currency != value)
                {
                    _currency = value;
                    OnPropertyChanged(nameof(Currency));
                }
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        public string Label
        {
            get => _label;
            set
            {
                if (_label != value)
                {
                    _label = value;
                    OnPropertyChanged(nameof(Label));
                }
            }
        }

        public List<string> FilePaths
        {
            get => _filePaths;
            set
            {
                if (_filePaths != value)
                {
                    _filePaths = value;
                    OnPropertyChanged(nameof(FilePaths));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}