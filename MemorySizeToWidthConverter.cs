using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace pr4
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class MemorySizeToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = System.Convert.ToDouble(value); // Размер области памяти
            double total = System.Convert.ToDouble(parameter); // Общий объем памяти
            return size / total * 700; // Пропорциональная ширина
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Не требуется обратное преобразование
        }
    }



}
