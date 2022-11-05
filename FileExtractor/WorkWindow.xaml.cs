using FileExtractor.Dialogs;
using FileExtractor.Models;
using FileExtractor.ViewModels;
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
            SetBinding(lbx_varMapping, ListBox.ItemsSourceProperty, configData, nameof(configData.ValueMappingList));
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
            ItemInfoDialog.ShowDialog(tabControl_itemList.SelectedIndex, null, dialog =>
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
                        WorkData.SaveConfigData();
                        break;
                    case 2:
                        var varName = dialog.tbx_varName.Text;
                        var varValue = dialog.tbx_varValue.Text;
                        //验证
                        WorkData.ConfigData.ValueMappingList.Add(new ViewModels.ValueMapping() { VarName = varName, VarValue = varValue });
                        WorkData.SaveConfigData();
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

        private void menuItem_removeItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var data = menuItem.DataContext;
            if (data == null) throw new Exception($"操作失败：未选中数据");
            switch (data)
            {
                case FileMapping _:
                    var fileMappingData = (FileMapping)data;
                    WorkData.ConfigData.FileMappingList.Remove(fileMappingData);
                    break;
                case DirMapping _:
                    var dirMappingData = (DirMapping)data;
                    WorkData.ConfigData.DirMappingList.Remove(dirMappingData);
                    break;
                case ValueMapping _:
                    var valueMappingData = (ValueMapping)data;
                    WorkData.ConfigData.ValueMappingList.Remove(valueMappingData);
                    break;
                default:
                    throw new Exception("选中的数据类型异常");
            }
            WorkData.SaveConfigData();
        }


        private void menuItem_editItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var data = menuItem.DataContext;
            if (data == null) throw new Exception($"操作失败：未选中数据");
            var funcIndex = 0;
            switch (data)
            {
                case FileMapping _:
                    funcIndex = 0;
                    break;
                case DirMapping _:
                    funcIndex = 1;
                    break;
                case ValueMapping _:
                    funcIndex = 2;
                    break;
                default:
                    throw new Exception("选中的数据类型异常");
            }
            WorkData.SaveConfigData();

            ItemInfoDialog.ShowDialog(funcIndex, dialog =>
            {
                switch (funcIndex)
                {
                    case 0:
                        var fileMappingData = (FileMapping)data;
                        dialog.tbx_fileMapping_source.Text = fileMappingData.SrcPath;
                        dialog.tbx_fileMapping_dest.Text = fileMappingData.DestPath;
                        break;
                    case 1:
                        var dirMappingData = (DirMapping)data;
                        dialog.tbx_dirMapping_source.Text = dirMappingData.SrcPath;
                        dialog.tbx_dirMapping_dest.Text = dirMappingData.DestPath;
                        break;
                    case 2:
                        var valueMappingData = (ValueMapping)data;
                        dialog.tbx_varName.Text = valueMappingData.VarName;
                        dialog.tbx_varValue.Text = valueMappingData.VarValue;
                        break;
                    default:
                        throw new Exception("超出预测的功能范围");
                }
            }, dialog =>
            {
                switch (dialog.FuncIndex)
                {
                    case 0:
                        var fileMappingData = (FileMapping)data;
                        fileMappingData.SrcPath = dialog.tbx_fileMapping_source.Text;
                        fileMappingData.DestPath = dialog.tbx_fileMapping_dest.Text;
                        break;
                    case 1:
                        var dirMappingData = (DirMapping)data;
                        dirMappingData.SrcPath = dialog.tbx_dirMapping_source.Text;
                        dirMappingData.DestPath = dialog.tbx_dirMapping_dest.Text;
                        break;
                    case 2:
                        break;
                    default:
                        throw new Exception("超出预期范围");
                }
            });
        }
    }
}
