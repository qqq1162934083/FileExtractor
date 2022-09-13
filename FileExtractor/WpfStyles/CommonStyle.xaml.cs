using FileExtractor.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

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

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Grid || e.OriginalSource is Border || e.OriginalSource is Window)
            {
                WindowInteropHelper wih = new WindowInteropHelper((Window)sender);
                Win32.SendMessage(wih.Handle, Win32.WM_NCLBUTTONDOWN, (int)Win32.HitTest.HTCAPTION, 0);
                return;
            }
        }
    }
}
