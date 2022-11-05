using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileExtractor.WpfControls.Converters
{
    public class TabControlListBoxHasSelectionBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var visibleIfTrue = true;
            //if (parameter != null) visibleIfTrue = bool.Parse(parameter.ToString());
            //var visible = (bool)value;
            //visible = visibleIfTrue ? visible : !visible;
            //return visible ? Visibility.Visible : Visibility.Hidden;
            Console.WriteLine();
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
