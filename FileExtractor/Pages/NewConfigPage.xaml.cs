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

namespace FileExtractor.Pages
{
    /// <summary>
    /// NewConfigPage.xaml 的交互逻辑
    /// </summary>
    public partial class NewConfigPage : Page
    {
        public NewConfigPage()
        {
            InitializeComponent();
        }

        private void btn_previoutStep_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("pack://application:,,,/Pages/LaunchPage.xaml"));
        }
    }
}
