using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyTool.Common
{
    public class ExtractedFile
    {
        public ExtractedFile(string srcFilePath, string destFilePath)
        {
            SrcPath = srcFilePath ?? throw new ArgumentNullException(nameof(srcFilePath));
            DestPath = destFilePath ?? "\\";
        }

        public ExtractedFile(string srcFilePath) : this(srcFilePath, null) { }

        public ExtractedFile() { }

        /// <summary>
        /// 绝对路径
        /// </summary>
        public string SrcPath { get; set; }
        /// <summary>
        /// 相对路径/绝对路径
        /// </summary>
        public string DestPath { get; set; }
    }

    public class ExtractedFileViewModel
    {
        /// <summary>
        /// 原文件名
        /// </summary>
        public string SrcName
        {
            get
            {
                var match = Regex.Match(SrcPath, "(?<=[\\\\/])[^\\\\/]*$");
                if (match.Success) return match.Value;
                else return "[无法解析文件名]";
            }
            set { }
        }

        /// <summary>
        /// 目的文件名
        /// </summary>
        public string DestName
        {
            get
            {
                var match = Regex.Match(DestPath, "(?<=[\\\\/])[^\\\\/]*$");
                if (match.Success)
                {
                    var destFileName = match.Value.Trim();
                    if (destFileName.Length == 0) return SrcName;
                    else return destFileName;
                }
                else return "[无法解析文件名]";
            }
            set { }
        }
        /// <summary>
        /// 原文件绝对路径
        /// </summary>
        public string SrcPath { get; set; }
        /// <summary>
        /// 目标文件 相对路径/绝对路径
        /// </summary>
        public string DestPath { get; set; }

        /// <summary>
        /// 映射编辑按钮区域宽度
        /// </summary>
        public double EditingAreaWidth { get; set; }

        public ExtractedFileViewModel(string srcFilePath, string destFilePath)
        {
            SrcPath = srcFilePath ?? throw new ArgumentNullException(nameof(srcFilePath));
            DestPath = destFilePath ?? "\\";
        }
        public ExtractedFileViewModel(string srcDirPath) : this(srcDirPath, null) { }
        public ExtractedFileViewModel() { }
    }

    public class ExtractedDir
    {
        public ExtractedDir(string srcDirPath, string destDirPath)
        {
            SrcPath = srcDirPath ?? throw new ArgumentNullException(nameof(srcDirPath));
            DestPath = destDirPath ?? "\\";
        }

        public ExtractedDir(string srcDirPath) : this(srcDirPath, null) { }

        public ExtractedDir() { }

        /// <summary>
        /// 绝对路径
        /// </summary>
        public string SrcPath { get; set; }
        /// <summary>
        /// 相对路径/绝对路径
        /// </summary>
        public string DestPath { get; set; }
    }

    public class ExtractedDirViewModel
    {
        public ExtractedDirViewModel(string srcDirPath, string destDirPath)
        {
            SrcPath = srcDirPath ?? throw new ArgumentNullException(nameof(srcDirPath));
            DestPath = destDirPath ?? "\\";
        }

        public ExtractedDirViewModel(string srcDirPath) : this(srcDirPath, null) { }

        public ExtractedDirViewModel() { }

        /// <summary>
        /// 绝对路径
        /// </summary>
        public string SrcPath { get; set; }
        /// <summary>
        /// 相对路径/绝对路径
        /// </summary>
        public string DestPath { get; set; }


        /// <summary>
        /// 原文件名
        /// </summary>
        public string SrcName
        {
            get
            {
                var match = Regex.Match(SrcPath, "(?<=[\\\\/])[^\\\\/]*$");
                if (match.Success) return match.Value;
                else return "[无法解析文件名]";
            }
            set { }
        }

        /// <summary>
        /// 映射编辑按钮区域宽度
        /// </summary>
        public double EditingAreaWidth { get; set; }

        /// <summary>
        /// 目的文件名
        /// </summary>
        public string DestName
        {
            get
            {
                var match = Regex.Match(DestPath, "(?<=[\\\\/])[^\\\\/]*$");
                if (match.Success)
                {
                    var destFileName = match.Value.Trim();
                    if (destFileName.Length == 0) return SrcName;
                    else return destFileName;
                }
                else return "[无法解析文件名]";
            }
            set { }
        }
    }
}
