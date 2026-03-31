using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BroShopApp.Converters
{
    public class StatusToCancelVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                // Убираем пробелы и переводим в нижний регистр для надежности
                return status.Trim().Equals("в обработке", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
