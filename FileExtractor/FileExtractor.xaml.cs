using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MyTool.Common;
using System.Windows.Media;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO.Compression;
using MyTool.Modules.Module_FileExtractor;
using Microsoft.VisualBasic.FileIO;
using MyTool.Utils;
using NetCore5WpfToolsApp.Utils.Controls;
using Newtonsoft.Json;
using MyTool.Components;

namespace MyTool
{
    /// <summary>
    /// FileExtractor.xaml 的交互逻辑
    /// </summary>
    public partial class FileExtractor : IConfigControlCache
    {
        private ViewCacheMgr<FileExtractor, FileExtractorDataCache, FileExtractorViewCache> CacheMgr { get; set; }

        private ObservableCollection<ExtractedFileViewModel> extractedFileInfos = new ObservableCollection<ExtractedFileViewModel>();
        private CollectionLocker<ExtractedFileViewModel> extractedFileInfosLocker = new CollectionLocker<ExtractedFileViewModel>();
        private ObservableCollection<ExtractedDirViewModel> extractedDirInfos = new ObservableCollection<ExtractedDirViewModel>();
        private CollectionLocker<ExtractedDirViewModel> extractedDirInfosLocker = new CollectionLocker<ExtractedDirViewModel>();

        private bool _enableMoreOption = false;
        private bool EnableMoreOption
        {
            get => _enableMoreOption;
            set
            {
                var visibility = value ? Visibility.Visible : Visibility.Hidden;
                MoreOptionVisibilityBinder.Change(this, x => MoreOptionVisibility, visibility);
                _enableMoreOption = value;
            }
        }
        private Visibility MoreOptionVisibility { get; set; } = Visibility.Hidden;
        private Binder MoreOptionVisibilityBinder { get; set; } = new Binder();
        public FileExtractor()
        {
            CacheMgr = new ViewCacheMgr<FileExtractor, FileExtractorDataCache, FileExtractorViewCache>(this, "文件提取", "FileExtractorConfig", "FileExtractorCache");
            Closing += (s, e) => CacheMgr.NotifySave();
            Loaded += (s, e) => CacheMgr.NotifyLoad();

            InitializeComponent();
            InitBinder();
        }

        private void InitBinder()
        {
            //更多选项可视状态绑定
            MoreOptionVisibilityBinder
                .Bind(this, x => x.MoreOptionVisibility)
                .Bind(btn_reloadConfigFileList, x => x.Visibility)
                .Bind(btn_modifyPackDir, x => x.Visibility)
                .Bind(btn_batchReplaceSrcFilePath, x => x.Visibility)
                .Bind(btn_batchReplaceDestFilePath, x => x.Visibility)
                .Bind(btn_batchReplaceSrcDirPath, x => x.Visibility)
                .Bind(btn_batchReplaceDestDirPath, x => x.Visibility)
                .Bind(btn_addDirPath, x => x.Visibility)
                .Sync();
        }

        private void btn_loadConfigByFile_Click(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectOpenFile(x => x.Filter = "配置文件|*.json", x =>
            {
                CacheMgr.DataCache = JsonConvert.DeserializeObject<FileExtractorDataCache>(File.ReadAllText(x.FileName));
                LoadDataCache();
                AppendMsg($"加载配置[{x.FileName}]成功");
            });
        }

        private void btn_loadSelectedConfig_Click(object sender, RoutedEventArgs e)
        {
            var configFileInfo = cbx_configFileList.SelectedItem as FileInfo;
            if (configFileInfo == null)
            {
                AppendMsg($"加载配置失败");
                return;
            }
            CacheMgr.DataCache = JsonConvert.DeserializeObject<FileExtractorDataCache>(File.ReadAllText(configFileInfo.FullName));
            LoadDataCache();
            AppendMsg($"加载配置[{configFileInfo.Name}]成功");
        }

