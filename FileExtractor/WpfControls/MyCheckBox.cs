using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileExtractor.WpfControls
{
    public class MyCheckBox : CheckBox
    {
        static MyCheckBox()
        {
            var type = typeof(MyCheckBox);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
        }



        public Brush TextForeground
        {
            get { return (Brush)GetValue(TextForegroundProperty); }
            set { SetValue(TextForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextForegroundProperty =
            DependencyProperty.Register("TextForeground", typeof(Brush), typeof(MyCheckBox), new PropertyMetadata((Brush)new BrushConverter().ConvertFrom("White")));



        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(double), typeof(MyCheckBox), new PropertyMetadata(12d));


    }
}
