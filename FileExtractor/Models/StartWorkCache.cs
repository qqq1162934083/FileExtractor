using FileExtractor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExtractor.Models
{
    public class StartWorkCache
    {
        /// <summary>
        /// 最近访问的项目
        /// </summary>
        public List<RecentAccessItem> RecenteAccessItemList { get; set; }
        /// <summary>
        /// 配置文件所在目录，方便快速加载
        /// </summary>
        public string ConfigDirPath { get; set; }
    }
}
