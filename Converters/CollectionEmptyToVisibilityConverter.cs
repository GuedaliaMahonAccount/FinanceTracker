using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Finance.Converters
{
    /// <summary>
    /// Convertisseur pour afficher un message seulement si une collection est vide
    /// </summary>
    public class CollectionEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection collection)
            {
                // Si la collection est vide, afficher le message (Visible)
                // Sinon, masquer le message (Collapsed)
                return collection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            
            // Par d?faut, masquer
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
