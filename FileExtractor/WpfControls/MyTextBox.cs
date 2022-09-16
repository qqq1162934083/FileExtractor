using FileExtractor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace FileExtractor.WpfControls
{
    /// <summary>
    /// MyTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class MyTextBox : TextBox
    {
        public static readonly DependencyProperty HintTextProperty = RegisterDependencyProperties<MyTextBox, string>(nameof(HintText), string.Empty);
        public string HintText
        {
            get { return (string)GetValue(HintTextProperty); }
            set { SetValue(HintTextProperty, value); }
        }

        public static readonly DependencyProperty HintVisibilityProperty = RegisterDependencyProperties<MyTextBox, Visibility>(nameof(HintVisibility), Visibility.Collapsed);
        public Visibility HintVisibility
        {
            get { return (Visibility)GetValue(HintVisibilityProperty); }
            set { SetValue(HintVisibilityProperty, value); }
        }

        private static DependencyProperty RegisterDependencyProperties<TElement, TProperty>(string name, TProperty defaultValue)
            where TElement : DependencyObject
        {
            return DependencyProperty.Register(name, typeof(TProperty), typeof(TElement), new PropertyMetadata(defaultValue));
        }

        public MyTextBox() : base()
        {
            Style = ResDicUtils.GetCustomControlStyle<MyTextBox>();
        }
    }
}
