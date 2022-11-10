using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace FileExtractor.WpfControls
{
    public class MyMenuItem : MenuItem
    {
        static MyMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyMenuItem), new FrameworkPropertyMetadata(typeof(MyMenuItem)));
        }
        public MyMenuItem()
        {
        }

        /// <summary>
        /// 文字内容
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(MyMenuItem), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 文字对齐方式
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(nameof(TextAlignment), typeof(TextAlignment), typeof(MyMenuItem), new PropertyMetadata(TextAlignment.Left));

        /// <summary>
        /// 子菜单弹出位置
        /// </summary>
        public PlacementMode SubmenuPlacement
        {
            get { return (PlacementMode)GetValue(SubmenuPlacementProperty); }
            set { SetValue(SubmenuPlacementProperty, value); }
        }

        public static readonly DependencyProperty SubmenuPlacementProperty =
            DependencyProperty.Register(nameof(SubmenuPlacement), typeof(PlacementMode), typeof(MyMenuItem), new PropertyMetadata(PlacementMode.Right));
    }
}
