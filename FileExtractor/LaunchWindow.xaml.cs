using FileExtractor.Libs;
using FileExtractor.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileExtractor
{
    /// <summary>
    /// LaunchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchWindow : NavigationWindow
    {
        public LaunchWindow()
        {
            InitializeComponent();
        }

        public void Jump2WorkWindow(WorkData data)
        {
            Hide();
            var mainWindow = new WorkWindow(data);
            mainWindow.Show();
            Close();
        }
    }
}
