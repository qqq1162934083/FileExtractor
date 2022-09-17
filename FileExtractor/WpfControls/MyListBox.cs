using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileExtractor.Utils;

namespace FileExtractor.WpfControls
{
    public partial class MyListBox : ListBox
    {
        static MyListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyListBox), new FrameworkPropertyMetadata(typeof(MyListBox)));
        }

        public MyListBox() : base()
        {
            Resources = ResDicUtils.GetCustomControlDefaultResourceDictionary<MyListBox>();
            Style = ResDicUtils.GetCustomControlStyle<MyListBox>();
        }
    }
    public class ScrollBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isMouseOver = (bool)value;
            return isMouseOver ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
