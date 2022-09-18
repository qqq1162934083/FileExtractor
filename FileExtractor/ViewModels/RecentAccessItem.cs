using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExtractor.ViewModels
{
    public class RecentAccessItem
    {
        public string FileName { get; set; }
        public string DirPath { get; set; }
        public DateTime AccessTime { get; set; }
        public override string ToString()
        {
            return FileName + "  " + AccessTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
