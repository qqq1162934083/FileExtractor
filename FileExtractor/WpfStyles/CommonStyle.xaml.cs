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
            var window = GetWindowFromSender(sender);
            window.Close();
        }

        private void btn_maximizeNormalizeWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = GetWindowFromSender(sender);
            if (window.WindowState != WindowState.Minimized)
            {
                if (window.WindowState != WindowState.Maximized)
                    window.WindowState = WindowState.Maximized;
                else
                    window.WindowState = WindowState.Normal;
            }
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = GetWindowFromSender(sender);
            WindowInteropHelper wih = new WindowInteropHelper(window);
            Win32.SendMessage(wih.Handle, Win32.WM_NCLBUTTONDOWN, (int)Win32.HitTest.HTCAPTION, 0);
        }

        private void btn_minimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = GetWindowFromSender(sender);
            window.WindowState = WindowState.Minimized;
        }

        private static Window GetWindowFromSender(object sender)
        {
            return (Window)(sender is Window ? sender : ((FrameworkElement)sender).TemplatedParent);
        }

        private void grid_title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = GetWindowFromSender(sender);
            if (e.ClickCount == 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                btn_maximizeNormalizeWindow_Click(sender, null);
            }
            else
            {
                MainWindow_MouseLeftButtonDown(sender, e);
            }
        }
    }
}
