using AutoClicker.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AutoClicker.Converters
{
    /// <summary>
    /// Convert statut flag into a string
    /// </summary>
    public class StatutConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if value parameter is an uint
            if (value.GetType() != typeof(uint))
                return string.Empty;

            if ((uint)value == 2)
                return Application.Current.MainWindow.FindResource("State_Error");
            else if ((uint)value == 0)
                return Application.Current.MainWindow.FindResource("State_Running");
            else
                return Application.Current.MainWindow.FindResource("State_Waiting");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
