using FileExtractor.Dialogs;
using FileExtractor.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FileExtractor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WorkWindow : Window
    {
        public WorkData WorkData { get; set; }

        public WorkWindow() : this(null) { }
        public WorkWindow(WorkData workData)
        {
            InitializeComponent();
            ReloadWorkData(workData);
        }

        /// <summary>
        /// 重新加载配置数据
        /// </summary>
        private void ReloadWorkData(WorkData workData)
        {
            WorkData = workData;
            if (WorkData.ConfigData == null)
                workData.ConfigData = new ViewModels.ConfigData();
            var configData = workData.ConfigData;
            //加载视图
            SetBinding(lbx_fileMapping, ListBox.ItemsSourceProperty, configData, nameof(configData.FileMappingList));
            SetBinding(lbx_dirMapping, ListBox.ItemsSourceProperty, configData, nameof(configData.DirMappingList));
            SetBinding(tbx_packageDir, TextBox.TextProperty, configData, nameof(configData.PackageDir));
            SetBinding(tbx_packageName, TextBox.TextProperty, configData, nameof(configData.PackageName));
            SetBinding(cb_enabledCompress, CheckBox.IsCheckedProperty, configData, nameof(configData.EnabledCompress));
            SetBinding(cb_enabledDateTimeExpression, CheckBox.IsCheckedProperty, configData, nameof(configData.EnabledDateTimeExpression));
        }
        private void SetBinding(FrameworkElement elem, DependencyProperty dependencyProperty, object source, string path)
        {
            var binding = new Binding();
            binding.Source = source;
            binding.Path = new PropertyPath(path);
            elem.SetBinding(dependencyProperty, binding);
        }

        private void btn_pack_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_packedDestNameOptions_Click(object sender, RoutedEventArgs e)
        {
            popup_btn_packedDestNameOptions.IsOpen = true;
        }

        private void btn_packedDestDirOptions_Click(object sender, RoutedEventArgs e)
        {
            popup_btn_packedDestDirOptions.IsOpen = true;
        }

        private void btn_addItemByTyping_Click(object sender, RoutedEventArgs e)
        {
            ItemInfoDialog.ShowDialog(tabControl_itemList.SelectedIndex, dialog =>
            {
                switch (dialog.FuncIndex)
                {
                    case 0:
                        var srcFilePath = dialog.tbx_fileMapping_source.Text;
                        var destFilePath = dialog.tbx_fileMapping_dest.Text;
                        //验证
                        WorkData.ConfigData.FileMappingList.Add(new ViewModels.FileMapping() { DestPath = destFilePath, SrcPath = srcFilePath });
                        WorkData.SaveConfigData();
                        WorkData.ConfigData.NotifyChanged(nameof(WorkData.ConfigData.FileMappingList));
                        break;
                    case 1:
                        var srcDirPath = dialog.tbx_dirMapping_source.Text;
                        var destDirPath = dialog.tbx_dirMapping_dest.Text;
                        //验证
                        WorkData.ConfigData.DirMappingList.Add(new ViewModels.DirMapping() { DestPath = destDirPath, SrcPath = srcDirPath });
                        break;
                    case 2:
                        var varName = dialog.tbx_varName.Text;
                        var varValue = dialog.tbx_varValue.Text;
                        //验证
                        //WorkData.ConfigData.Add(new ViewModels.DirMapping() { DestPath = destDirPath, SrcPath = srcDirPath });
                        break;
                    default:
                        throw new Exception("超出预期范围");
                }
            });
        }

        private void btn_addItemByChoose_Click(object sender, RoutedEventArgs e)
        {
            switch (tabControl_itemList.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }
    }
}
