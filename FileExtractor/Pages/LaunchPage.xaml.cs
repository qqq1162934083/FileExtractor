using FileExtractor.Models;
using FileExtractor.ViewModels;
using MyTool;
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
        public LaunchWindow ParentWindow { get; set; }
        public LaunchPage()
        {
            App.Cache.StartWorkCacheMgr = new ViewCacheMgr<LaunchPage, StartWorkCache, object>(this); Loaded += (s, e) => App.Cache.StartWorkCacheMgr.NotifyLoad();
            InitializeComponent();
            Loaded += (s, e) => ParentWindow = (LaunchWindow)Window.GetWindow(this);
        }

        private void btn_openConfig_Click(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectOpenFile(x => x.Filter = "配置文件|*" + App.ConfigNameExtName, x =>
            {
                var fileInfo = new FileInfo(x.FileName);
                ConfigData data = null;
                try
                {
                    data = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(x.FileName));
                }
                catch (Exception exp)
                {
                    MessageBox.Show("打开文件失败，无法识别文件内容");
                    return;
                }
                var accessItem = new RecentAccessItem
                {
                    FileName = fileInfo.Name,
                    DirPath = fileInfo.DirectoryName,
                    AccessTime = DateTime.Now
                };
                try
                {
                    App.Cache.StartWorkCache.UpdateRecentAccessItem(accessItem);
                }
                finally
                {
                    ParentWindow.Jump2WorkWindow(new WorkData
                    {
                        AccessItemInfo = accessItem,
                        ConfigData = data
                    });
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
            ReloadRecentAccessItemList();
        }

        private void ReloadRecentAccessItemList()
        {
            lbx_recentAccessItem.ItemsSource = null;
            lbx_recentAccessItem.ItemsSource = App.Cache.StartWorkCache.RecentAccessItemList;
        }

        public void ApplyDataCache()
        {
        }

        private void menuItem_removeItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var data = (RecentAccessItem)menuItem.DataContext;
            App.Cache.StartWorkCache.RemoveRecentAccessItem(data);
            ReloadRecentAccessItemList();
        }

        private void menuItem_copyDirPath_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var data = (RecentAccessItem)menuItem.DataContext;
            Clipboard.SetDataObject(data.DirPath);
        }

        private void menuItem_copyFilePath_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var data = (RecentAccessItem)menuItem.DataContext;
            Clipboard.SetDataObject(data.FilePath);
        }

        private void ListBoxItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            var item = (FrameworkElement)sender;
            var accessItem = (RecentAccessItem)item.DataContext;
            if (!File.Exists(accessItem.FilePath))
            {
                var msgBoxResult = MessageBox.Show("该文件已经不存在，是否移除该项引用?", "警告", MessageBoxButton.YesNo);
                if (msgBoxResult == MessageBoxResult.Yes)
                {
                    //删除引用
                    App.Cache.StartWorkCache.RemoveRecentAccessItem(accessItem);
                }
                return;
            }
            else
            {
                var workData = new WorkData
                {
                    AccessItemInfo = accessItem
                };
                try
                {
                    workData.LoadConfigData();
                }
                catch(Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
                ParentWindow.Jump2WorkWindow(workData);
            }
        }
    }
}
