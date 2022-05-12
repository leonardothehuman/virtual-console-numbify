using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Virtual_Console_Numbify_fw.Converters {
    public class BooleanToBackgroundColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool vvalue = (bool)value;
            if (vvalue == true) {
                return new SolidColorBrush(Colors.Yellow);
            } else {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}
