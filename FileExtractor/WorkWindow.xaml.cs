using Common.Libs;
using FileExtractor.Dialogs;
using FileExtractor.Models;
using FileExtractor.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            SetBinding(cb_enabledPackageDirFtpSupport, CheckBox.IsCheckedProperty, configData, nameof(configData.EnabledPackageDirFtpSupport));
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
            Console.WriteLine(JsonConvert.SerializeObject(WorkData.ConfigData, Formatting.Indented));
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
                        //此处还需要添加验证
                        srcFilePath = srcFilePath.Replace("/", "\\");
                        destFilePath = destFilePath.Replace("/", "\\");
                        WorkData.ConfigData.FileMappingList.Add(new FileMapping() { DestPath = destFilePath, SrcPath = srcFilePath });
                        WorkData.SaveConfigData();
                        break;
                    case 1:
                        var srcDirPath = dialog.tbx_dirMapping_source.Text;
                        var destDirPath = dialog.tbx_dirMapping_dest.Text;
                        //此处还需要添加验证
                        srcDirPath = srcDirPath.Replace("/", "\\");
                        destDirPath = destDirPath.Replace("/", "\\");
                        WorkData.ConfigData.DirMappingList.Add(new DirMapping() { DestPath = destDirPath, SrcPath = srcDirPath });
                        WorkData.SaveConfigData();
                        break;
                    case 2:
                        var varName = dialog.tbx_varName.Text;
                        var varValue = dialog.tbx_varValue.Text;
                        //此处还需要添加验证
                        WorkData.ConfigData.ValueMappingList.Add(new ValueMapping() { VarName = varName, VarValue = varValue });
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
                    FileDialogUtils.SelectOpenFile(x => x.Filter = "文件|*", x =>
                    {
                        //此处还需要添加验证
                        WorkData.ConfigData.FileMappingList.Add(new FileMapping() { DestPath = "\\", SrcPath = x.FileName });
                        WorkData.SaveConfigData();
                        WorkData.ConfigData.NotifyChanged(nameof(WorkData.ConfigData.FileMappingList));
                    });
                    break;
                case 1:
                    FileDialogUtils.SelectFolder(x =>
                    {
                        //此处还需要添加验证
                        WorkData.ConfigData.DirMappingList.Add(new DirMapping() { DestPath = "\\", SrcPath = x.SelectedPath });
                        WorkData.SaveConfigData();
                        WorkData.ConfigData.NotifyChanged(nameof(WorkData.ConfigData.FileMappingList));
                    });
                    break;
                default:
                    throw new Exception("不在预期的范围");
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
                    WorkData.SaveConfigData();
                    break;
                case DirMapping _:
                    var dirMappingData = (DirMapping)data;
                    WorkData.ConfigData.DirMappingList.Remove(dirMappingData);
                    WorkData.SaveConfigData();
                    break;
                case ValueMapping _:
                    var valueMappingData = (ValueMapping)data;
                    WorkData.ConfigData.ValueMappingList.Remove(valueMappingData);
                    WorkData.SaveConfigData();
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

        private void tabControl_itemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btn_addItemByChoose.IsEnabled = tabControl_itemList.SelectedIndex != 2;
        }

        private void cb_enabledDateTimeExpression_CheckedChanged(object sender, RoutedEventArgs e)
        {
            HandleConfigDataIfNotNull(configData => WorkData.SaveConfigData());
        }
        private void cb_enabledCompress_CheckedChanged(object sender, RoutedEventArgs e)
        {
            HandleConfigDataIfNotNull(configData => WorkData.SaveConfigData());
        }
        private void cb_enabledPackageDirFtpSupport_CheckedChanged(object sender, RoutedEventArgs e)
        {
            HandleConfigDataIfNotNull(configData => WorkData.SaveConfigData());
        }

        private void btn_setPackedDestDirByTyping_Click(object sender, RoutedEventArgs e)
        {
            ValueBox.Show("设置路径值：", tbx_packageDir.Text, (srcValue, destValue) =>
            {
                //此处还需要添加验证
                HandleConfigDataIfNotNull(configData =>
                {
                    configData.PackageDir = destValue;
                    WorkData.SaveConfigData();
                });
            });
        }

        private void btn_setPackedDestDirByChoose_Click(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectFolder(x =>
            {
                //此处还需要添加验证
                HandleConfigDataIfNotNull(configData =>
                {
                    configData.PackageDir = x.SelectedPath;
                    WorkData.SaveConfigData();
                });
            });
        }

        private void btn_openPackedDestDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Explorer.exe", tbx_packageDir.Text);
        }

        /// <summary>
        /// 如果configData不为空则进行处理
        /// </summary>
        /// <param name="handle"></param>
        private void HandleConfigDataIfNotNull(Action<ConfigData> handle)
        {
            var configData = WorkData?.ConfigData;
            if (configData != null)
            {
                handle?.Invoke(configData);
            }
        }

        private void btn_setPackageNameByTyping_Click(object sender, RoutedEventArgs e)
        {
            ValueBox.Show("设置包名：", tbx_packageName.Text, (srcValue, destValue) =>
            {
                //此处还需要添加验证
                HandleConfigDataIfNotNull(configData =>
                {
                    configData.PackageName = destValue;
                    WorkData.SaveConfigData();
                });
            });
        }
    }
}
