using MyTool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTool.Modules.Module_FileExtractor
{
    public class FileExtractorDataCache
    {
        /// <summary>
        /// 可选择的配置文件列表目录
        /// </summary>
        public string ConfigFileListDirPath { get; set; } = string.Empty;
        public string DestDirPath { get; set; } = string.Empty;
        public string DestFolderName { get; set; } = string.Empty;
        public bool EnableHandleTimeExpression { get; set; } = false;
        public bool EnableCompress { get; set; } = true;
        public List<ExtractedFile> ExtractedFileInfoList { get; set; } = new List<ExtractedFile>();
        public List<ExtractedDir> ExtractedDirInfoList { get; set; } = new List<ExtractedDir>();
    }
}
