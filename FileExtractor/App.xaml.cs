using MyTool;
using NetCore5WpfToolsApp.PrjLogger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileExtractor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            //配置日志
            InitLoggerSystem();

            //加载App配置
            LoadConfigData();

            //初始化缓存管理
            ViewCacheMgrParams.Init((a, b, c) => GlobalConfig.GetCacheFilePath(a, b, c));

            //程序退出事件
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs e)
        {
            ////保存配置
            //var fileInfo = new FileInfo(GlobalConfig.AppCachePath);
            //if (!fileInfo.Directory.Exists)
            //    fileInfo.Directory.Create();
            //if (!fileInfo.Exists)
            //    fileInfo.Create().Dispose();

            ////配置文件更新
            //var fileStream = File.Create(GlobalConfig.AppCachePath);
            //var jsonStr = JsonConvert.SerializeObject(GlobalConfig.AppViewCache, Formatting.Indented);
            //var dataBytes = Encoding.UTF8.GetBytes(jsonStr);
            //fileStream.Write(dataBytes, 0, dataBytes.Length);
            //fileStream.Dispose();
        }

        private static void InitLoggerSystem()
        {
            var configuration = new LoggerConfiguration();
            configuration.LogAppenderList.Add(new RollingDateTimeAppender(GlobalConfig.LogFileDir + "\\${yyyy-MM-dd}", "${yyyy-MM-dd-HH}.txt"));
            LogManager.Config(configuration);
        }

        private static void LoadConfigData()
        {
            ////读取配置
            //if (!File.Exists(GlobalConfig.AppCachePath))
            //{
            //    GlobalConfig.AppViewCache = new AppUserViewCache();
            //    return;
            //}

            //var jsonStr = File.ReadAllText(GlobalConfig.AppCachePath);
            //GlobalConfig.AppViewCache = JsonConvert.DeserializeObject<AppUserViewCache>(jsonStr) ?? new AppUserViewCache();
        }
    }
}
