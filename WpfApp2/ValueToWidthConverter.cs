using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp2
{
    public class ValueToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double sliderValue)
            {
                // Slider Width = 200, Margin = 10+10, effective width = 180
                double maxWidth = 180; // 200 - 10 - 10
                return (sliderValue / 100.0) * maxWidth;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}