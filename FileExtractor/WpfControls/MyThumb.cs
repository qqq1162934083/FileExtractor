using FileExtractor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileExtractor.WpfControls
{
    public class MyThumb : Thumb
    {
        static MyThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyThumb), new FrameworkPropertyMetadata(typeof(MyThumb)));
        }
        private Grid BackgroundPanel { get; set; }
        private Brush NormalBrush { get; set; }
        private Brush FocusBrush { get; set; }
        private Brush PressedBrush { get; set; }
        public MyThumb() : base()
        {
            //Style = ResDicUtils.GetCustomControlStyle<MyThumb>();
            var brushConverter = new BrushConverter();
            NormalBrush = (Brush)brushConverter.ConvertFrom("#686868");
            FocusBrush = (Brush)brushConverter.ConvertFrom("#9e9e9e");
            PressedBrush = (Brush)brushConverter.ConvertFrom("#efebef");
            MouseEnter += (s, e) => BackgroundPanel.Background = FocusBrush;
            MouseLeave += (s, e) => BackgroundPanel.Background = NormalBrush;
            DragStarted += (s, e) => BackgroundPanel.Background = PressedBrush;
            DragCompleted += (s, e) => BackgroundPanel.Background = IsMouseOver ? FocusBrush : NormalBrush;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BackgroundPanel = (Grid)Template.FindName("PART_grid", this);
        }
    }
}