        private void btn_configSaveAs_Click(object sender, RoutedEventArgs e)
        {
            ApplyDataCache();
            FileDialogUtils.SelectSaveFile(x => x.Filter = "配置文件|*.json", x =>
            {
                var doYes = true;
                if (File.Exists(x.FileName))
                {
                    doYes = MsgBox.Show("该文件已存在，确定要删除该文件(移入回收站)并重新生成吗？", "重要提示", MsgBoxBtnOption.HasOkCancelBtn);
                    if (doYes) FileSystem.DeleteFile(x.FileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }

                if (!doYes) return;

                File.Create(x.FileName).Dispose();
                //写入文件
                File.WriteAllText(x.FileName, JsonConvert.SerializeObject(CacheMgr.DataCache));
                //MsgBox.Show("保存配置成功", "执行结果", MsgBoxBtnOption.HasOkBtn);
                AppendMsg("另存为配置成功");
            });
        }

        private void btn_destDirPath_Click(object sender, RoutedEventArgs e) => FileDialogUtils.SelectFolderByFileDialog(x => tbx_destDirPath.Text = x.FileName);

        private void btn_Test_Click(object sender, RoutedEventArgs e)
        {
            MsgBox.Show("系统将在15秒后关机", "警告");
        }

        private void btn_selectFiles_Click(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectOpenFile(dialog =>
            {
                var unaddFiles = new List<string>();
                foreach (var fileName in dialog.FileNames)
                {
                    if (!AddFileInfo(fileName))
                    {
                        unaddFiles.Add(fileName);
                    }
                }
                if (unaddFiles.Count > 0)
                {
                    var msgBuilder = new StringBuilder($"下列文件由于重复添加导致添加失败!\r\n");
                    unaddFiles.ForEach(x => msgBuilder.AppendLine(x));
                    msgBuilder.Remove(msgBuilder.Length - 2, 2);
                    AppendMsg("部分文件添加成功");
                    AppendMsg(msgBuilder.ToString());
                    MsgBox.Show(msgBuilder.ToString(), "温馨提示");
                }
                else AppendMsg("文件添加成功");
            }, true);
        }

        private bool AddFileInfo(string path)
        {
            if (!extractedFileInfos.Any(x => x.SrcPath == path))
            {
                extractedFileInfos.Add(new ExtractedFileViewModel(path) { EditingAreaWidth = GetEditingAreaWidth() });
                return true;
            }
            else
                return false;
        }

        private bool AddDirInfo(string path)
        {
            if (!extractedDirInfos.Any(x => x.SrcPath == path))
            {
                extractedDirInfos.Add(new ExtractedDirViewModel(path) { EditingAreaWidth = GetEditingAreaWidth() });
                return true;
            }
            else
                return false;
        }

        private void btn_lstItemRemove_Click(object sender, RoutedEventArgs e)
        {
            var data = ((Button)sender).DataContext;
            var dataType = data.GetType();
            if (dataType == typeof(ExtractedFileViewModel))
            {
                if (extractedFileInfos.Remove((ExtractedFileViewModel)data))
                    AppendMsg("移除成功");
                else
                    AppendMsg("移除失败");
            }
            else if (dataType == typeof(ExtractedDirViewModel))
            {
                if (extractedDirInfos.Remove((ExtractedDirViewModel)data))
                    AppendMsg("移除成功");
                else
                    AppendMsg("移除失败");
            }
            else
            {
                MsgBox.Show("移除失败 003", "错误提示");
                AppendMsg("移除失败 003");
            }
        }

        private void btn_lstItemEditDestPath_Click(object sender, RoutedEventArgs e)
        {
            var dataObject = ((Button)sender).DataContext;
            if (dataObject.GetType() == typeof(ExtractedFileViewModel))
            {
                var data = (ExtractedFileViewModel)dataObject;
                VariableBox.Show(data.DestPath, result => data.DestPath = result);
                ReloadObservableItem(extractedFileInfos, data, extractedFileInfosLocker);
                AppendMsg("编辑成功");
            }
            else if (dataObject.GetType() == typeof(ExtractedDirViewModel))
            {
                var data = (ExtractedDirViewModel)dataObject;
                VariableBox.Show(data.DestPath, result => data.DestPath = result);
                ReloadObservableItem(extractedDirInfos, data, extractedDirInfosLocker);
                AppendMsg("编辑成功");
            }
            else
            {
                MsgBox.Show("编辑失败 003", "错误提示");
                AppendMsg("编辑失败 003");
            }
        }

        public T FindFirstVisualChild<T>(DependencyObject obj, string childName) where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && child.GetValue(NameProperty).ToString() == childName)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindFirstVisualChild<T>(child, childName);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
        public void FindAllVisualChildList<T>(DependencyObject obj, string childName, ref List<T> resultList, Predicate<T> selector = null) where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && child.GetValue(NameProperty).ToString() == childName)
                {
                    var childControl = (T)child;
                    if (selector == null)
                        resultList.Add(childControl);
                    else
                    {
                        if (selector.Invoke(childControl))
                        {
                            resultList.Add(childControl);
                        }
                    }
                }
                FindAllVisualChildList<T>(child, childName, ref resultList, selector);
            }
        }

        private void btn_modifyDestFolderName_Click(object sender, RoutedEventArgs e)
        {
            VariableBox.Show(tbx_destFolderName.Text, result => tbx_destFolderName.Text = result);
        }

        private bool IsPacking
        {
            get
            {
                lock (packingLocker)
                {
                    return _isPacking;
                }
            }
            set
            {
                lock (packingLocker)
                {
                    _isPacking = value;
                }
            }
        }
        private bool _isPacking = false;
        private object packingLocker = new object();
        private void BeginPacking() => IsPacking = true;
        private void EndPacking() => IsPacking = false;

        private void btn_pack_Click(object sender, RoutedEventArgs e)
        {
            ApplyDataCache();
            if (IsPacking)
            {
                AppendMsg("当前正在打包中，请勿重复操作");
                return;
            }
            BeginPacking();
            AppendMsg("正在打包中....");
            Task.Run(() =>
            {
                //检查项目
                try
                {
                    new Action(() =>
                    {
                        var enableTimeExpression = CacheMgr.DataCache.EnableHandleTimeExpression;
                        var enableCompress = CacheMgr.DataCache.EnableCompress;
                        var destRootFolderName = CacheMgr.DataCache.DestFolderName?.Trim();
                        var destRootDirPath = CacheMgr.DataCache.DestDirPath?.Trim();
                        var randomCode = Guid.NewGuid().ToString("N");
                        if (string.IsNullOrEmpty(destRootFolderName))
                            throw new BizException("打包文件夹名称为空");
                        if (string.IsNullOrEmpty(destRootDirPath))
                            throw new BizException("打包目录为空");
                        try
                        {
                            destRootFolderName = enableTimeExpression ? GetHandleTimeExpressionResult(destRootFolderName) : destRootFolderName;
                        }
                        catch (GetHandleTimeExpressionResultException getHandleTimeExpressionResultException)
                        {
                            Dispatcher.Invoke(() => MsgBox.Show(getHandleTimeExpressionResultException.Message, "错误提示"));
                            AppendMsg(getHandleTimeExpressionResultException.Message);
                            return;
                        }

                        //启用压缩时更改指定路径
                        var realDestRootDirPath = destRootDirPath;
                        if (enableCompress)
                        {
                            destRootDirPath = Path.Combine(GlobalConfig.AppTmpDir, GetType().Name, randomCode);
                        }

                        //判断原路径是否存在
                        var destRootFolderInfo = new DirectoryInfo(Path.Combine(destRootDirPath, destRootFolderName));
                        var doYes = true;
                        if (destRootFolderInfo.Exists)
                        {
                            doYes = Dispatcher.Invoke(() => MsgBox.Show($"该目录下文件夹 [ {tbx_destFolderName.Text} ] 已经存在，继续操作会删除原文件夹(移入回收站)然后重新生成，是否要继续操作？", "重要提示", MsgBoxBtnOption.HasOkCancelBtn));
                            if (doYes) FileSystem.DeleteDirectory(destRootFolderInfo.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }

                        if (!doYes) return;

                        destRootFolderInfo.Create();
                        //文件映射处理
                        var fileLst = extractedFileInfos.ToList();
                        fileLst.ForEach(x =>
                        {
                            var srcFilePath = x.SrcPath;
                            var destFilePath = GetAbsoluteDestPath(destRootFolderInfo.FullName, x.DestPath, x.DestName);
                            var destFileInfo = new FileInfo(destFilePath);
                            if (!destFileInfo.Directory.Exists) Directory.CreateDirectory(destFileInfo.Directory.FullName);
                            File.Copy(x.SrcPath, destFilePath);
                        });

                        //文件夹映射处理
                        var dirLst = extractedDirInfos.ToList();
                        dirLst.ForEach(x =>
                        {
                            var srcDirPath = x.SrcPath;
                            var destDirPath = GetAbsoluteDestPath(destRootFolderInfo.FullName, x.DestPath, x.DestName);
                            Directory.CreateDirectory(destDirPath);
                            FileUtils.CopyDirRecursively(srcDirPath, destDirPath);
                        });

                        //启用压缩移动到真实位置
                        if (enableCompress)
                        {
                            //压缩
                            var zipPath = destRootFolderInfo.FullName + ".zip";
                            ZipFile.CreateFromDirectory(destRootFolderInfo.FullName, destRootFolderInfo.FullName + ".zip");
                            //移动
                            var destZipPath = Path.Combine(realDestRootDirPath, destRootFolderName + ".zip");
                            try
                            {
                                File.Move(zipPath, destZipPath);
                            }
                            finally
                            {
                                try
                                {
                                    //删除临时
                                    Directory.Delete(destRootFolderInfo.Parent.FullName, true);
                                }
                                catch { }
                            }
                        }

                        AppendMsg("打包成功");
                    }).Invoke();
                }
                catch (BizException bizExp)
                {
                    AppendMsg("打包失败，" + bizExp.Message);
                }
                catch (Exception ex)
                {
                    AppendMsg("打包失败，异常原因" + ex.Message);
                }
                EndPacking();
            });
        }

        /// <summary>
        /// 获取绝对路径
        /// </summary>
        /// <param name="baseDir">根目录</param>
        /// <param name="destRelativePath">相对路径</param>
        /// <param name="destName">目标名</param>
        /// <returns></returns>
        private string GetAbsoluteDestPath(string baseDir, string destRelativePath, string destName)
        {
            destRelativePath = destRelativePath.Trim();
            //统一路径分隔符
            baseDir = baseDir.Replace("/", "\\");
            destRelativePath = destRelativePath.Replace("/", "\\");
            //移除文件名
            if (!destRelativePath.EndsWith("\\"))
                destRelativePath = Regex.Replace(destRelativePath, "[^\\\\]*$", x => string.Empty);

            //解析相对目录
            var pathItemList = baseDir.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //驱动盘
            var diskNo = pathItemList.First();
            pathItemList.RemoveAt(0);
            pathItemList.AddRange(destRelativePath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));
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
            //拼接结果
            pathItemList.Add(destName);
            var destPath = string.Join("\\", pathItemList);
            destPath = diskNo + "\\" + destPath;

            return destPath;
        }

        private string GetHandleTimeExpressionResult(string fileName)
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
            else
            {
                throw new GetHandleTimeExpressionResultException("时间表达式有误");
            }
        }
        class GetHandleTimeExpressionResultException : Exception
        {
            public GetHandleTimeExpressionResultException(string msg) : base(msg) { }
        }

        private void btn_batchReplacePath_Click(object sender, RoutedEventArgs e)
        {
            ReplaceBox.Show(info =>
            {
                var getResultFunc = info.EnableRegex
                    ? new Func<string, string>(srcValue => Regex.Replace(srcValue, info.PatternStr, x => info.ResultStr))
                    : new Func<string, string>(srcValue => srcValue.Replace(info.PatternStr, info.ResultStr));

                if (btn_batchReplaceSrcFilePath.Equals(sender))//文件源路径
                {
                    for (int i = 0; i < extractedFileInfos.Count; i++)
                    {
                        var fileInfo = extractedFileInfos[i];
                        fileInfo.SrcPath = getResultFunc(fileInfo.SrcPath);
                        ReloadObservableItem(extractedFileInfos, fileInfo, extractedFileInfosLocker);
                    }
                }
                else if (btn_batchReplaceDestFilePath.Equals(sender))//文件目的路径
                {
                    for (int i = 0; i < extractedFileInfos.Count; i++)
                    {
                        var fileInfo = extractedFileInfos[i];
                        fileInfo.DestPath = getResultFunc(fileInfo.DestPath);
                        ReloadObservableItem(extractedFileInfos, fileInfo, extractedFileInfosLocker);
                    }
                }
                else if (btn_batchReplaceSrcDirPath.Equals(sender))//目录源路径
                {
                    for (int i = 0; i < extractedDirInfos.Count; i++)
                    {
                        var dirInfo = extractedDirInfos[i];
                        dirInfo.SrcPath = getResultFunc(dirInfo.SrcPath);
                        ReloadObservableItem(extractedDirInfos, dirInfo, extractedDirInfosLocker);
                    }
                }
                else if (btn_batchReplaceDestDirPath.Equals(sender))//目录目的路径
                {
                    for (int i = 0; i < extractedDirInfos.Count; i++)
                    {
                        var dirInfo = extractedDirInfos[i];
                        dirInfo.DestPath = getResultFunc(dirInfo.DestPath);
                        ReloadObservableItem(extractedDirInfos, dirInfo, extractedDirInfosLocker);
                    }
                }
                else
                {
                    AppendMsg("操作失败");
                    return;
                }
                AppendMsg("操作成功");
            }, "批量替换路径");
        }

        public void LoadViewCache()
        {
            var viewCache = CacheMgr.ViewCache;
            if (viewCache == null) return;
            if (!string.IsNullOrWhiteSpace(viewCache.ConfigDirPath))
            {
                ReloadConfigFileList();
            }
        }

        public void ApplyViewCache()
        {

        }

        public void ApplyDataCache()
        {
            CacheMgr.DataCache.DestDirPath = tbx_destDirPath.Text;
            CacheMgr.DataCache.DestFolderName = tbx_destFolderName.Text;
            CacheMgr.DataCache.EnableHandleTimeExpression = cb_enableTimeExpression.IsChecked.GetValueOrDefault(false);
            CacheMgr.DataCache.EnableCompress = cb_compress.IsChecked.GetValueOrDefault(false);
            CacheMgr.DataCache.ExtractedFileInfoList = extractedFileInfos.Select(x => ToExtractedFileDataModel(x)).ToList();
            CacheMgr.DataCache.ExtractedDirInfoList = extractedDirInfos.Select(x => ToExtractedDirDataModel(x)).ToList();
        }

        public void LoadDataCache()
        {
            var dataCache = CacheMgr.DataCache ?? new FileExtractorDataCache();
            tbx_destDirPath.Text = dataCache?.DestDirPath ?? string.Empty;
            tbx_destFolderName.Text = dataCache?.DestFolderName ?? string.Empty;
            cb_enableTimeExpression.IsChecked = dataCache?.EnableHandleTimeExpression ?? false;
            cb_compress.IsChecked = dataCache?.EnableCompress ?? false;

            extractedFileInfos.Clear();
            dataCache.ExtractedFileInfoList.Select(x => ToExtractedFileViewModel(x)).ToList().ForEach(x => extractedFileInfos.Add(x));
            lstBox_srcFileInfo.ItemsSource = extractedFileInfos;
            extractedDirInfos.Clear();
            dataCache.ExtractedDirInfoList.Select(x => ToExtractedDirViewModel(x)).ToList().ForEach(x => extractedDirInfos.Add(x));
            lstBox_srcDirInfo.ItemsSource = extractedDirInfos;
        }

        private void btn_applyConfig_Click(object sender, RoutedEventArgs e)
        {
            ApplyDataCache();
            AppendMsg("应用配置成功");
        }

        private void btn_resetConfig_Click(object sender, RoutedEventArgs e)
        {
            LoadDataCache();
            AppendMsg("还原配置成功");
        }

        private void AppendMsg(string msg)
        {
            var nowTime = DateTime.Now;
            msg = nowTime.ToString("[HH:mm:ss fff] - ") + msg + "\r\n";
            Dispatcher.Invoke(() =>
            {
                tbx_appMsg.AppendText(msg);
                tbx_appMsg.ScrollToEnd();
            });
        }

        private ExtractedFileViewModel ToExtractedFileViewModel(ExtractedFile extractedFile)
        {
            return new ExtractedFileViewModel
            {
                SrcPath = extractedFile.SrcPath,
                DestPath = extractedFile.DestPath,
                EditingAreaWidth = GetEditingAreaWidth()
            };
        }

        private ExtractedFile ToExtractedFileDataModel(ExtractedFileViewModel extractedFileViewModel)
        {
            return new ExtractedFile
            {
                SrcPath = extractedFileViewModel.SrcPath,
                DestPath = extractedFileViewModel.DestPath
            };
        }

        private ExtractedDirViewModel ToExtractedDirViewModel(ExtractedDir extractedDir)
        {
            return new ExtractedDirViewModel
            {
                SrcPath = extractedDir.SrcPath,
                DestPath = extractedDir.DestPath,
                EditingAreaWidth = GetEditingAreaWidth()
            };
        }

        private ExtractedDir ToExtractedDirDataModel(ExtractedDirViewModel extractedDirViewModel)
        {
            return new ExtractedDir
            {
                SrcPath = extractedDirViewModel.SrcPath,
                DestPath = extractedDirViewModel.DestPath
            };
        }

        private void tbx_configDirPath_TextChanged(object sender, TextChangedEventArgs e) => ReloadConfigFileList();

        private void ReloadConfigFileList()
        {
            try
            {
                var path = CacheMgr.ViewCache.ConfigDirPath.Trim();
                if (path.Length == 0)
                {
                    cbx_configFileList.ItemsSource = null;
                    return;
                }
                var dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Exists) return;
                cbx_configFileList.ItemsSource = dirInfo.GetFiles("*.json");
                cbx_configFileList.DisplayMemberPath = "Name";
                if (cbx_configFileList.Items.Count > 0) cbx_configFileList.SelectedIndex = 0;
            }
            catch { }
        }

        private void btn_reloadConfigFileList_Click(object sender, RoutedEventArgs e) => ReloadConfigFileList();

        private double GetEditingAreaWidth()
        {
            return 60;
        }

        private void SyncEditStatus()
        {
            var targetWidth = GetEditingAreaWidth();
            var extractedFileInfoList = extractedFileInfos.Where(x => x.EditingAreaWidth != targetWidth).ToList();
            extractedFileInfoList.ForEach(x =>
            {
                x.EditingAreaWidth = targetWidth;
                ReloadObservableItem(extractedFileInfos, x, extractedFileInfosLocker);
            });
            var extractedDirInfoList = extractedDirInfos.Where(x => x.EditingAreaWidth != targetWidth).ToList();
            extractedDirInfoList.ForEach(x =>
            {
                x.EditingAreaWidth = targetWidth;
                ReloadObservableItem(extractedDirInfos, x, extractedDirInfosLocker);
            });
        }

        private void ReloadObservableItem<T>(ObservableCollection<T> collection, T item, CollectionLocker<T> locker)
        {
            lock (locker)
            {
                var index = collection.IndexOf(item);
                if (index < 0) throw new Exception("该元素不存在或已经被移除");
                collection.Remove(item);
                collection.Insert(index, item);
            }
        }

        private void btn_selectDir_Click(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectFolderByFileDialog(dialog =>
            {
                var dirPath = dialog.FileName;
                if (!AddDirInfo(dirPath))
                {
                    AppendMsg("目录添加失败");
                    MsgBox.Show("目录添加失败", "错误提示");
                }
                else AppendMsg("目录添加成功");
            });
        }

        private void btn_saveConfig_Click(object sender, RoutedEventArgs e)
        {
            ApplyDataCache();
            CacheMgr.ApplyDataCache();
            AppendMsg("保存配置成功");
        }

        private void btn_openPackDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Explorer.exe", tbx_destDirPath.Text);
        }

        private void btn_addDirPath_Click(object sender, RoutedEventArgs e)
        {
            VariableBox.Show(x =>
            {
                if (!AddDirInfo(x))
                {
                    AppendMsg("添加目录失败");
                    MsgBox.Show("添加目录失败");
                }
                else
                {
                    AppendMsg("添加目录成功");
                }
            }, "添加目录路径");
        }

        private void btn_modifyPackDir_Click(object sender, RoutedEventArgs e)
        {
            VariableBox.Show(tbx_destDirPath.Text, result => tbx_destDirPath.Text = result);
        }

        private void btn_setting_Click(object sender, RoutedEventArgs e)
        {
            var settingWindow = new FileExtractor_Setting(CacheMgr);
            settingWindow.ApplyedConfig += SettingWindow_ApplyedConfig;
            settingWindow.ShowDialog();
        }

        private void SettingWindow_ApplyedConfig(object sender, EventArgs e)
        {
            ReloadConfigFileList();
        }

        private void btn_switchMoreOption_Click(object sender, RoutedEventArgs e)
        {
            EnableMoreOption = !EnableMoreOption;
            if (EnableMoreOption) btn_switchMoreOption.Header = "关闭更多选项";
            else btn_switchMoreOption.Header = "开启更多选项";
            Debug.WriteLine(MoreOptionVisibility);
        }

        private void btn_openConfigFileListDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Explorer.exe", CacheMgr.ViewCache.ConfigDirPath);
        }

        private void btn_configFilePath_Click(object sender, RoutedEventArgs e)
        {
            FileDialogUtils.SelectFolderByFileDialog(dialog =>
            {
                CacheMgr.DataCache.ConfigFileListDirPath = dialog.FileName;
                ReloadConfigFileList();
            });
        }
    }
}