using FileExtractor.ViewModels;
using Common.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExtractor.Models
{
    public class WorkData
    {
        public RecentAccessItem AccessItemInfo { get; set; }
        public ConfigData ConfigData { get; set; }

        //public ExtractedDirViewModel Data { get; set; }

        /// <summary>
        /// 加载配置数据
        /// </summary>
        public void LoadConfigData()
        {
            var configContent = File.ReadAllText(AccessItemInfo.FilePath);
            try
            {
                ConfigData = JsonConvert.DeserializeObject<ConfigData>(configContent);
            }
            catch
            {
                throw new Exception("无效的配置文件");
            }
        }

        /// <summary>
        /// 保存配置数据
        /// </summary>
        /// <param name="savePath"></param>
        public void SaveConfigData(string savePath = null)
        {
            savePath = savePath ?? AccessItemInfo.FilePath;
            if (string.IsNullOrWhiteSpace(savePath))
                throw new Exception("无效的保存路径");
            var dataJson = JsonConvert.SerializeObject(ConfigData, Formatting.Indented);
            if (!File.Exists(savePath))
                File.Create(savePath).Dispose();
            File.WriteAllText(savePath, dataJson);
        }
    }
}
