using FileExtractor.ViewModels;
using Newtonsoft.Json;
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
        public List<RecentAccessItem> RecentAccessItemList
        {
            get
            {
                if (_recentAccessItemList == null)
                    _recentAccessItemList = new List<RecentAccessItem>();
                return _recentAccessItemList;
            }
            set
            {
                _recentAccessItemList = value;
            }
        }

        [JsonIgnore]
        public List<RecentAccessItem> _recentAccessItemList;

        /// <summary>
        /// 配置文件所在目录，方便快速加载
        /// </summary>
        public string ConfigDirPath { get; set; }

        public void UpdateRecentAccessItem(RecentAccessItem recentAccessItem)
        {
            var item = App.Cache.StartWorkCache.RecentAccessItemList.FirstOrDefault(x => x.DirPath == recentAccessItem.DirPath && recentAccessItem.FileName == x.FileName);
            if (item == null)
            {
                //创建
                item = recentAccessItem;
                App.Cache.StartWorkCache.RecentAccessItemList.Add(item);
            }
            else
            {
                //更新
                item.AccessTime = recentAccessItem.AccessTime;
            }
            App.Cache.StartWorkCache.RecentAccessItemList = App.Cache.StartWorkCache.RecentAccessItemList.OrderByDescending(x => x.AccessTime).ToList();//排序
            App.Cache.StartWorkCacheMgr.NotifySave();//保存
        }
    }
}
