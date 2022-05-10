using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Virtual_Console_Numbify_fw {
    public class BooleanToForegroundColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool vvalue = (bool)value;
            if (vvalue == true) {
                return new SolidColorBrush(Colors.Red);
            } else {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }
    }
}
