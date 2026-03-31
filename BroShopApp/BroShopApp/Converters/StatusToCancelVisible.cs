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
            // Отменить можно только тот заказ, который еще не уехал в доставку
            return value?.ToString() == "В обработке";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
