using MyTool;
using MyTool.Modules.Module_FileExtractor;
using NetCore5WpfToolsApp.Utils.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    /// LaunchPage.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchPage : Page, IConfigControlCache
    {
        public ViewCacheMgr<LaunchPage, object, object> CacheMgr { get; set; }
        public LaunchPage()
        {
            CacheMgr = new ViewCacheMgr<LaunchPage, object, object>(this);
            Loaded += (s, e) => CacheMgr.NotifyLoad();
            Unloaded += (s, e) => CacheMgr.NotifySave();
            InitializeComponent();
        }

        private void btn_openConfig_Click(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectOpenFile(x => x.Filter = "配置文件|*.cfg;*.json", x =>
            {
                var data = JsonConvert.DeserializeObject<FileExtractorDataCache>(File.ReadAllText(x.FileName));
                Console.WriteLine();
            });
        }

        private void btn_createConfig_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("pack://application:,,,/Pages/NewConfigPage.xaml"));
        }

        public void LoadViewCache()
        {
            Console.WriteLine();
        }

        public void ApplyViewCache()
        {
            Console.WriteLine();
        }

        public void LoadDataCache()
        {
            Console.WriteLine();
        }

        public void ApplyDataCache()
        {
            Console.WriteLine();
        }
    }
    public class ScrollBarVisibilityConverter : IValueConverter
    {
        //public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        //{
        //    var visibility = (ScrollBarVisibility)values.First(x => x is ScrollBarVisibility);
        //    var isMouseOver = (bool)values.First(x => x is bool);

        //    if (isMouseOver || visibility == ScrollBarVisibility.Visible) return Visibility.Visible;
        //    else return visibility;
        //}

        //public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isMouseOver = (bool)value;
            return isMouseOver ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
