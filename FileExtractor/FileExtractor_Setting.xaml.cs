using NetCore5WpfToolsApp.Utils.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace MyTool.Modules.Module_FileExtractor
{
    /// <summary>
    /// FileExtractor_Setting.xaml 的交互逻辑
    /// </summary>
    public partial class FileExtractor_Setting : Window
    {
        public event EventHandler ApplyedConfig;
        public ViewCacheMgr<FileExtractorWindow, FileExtractorDataCache, FileExtractorViewCache> CacheMgr { get; set; }

        public FileExtractor_Setting(ViewCacheMgr<FileExtractorWindow, FileExtractorDataCache, FileExtractorViewCache> cacheMgr)
        {
            CacheMgr = cacheMgr;
            InitializeComponent();
        }

        private void btn_openConfigFileListDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Explorer.exe", tbx_configDirPath.Text);
        }

        private void btn_modifyConfigDir_Click(object sender, RoutedEventArgs e)
        {
            VariableBox.Show(tbx_configDirPath.Text, result => tbx_configDirPath.Text = result);
        }

        private void btn_configDirPath_Click(object sender, RoutedEventArgs e) => FileDialogUtils.SelectFolderByFileDialog(x => tbx_configDirPath.Text = x.FileName);

        private void btn_apply_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            ApplyedConfig?.Invoke(this, null);
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        /// <summary>
        /// 加载内存配置到视图
        /// </summary>
        private void LoadConfig()
        {
            tbx_configDirPath.Text = CacheMgr.ViewCache.ConfigDirPath;
        }

        /// <summary>
        /// 重新加载内存配置到视图
        /// </summary>
        private void RealodConfig()
        {
            ClearUiConfig();
            LoadConfig();
        }

        /// <summary>
        /// 保存视图配置到内存
        /// </summary>
        private void SaveConfig()
        {
            CacheMgr.ViewCache.ConfigDirPath = tbx_configDirPath.Text.Trim();
        }

        /// <summary>
        /// 清空视图中的配置内容
        /// </summary>
        private void ClearUiConfig()
        {
            tbx_configDirPath.Text = string.Empty;
        }

        private void btn_reloadConfig_Click(object sender, RoutedEventArgs e)
        {
            RealodConfig();
        }
    }
}
