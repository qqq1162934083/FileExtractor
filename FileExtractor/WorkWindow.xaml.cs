using Common.Libs;
using FileExtractor.Dialogs;
using FileExtractor.Models;
using FileExtractor.ViewModels;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic.FileIO;
using System.IO.Compression;

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
            configData.CalcNo4BindingList();
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

        /// <summary>
        /// 获取和处理时间日期表达式
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string ParseTimeExp(string fileName)
        {
            var timeExpressionRegexPattern = "\\$\\{\\s*TimePattern\\s*\\=\\s*\"\\s*[\\d\\D]*?\\s*\"\\s*\\}";//日期时间表达式匹配正则表达式
            var timePatternRegexPattern = "(?<=\\$\\{\\s*TimePattern\\s*\\=\\s*\"\\s*)[\\d\\D]*?(?=\\s*\"\\s*\\})";//日期表达式中的时间规则提取正则表达式
            var match = Regex.Match(fileName, timeExpressionRegexPattern);
            if (match.Success)
            {
                var timeExpressionStr = match.Value;
                var timePatternStr = Regex.Match(timeExpressionStr, timePatternRegexPattern).Value;
                var timeStr = DateTime.Now.ToString(timePatternStr);
                var result = fileName.Substring(0, match.Index);
                result += timeStr;
                result += fileName.Substring(match.Index + match.Length);
                return result;
            }
            return fileName;
        }

        /// <summary>
        /// 解析路径中的变量
        /// </summary>
        /// <param name="path"></param>
        /// <param name="valueMappingList"></param>
        /// <returns></returns>
        private string ParseValueByVarValue(string path, IList<ValueMapping> valueMappingList)
        {
            return Regex.Replace(path, "\\$\\{[^\\$\\\\/\\{\\}\\s]+?\\}", m =>
             {
                 var varName = m.Value.Substring(2, m.Value.Length - 3);
                 if (string.IsNullOrWhiteSpace(varName)) throw new Exception($"路径[{path}]中出现空变量名变量");
                 var valueMapping = valueMappingList.FirstOrDefault(x => x.VarName == varName);
                 if (valueMapping == null) throw new Exception($"没有配置变量名为[{varName}]的环境变量");
                 return valueMapping.VarValue;
             });
        }

        /// <summary>
        /// 获取目的地路径
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        private string GetDestPath(string srcPath, string destPath)
        {
            if (destPath.EndsWith("\\")) return destPath.TrimEnd('\\') + "\\" + srcPath.Substring(srcPath.LastIndexOf("\\") + 1).TrimStart('\\');
            return destPath;
        }

        /// <summary>
        /// 解析相对路径
        /// 从相对路径解析得到真实路径
        /// </summary>
        /// <returns></returns>
        private string ParseRelativePath(string relativePath)
        {
            relativePath.Trim();

            var targetName = relativePath.Substring(relativePath.LastIndexOf("\\") + 1);
            var dirPath = relativePath.Substring(0, relativePath.Length - targetName.Length - 1);

            //解析相对目录
            var pathItemList = dirPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            //驱动盘
            var diskNo = pathItemList.First();
            pathItemList.RemoveAt(0);

            var upCount = 0;
            var indexOfPathItemList = pathItemList.Count - 1;
            while (indexOfPathItemList >= 0)
            {
                if (pathItemList[indexOfPathItemList].Equals(".."))
                {
                    upCount++;
                    pathItemList.RemoveAt(indexOfPathItemList);
                }
                else
                {
                    if (upCount > 0)
                    {
                        upCount--;
                        pathItemList.RemoveAt(indexOfPathItemList);
                    }
                }
                indexOfPathItemList--;
            }
            pathItemList.Add(targetName);//拼接结果
            var destPath = string.Join("\\", pathItemList);
            destPath = diskNo + "\\" + destPath;

            return destPath;
        }

        private async void btn_pack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btn_pack.IsEnabled = false;
                await Task.Run(() => HandleConfigDataIfNotNull(configData =>
                {
                    AppendConsoleMessage("正在打包...");
                    var enableTimeExp = configData.EnabledDateTimeExpression;
                    var enableCompress = configData.EnabledCompress;
                    var enableFtp = configData.EnabledPackageDirFtpSupport;

                    var o_packageName = configData.PackageName;
                    var o_packageDir = configData.PackageDir;
                    if (string.IsNullOrEmpty(o_packageName)) throw new Exception("设置的包名为空");
                    if (string.IsNullOrEmpty(o_packageDir)) throw new Exception("设置的打包目录为空");
                    var packageName = o_packageName;
                    packageName = enableTimeExp ? ParseTimeExp(packageName) : packageName;
                    packageName = ParseValueByVarValue(packageName, configData.ValueMappingList);
                    var packageDir = ParseValueByVarValue(o_packageDir, configData.ValueMappingList);
                    if (string.IsNullOrEmpty(packageName)) throw new Exception("解析的包名结果为空");
                    if (string.IsNullOrEmpty(packageDir)) throw new Exception("解析的打包目录结果为空");

                    var tmpCompressFileRandomCode = Guid.NewGuid().ToString("N");//临时压缩文件guid

                    var compressedFileName = (string)null;//压缩文件名称，如果启用了压缩
                    var tmpPackageInfo = (DirectoryInfo)null; //临时打包文件夹信息，启用压缩则打包在temp目录，不启用则为打包的目的路径
                    var compressedFilePackagePath = (FileInfo)null;//压缩文件最终上传路径，如果启用了压缩
                    #region 判断原路径对象(文件夹或者压缩包)是否存在，如果存在询问删除以继续
                    if (enableCompress)
                    {
                        compressedFileName = packageName + ".zip";
                        compressedFilePackagePath = new FileInfo(Path.Combine(packageDir, compressedFileName));
                        if (compressedFilePackagePath.Exists)
                        {
                            if (MessageBoxResult.OK != Dispatcher.Invoke(() => MessageBox.Show($"指定的包目录下压缩文件 [ {compressedFileName} ] 已经存在，继续操作会删除原文件夹(移入回收站)然后重新生成，是否要继续操作？", "重要提示", MessageBoxButton.OKCancel))) return;
                            FileSystem.DeleteFile(compressedFilePackagePath.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                        tmpPackageInfo = new DirectoryInfo(Path.Combine(GlobalConfig.AppTmpDir, GetType().Name, tmpCompressFileRandomCode, packageName));
                    }
                    else
                    {

                        tmpPackageInfo = new DirectoryInfo(Path.Combine(packageDir, packageName));
                        if (tmpPackageInfo.Exists)
                        {
                            if (MessageBoxResult.OK != Dispatcher.Invoke(() => MessageBox.Show($"指定的包目录下文件夹 [ {packageName} ] 已经存在，继续操作会删除原文件夹(移入回收站)然后重新生成，是否要继续操作？", "重要提示", MessageBoxButton.OKCancel))) return;
                            FileSystem.DeleteDirectory(tmpPackageInfo.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                    }
                    #endregion

                    //临时打包目录，如果启用压缩，则为压缩临时文件夹
                    tmpPackageInfo.Create();//创建临时打包的目录

                    //映射处理
                    configData.FileMappingList.ToList().ForEach(mapping =>
                    {
                        var o_srcPath = mapping.SrcPath;
                        var srcPath = ParseValueByVarValue(o_srcPath, configData.ValueMappingList);
                        srcPath = ParseRelativePath(srcPath);

                        if (!File.Exists(srcPath)) throw new Exception($"文件[{o_srcPath}=>{srcPath}]不存在");

                        var o_destPath = mapping.DestPath;
                        var destPath = tmpPackageInfo.FullName.TrimEnd('\\') + "\\" + o_destPath.TrimStart('\\');
                        destPath = GetDestPath(srcPath, destPath);
                        destPath = ParseValueByVarValue(destPath, configData.ValueMappingList);

                        var destFileInfo = new FileInfo(destPath);
                        if (!destFileInfo.Directory.Exists) Directory.CreateDirectory(destFileInfo.Directory.FullName);
                        File.Copy(srcPath, destPath);
                    });
                    configData.DirMappingList.ToList().ForEach(mapping =>
                    {
                        var o_srcPath = mapping.SrcPath;
                        var srcPath = ParseValueByVarValue(o_srcPath, configData.ValueMappingList);
                        srcPath = ParseRelativePath(srcPath);

                        if (!Directory.Exists(srcPath)) throw new Exception($"文件夹[{o_srcPath}=>{srcPath}]不存在");

                        var o_destPath = mapping.DestPath;
                        var destPath = tmpPackageInfo.FullName.TrimEnd('\\') + "\\" + o_destPath.TrimStart('\\');
                        destPath = GetDestPath(srcPath, destPath);
                        destPath = ParseValueByVarValue(destPath, configData.ValueMappingList);

                        FileUtils.CopyDirRecursively(srcPath, destPath);
                    });

                    //如果启用压缩移动到真实位置
                    if (enableCompress)
                    {
                        //压缩
                        var tmpZipPath = Path.Combine(tmpPackageInfo.Parent.FullName, compressedFileName);
                        ZipFile.CreateFromDirectory(tmpPackageInfo.FullName, tmpZipPath);
                        try
                        {
                            if (!compressedFilePackagePath.Directory.Exists) compressedFilePackagePath.Directory.Create();
                            File.Copy(tmpZipPath, compressedFilePackagePath.FullName);
                        }
                        finally
                        {
                            try
                            {
                                //删除临时
                                Directory.Delete(tmpPackageInfo.Parent.FullName, true);
                            }
                            catch { }
                        }
                    }
                    AppendConsoleMessage("打包完成", 1);
                }));
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                btn_pack.IsEnabled = true;
            }
        }

        /// <summary>
        /// 显示状态信息
        /// </summary>
        /// <param name="message"></param>
        private void AppendConsoleMessage(string message, int type = 0)
        {
            var nowTime = DateTime.Now;
            message = "[" + nowTime.ToString("HH:mm:ss fff") + "]# " + message;
            var colorStr = nameof(Colors.White);
            switch (type)
            {
                case -1: colorStr = nameof(Colors.Red); break;
                case 1: colorStr = nameof(Colors.LightGreen); break;
            }
            Dispatcher.Invoke(() =>
            {
                var para = new Paragraph(new Run(message) { Foreground = (Brush)new BrushConverter().ConvertFromString(colorStr) });
                rtbx_consoleInfo.Document.Blocks.Add(para);
                rtbx_consoleInfo.ScrollToEnd();
            });
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
                HandleConfigDataIfNotNull(configData =>
                {
                    switch (dialog.FuncIndex)
                    {
                        case 0:
                            var srcFilePath = dialog.tbx_fileMapping_source.Text;
                            var destFilePath = dialog.tbx_fileMapping_dest.Text;
                            //此处还需要添加验证
                            srcFilePath = srcFilePath.Replace("/", "\\");
                            destFilePath = destFilePath.Replace("/", "\\");
                            configData.FileMappingList.Add(new FileMapping() { DestPath = destFilePath, SrcPath = srcFilePath });
                            break;
                        case 1:
                            var srcDirPath = dialog.tbx_dirMapping_source.Text;
                            var destDirPath = dialog.tbx_dirMapping_dest.Text;
                            //此处还需要添加验证
                            srcDirPath = srcDirPath.Replace("/", "\\");
                            destDirPath = destDirPath.Replace("/", "\\");
                            configData.DirMappingList.Add(new DirMapping() { DestPath = destDirPath, SrcPath = srcDirPath });
                            break;
                        case 2:
                            var varName = dialog.tbx_varName.Text;
                            var varValue = dialog.tbx_varValue.Text;
                            //此处还需要添加验证
                            configData.ValueMappingList.Add(new ValueMapping() { VarName = varName, VarValue = varValue });
                            break;
                        default:
                            throw new Exception("超出预期范围");
                    }
                    configData.CalcNo4BindingList();
                    WorkData.SaveConfigData();
                });
            });
        }

        private void btn_addItemByChoose_Click(object sender, RoutedEventArgs e)
        {
            HandleConfigDataIfNotNull(configData =>
            {
                switch (tabControl_itemList.SelectedIndex)
                {
                    case 0:
                        FileDialogUtils.SelectOpenFile(x => x.Filter = "文件|*", x =>
                        {
                            var filePathList = new List<string>();
                            if (x.FileNames != null) filePathList.AddRange(x.FileNames);
                            else filePathList.Add(x.FileName);
                            foreach (var filePath in filePathList)
                            {
                                //此处还需要添加验证
                                configData.FileMappingList.Add(new FileMapping() { DestPath = "\\", SrcPath = filePath });
                            }
                        }, true);
                        break;
                    case 1:
                        FileDialogUtils.SelectFolder(x =>
                        {
                            //此处还需要添加验证
                            configData.DirMappingList.Add(new DirMapping() { DestPath = "\\", SrcPath = x.SelectedPath });
                        });
                        break;
                    default:
                        throw new Exception("不在预期的范围");
                }
                configData.CalcNo4BindingList();
                WorkData.SaveConfigData();
            });
        }

        private void menuItem_removeItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var data = menuItem.DataContext;
            if (data == null) throw new Exception($"操作失败：未选中数据");
            HandleConfigDataIfNotNull(configData =>
            {
                switch (data)
                {
                    case FileMapping _:
                        var fileMappingData = (FileMapping)data;
                        configData.FileMappingList.Remove(fileMappingData);
                        break;
                    case DirMapping _:
                        var dirMappingData = (DirMapping)data;
                        configData.DirMappingList.Remove(dirMappingData);
                        break;
                    case ValueMapping _:
                        var valueMappingData = (ValueMapping)data;
                        configData.ValueMappingList.Remove(valueMappingData);
                        break;
                    default:
                        throw new Exception("选中的数据类型异常");
                }
                configData.CalcNo4BindingList();
                WorkData.SaveConfigData();
            });
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
                        var valueMappingData = (ValueMapping)data;
                        valueMappingData.VarName = dialog.tbx_varName.Text;
                        valueMappingData.VarValue = dialog.tbx_varValue.Text;
                        break;
                    default:
                        throw new Exception("超出预期范围");
                }
                WorkData.SaveConfigData();
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


        private void btn_setPackedDestDirByChoose_Click2(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectFolderByFileDialog(x =>
            {
                //此处还需要添加验证
                HandleConfigDataIfNotNull(configData =>
                {
                    configData.PackageDir = x.FileName;
                    WorkData.SaveConfigData();
                });
            });
        }

        private void btn_openPackedDestDir_Click(object sender, RoutedEventArgs e)
        {
            HandleConfigDataIfNotNull(configData =>
            {
                var path = tbx_packageDir.Text;
                path = ParseValueByVarValue(path, configData.ValueMappingList);
                var dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    if (MessageBoxResult.OK != Dispatcher.Invoke(() => MessageBox.Show($"目录信息：\r\n{tbx_packageDir.Text}\r\n\t↓↓解析结果↓↓\r\n{path}\r\n\r\n您要打开的目录尚不存在，若要继续打开此目录则会自动创建该目录，是否创建并打开？", "重要提示", MessageBoxButton.OKCancel))) return;
                    dir.Create();
                }
                Process.Start("Explorer.exe", path);
            });
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

        private void menuItem_closeCurrConfig_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            new LaunchWindow().Show();
            Close();
        }

        private void menuItem_bulkReplace_Click(object sender, RoutedEventArgs e)
        {
            int funcIndex = tabControl_itemList.SelectedIndex;
            switch (funcIndex)
            {

            }
        }

        private void menuItem_aboutInfo_Click(object sender, RoutedEventArgs e)
        {
            new AboutInfoDialog().ShowDialog();
        }

        private void menuItem_showInExplorer_Click(object sender, RoutedEventArgs e)
        {
            HandleConfigDataIfNotNull(x =>
            {
                Process.Start("Explorer.exe", "/select," + WorkData.AccessItemInfo.FilePath);
            });
        }
    }
}
