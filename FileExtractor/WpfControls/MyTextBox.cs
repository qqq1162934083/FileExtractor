using FileExtractor.Utils;
using System;
using System.Collections.Generic;
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

namespace FileExtractor.WpfControls
{
    public class MyTextBox : TextBox
    {
        static MyTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyTextBox), new FrameworkPropertyMetadata(typeof(MyTextBox)));
        }

        public string HintText
        {
            get { return (string)GetValue(HintTextProperty); }
            set { SetValue(HintTextProperty, value); }
        }
        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register("HintText", typeof(string), typeof(MyTextBox), new PropertyMetadata(string.Empty));



        public Visibility HintVisibility
        {
            get { return (Visibility)GetValue(HintVisibilityProperty); }
            set { SetValue(HintVisibilityProperty, value); }
        }
        public static readonly DependencyProperty HintVisibilityProperty =
            DependencyProperty.Register("HintVisibility", typeof(Visibility), typeof(MyTextBox), new PropertyMetadata(Visibility.Collapsed));

        public MyTextBox() : base()
        {
            //Style = ResDicUtils.GetCustomControlStyle<MyTextBox>();
        }
    }
}
