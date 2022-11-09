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
    public class MyButton : Button
    {
        static MyButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyButton), new FrameworkPropertyMetadata(typeof(MyButton)));
        }
        public MyButton()
        {
            IsEnabledChanged += MyButton_IsEnabledChanged;
            _brushConverter = new BrushConverter();
        }

        protected BrushConverter _brushConverter;
        protected Brush _textForegroundBeforeEnabledFalse;//禁用前字体颜色
        protected bool _hasSetTextForegroundBehindEnbaledFalse = false;//是否在禁用后重新设置了颜色，如果设置了颜色，启用时不会变回原色
        private void MyButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
            {
                if (!_hasSetTextForegroundBehindEnbaledFalse) TextForeground = _textForegroundBeforeEnabledFalse;
            }
            else
            {
                _textForegroundBeforeEnabledFalse = TextForeground;
                TextForeground = (Brush)_brushConverter.ConvertFromString("Gray");
                _hasSetTextForegroundBehindEnbaledFalse = false;
            }
        }

        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }
        public Brush TextForeground
        {
            get { return (Brush)GetValue(TextForegroundProperty); }
            set
            {
                SetValue(TextForegroundProperty, value);
                _hasSetTextForegroundBehindEnbaledFalse = true;
            }
        }

        public void test()
        {

        }


        // Using a DependencyProperty as the backing store for TextFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register(nameof(TextFontSize), typeof(double), typeof(MyButton), new PropertyMetadata(12d));
        public static readonly DependencyProperty TextForegroundProperty =
            DependencyProperty.Register(nameof(TextForeground), typeof(Brush), typeof(MyButton), new PropertyMetadata(new BrushConverter().ConvertFromString("White")));
    }
}
