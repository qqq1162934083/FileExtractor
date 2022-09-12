using FileExtractor.Libs;
using MyTool.Modules.Module_FileExtractor;
using NetCore5WpfToolsApp.Utils.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileExtractor
{
    /// <summary>
    /// LaunchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchWindow : Window
    {
        public LaunchWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
        }

        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Grid || e.OriginalSource is Border || e.OriginalSource is Window)
            {
                WindowInteropHelper wih = new WindowInteropHelper(this);
                Win32.SendMessage(wih.Handle, Win32.WM_NCLBUTTONDOWN, (int)Win32.HitTest.HTCAPTION, 0);
                return;
            }
        }

        private void btn_openConfig_Click(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectOpenFile(x=>x.Filter= "配置文件|*.cfg;*.json", x=>
            {
                var data = JsonConvert.DeserializeObject<FileExtractorDataCache>(File.ReadAllText(x.FileName));
                Console.WriteLine();
            });
        }

        private void btn_createConfig_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
