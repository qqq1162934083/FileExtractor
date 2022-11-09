using Common.Libs;
using MyTool;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace FileExtractor.Pages
{
    /// <summary>
    /// NewConfigPage.xaml 的交互逻辑
    /// </summary>
    public partial class NewConfigPage : Page
    {
        public LaunchWindow ParentWindow { get; private set; }

        public NewConfigPage()
        {
            InitializeComponent();
            Loaded += (s, e) => ParentWindow = (LaunchWindow)Window.GetWindow(this);
            InitDefaultUI();
        }

        private void InitDefaultUI()
        {
            var configDirPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            tbx_configDirPath.Text = configDirPath;
            var defatultConfigName = "文件提取配置";
            var configIndex = 1;
            var configName = (string)null;
            while (true)
            {
                configName = defatultConfigName + "(" + configIndex + ")";
                var configPath = Path.Combine(configDirPath, configName + App.ConfigNameExtName);
                if (!File.Exists(configPath)) break;
                configIndex++;
            }
            tbx_configName.Text = configName;
        }

        private void btn_previoutStep_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("pack://application:,,,/Pages/LaunchPage.xaml"));
        }

        private void btn_selectConfigDirPath_Click(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectFolder(x => tbx_configDirPath.Text = x.SelectedPath);
        }

        private void btn_createConfig_Click(object sender, RoutedEventArgs e)
        {
            //验证
            var dirPath = tbx_configDirPath.Text.Trim().TrimEnd(new char[] { '/', '\\' });
            var configName = tbx_configName.Text.Trim();
            if (!Directory.Exists(dirPath))
            {
                MessageBox.Show("选择的位置不存在，请重新设置");
                return;
            }
            if (configName.Length < 1)
            {
                MessageBox.Show("请输入配置名");
                return;
            }
            var fileName = configName + App.ConfigNameExtName;
            var filePath = Path.Combine(dirPath, fileName);
            try
            {
                File.Create(filePath).Dispose();
            }
            catch (Exception exp)
            {
                if (!Directory.Exists(dirPath))
                {
                    MessageBox.Show("选择的位置不存在，请重新设置");
                    return;
                }
                else
                {
                    MessageBox.Show("无效的配置名");
                    return;
                }
            }
            var accessItem = new ViewModels.RecentAccessItem
            {
                DirPath = dirPath,
                FileName = fileName,
                AccessTime = DateTime.Now
            };
            //缓存打开记录
            App.Cache.StartWorkCache.UpdateRecentAccessItem(accessItem);

            //进入主页面
            ParentWindow.Jump2WorkWindow(new Models.WorkData
            {
                AccessItemInfo = accessItem
            });
        }
    }
}
