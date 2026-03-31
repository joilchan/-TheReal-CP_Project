using System.Globalization;
using Microsoft.Maui.Graphics;

namespace BroShopApp.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string status = value?.ToString();
        bool isBackground = parameter?.ToString() == "BG"; // Параметр для определения: фон или текст

        Color color = status switch
        {
            "В обработке" => Colors.Orange,
            "Отправлен" => Colors.DeepSkyBlue,
            "Доставлен" => Colors.LimeGreen,
            "Отменен" => Colors.Red,
            _ => Colors.Gray
        };

        // Если нужен цвет фона, делаем его полупрозрачным
        return isBackground ? color.WithAlpha(0.2f) : color;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}