using Common.Libs;
using FileExtractor.Models;
using FileExtractor.Models.ObsoleteVersion;
using FileExtractor.ViewModels;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Diagnostics;

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
            Loaded += (s, e) =>
            {
                ParentWindow = (LaunchWindow)Window.GetWindow(this);

                //检查是否通过配置文件打开
                var openFilePath = App.OpenFilePath;
                if (!string.IsNullOrWhiteSpace(openFilePath))
                {
                    var file = new FileInfo(openFilePath);
                    App.OpenFilePath = null;
                    if (file.Exists) HandleOpenConfig(file.FullName);
                }
            };
        }

        private void btn_openConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialogUtils.SelectOpenFile(x => x.Filter = "配置文件|*" + App.ConfigNameExtName, x =>
                {
                    HandleOpenConfig(x.FileName);
                });
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void HandleOpenConfig(string configPath)
        {
            var fileInfo = new FileInfo(configPath);
            ConfigData data = null;
            try
            {
                data = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(configPath));
            }
            catch (Exception exp)
            {
                throw new Exception("打开文件失败，无法识别文件内容");
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
            SetBinding(lbx_recentAccessItem, ListBox.ItemsSourceProperty, App.Cache.StartWorkCache, nameof(App.Cache.StartWorkCache.RecentAccessItemList));
        }

        private void SetBinding(FrameworkElement elem, DependencyProperty dependencyProperty, object source, string path)
        {
            var binding = new Binding();
            binding.Source = source;
            binding.Path = new PropertyPath(path);
            elem.SetBinding(dependencyProperty, binding);
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
                HandleOpenConfig(accessItem.FilePath);
            }
        }

        private void btn_convertByObsoleteConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialogUtils.SelectOpenFile(x =>
                {
                    x.Filter = "配置文件|*";
                    x.Title = "选择旧版本配置文件";
                }, dialog =>
                {
                    var fileInfo = new FileInfo(dialog.FileName);
                    var oldFilePath = fileInfo.FullName;
                    if (!File.Exists(oldFilePath)) throw new Exception("选中的文件不存在或已被删除");
                    var fileContent = File.ReadAllText(oldFilePath);
                    var oldData = (FileExtractorDataCache)null;
                    try
                    {
                        oldData = JsonConvert.DeserializeObject<FileExtractorDataCache>(fileContent);
                    }
                    catch (Exception exp)
                    {
                        throw new Exception("不是有效的旧版配置文件");
                    }
                    //转化成新类型
                    var newData = new ConfigData();
                    newData.PackageDir = oldData.DestDirPath.Replace("/", "\\");
                    newData.PackageName = oldData.DestFolderName;
                    newData.EnabledCompress = oldData.EnableCompress;
                    newData.EnabledDateTimeExpression = oldData.EnableHandleTimeExpression;
                    newData.DirMappingList = new BindingList<DirMapping>(oldData.ExtractedDirInfoList.Select(item => new DirMapping()
                    {
                        SrcPath = item.SrcPath,
                        DestPath = item.DestPath
                    }).ToList());
                    newData.FileMappingList = new BindingList<FileMapping>(oldData.ExtractedFileInfoList.Select(item => new FileMapping()
                    {
                        SrcPath = item.SrcPath,
                        DestPath = item.DestPath
                    }).ToList());

                    FileDialogUtils.SelectSaveFile(saveDialog =>
                    {
                        saveDialog.Title = "选择新生成的配置文件的保存位置";
                        saveDialog.FileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
                        saveDialog.Filter = "配置文件|*" + App.ConfigNameExtName;
                        saveDialog.AddExtension = true;
                    }, saveDialog =>
                    {
                        var saveConfigPath = saveDialog.FileName;
                        var configContent2Save = JsonConvert.SerializeObject(newData, Formatting.Indented);
                        using (var textWriter = File.CreateText(saveConfigPath))
                        {
                            textWriter.Write(configContent2Save);
                            textWriter.Close();
                        }
                        HandleOpenConfig(saveConfigPath);
                    });
                });
            }
            catch (Exception convertExp)
            {
                MessageBox.Show(convertExp.Message);
            }
        }

        private void menuItem_showInExplorer_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var accessItem = (RecentAccessItem)menuItem.DataContext;
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
                Process.Start("Explorer.exe", "/select," + accessItem.FilePath);
            }
        }
    }
}
