using FileExtractor.Models;
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
using System.Windows.Controls.Primitives;
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
        public LaunchPage()
        {
            App.Cache.StartWorkCacheMgr = new ViewCacheMgr<LaunchPage, StartWorkCache, object>(this);Loaded += (s, e) => App.Cache.StartWorkCacheMgr.NotifyLoad();
            InitializeComponent();
        }

        private void btn_openConfig_Click(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectOpenFile(x => x.Filter = "配置文件|*.cfg;*.json", x =>
            {
                var fileInfo = new FileInfo(x.FileName);
                FileExtractorDataCache data = null;
                try
                {
                    data = JsonConvert.DeserializeObject<FileExtractorDataCache>(File.ReadAllText(x.FileName));
                }
                catch(Exception exp)
                {
                    MessageBox.Show("打开文件失败，无法识别文件内容");
                    return;
                }
                try
                {
                    App.Cache.StartWorkCache.UpdateRecentAccessItem(new ViewModels.RecentAccessItem
                    {
                        FileName = fileInfo.Name,
                        DirPath = fileInfo.DirectoryName,
                        AccessTime = DateTime.Now
                    });
                }
                finally
                {
                    var window = Window.GetWindow(this);
                    window.Hide();
                    new MainWindow().Show();
                    window.Close();
                }
            });
        }

        private void btn_createConfig_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("pack://application:,,,/Pages/NewConfigPage.xaml"));
        }

        public void LoadViewCache()
        {
        }

        public void ApplyViewCache()
        {
        }

        public void LoadDataCache()
        {
            lbx_recentAccessItem.ItemsSource = App.Cache.StartWorkCache.RecentAccessItemList;
        }

        public void ApplyDataCache()
        {
        }
    }
}
