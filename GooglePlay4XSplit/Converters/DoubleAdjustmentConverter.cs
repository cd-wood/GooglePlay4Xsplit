using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GooglePlay4XSplit.Converters
{
    class DoubleAdjustmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var v = (Double)value;
            Double a = Double.Parse(parameter as String);

            return v + a;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var v = (Double)value;
            Double a = Double.Parse(parameter as String);

            return v - a;
        }
    }
}
