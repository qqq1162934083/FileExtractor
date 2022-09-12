using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExtractor.WpfStyles
{
    public partial class CommonStyle
    {
        private void btn_closeWindow_Click(object sender, RoutedEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            win.Close();
        }

        private void btn_maximizeNormalizeWindow_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
