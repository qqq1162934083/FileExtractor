using FileExtractor.ViewModels;
using MyTool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExtractor.Models
{
    public class WorkData
    {
        public RecentAccessItem AccessItemInfo { get; set; }
        internal ConfigData ConfigData { get; set; }
        //public ExtractedDirViewModel Data { get; set; }
    }
}
