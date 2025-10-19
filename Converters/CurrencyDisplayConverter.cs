using System;
using System.Globalization;
using System.Windows.Data;

namespace Finance.Converters
{
    public class CurrencyDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount)
            {
                var currency = App.SettingsViewModel?.PreferredCurrency ?? "ILS";
                var symbol = GetCurrencySymbol(currency);
                return $"{amount:N2} {symbol}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetCurrencySymbol(string currency)
        {
            switch (currency)
            {
                case "ILS":
                case "Shekel":
                    return "¤";
                case "USD":
                case "Dollars":
                    return "$";
                case "EUR":
                case "Euro":
                    return "€";
                default:
                    return "¤";
            }
        }
    }
}
