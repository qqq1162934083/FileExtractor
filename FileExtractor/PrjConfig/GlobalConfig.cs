using System;
using System.IO;

namespace Common
{
    public static class GlobalConfig
    {
        #region 应用配置
        public readonly static string AppName = "FileExtractor";
        #endregion

        #region 路径配置

        //日志目录
        public static string LogFileDir { get; set; }

        //缓存目录
        //缓存目录按照具体功能或者窗体分文件夹归类，文件夹下包括ViewCache和DataCache两个文件夹分别存放视图缓存和数据缓存
        public static string AppCacheDir { get; set; }

        //临时内容目录
        public static string AppTmpDir { get; set; }

        //应用数据根目录
        public static string AppDataRootDir { get; set; }

        #endregion

        static GlobalConfig()
        {
            #region 配置路径
            //应用根路径
            AppDataRootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
            //日志路径
            LogFileDir = Path.Combine(AppDataRootDir, "AppLogs");
            //缓存目录
            AppCacheDir = Path.Combine(AppDataRootDir, "AppCaches");
            //临时目录
            AppTmpDir = Path.Combine(AppDataRootDir, "Temp");
            #endregion
        }

        /// <summary>
        /// 获取缓存文件路径
        /// </summary>
        /// <param name="cacheGroupName">缓存分组名/文件夹名</param>
        /// <param name="cacheName">缓存文件名</param>
        /// <returns></returns>
        public static string GetCacheFilePath(CacheType cacheType, string cacheGroupName, string cacheName)
        {
            return Path.Combine(GetCacheDirPath(cacheType, cacheGroupName), cacheName + ".json");
        }

        /// <summary>
        /// 获取缓存目录
        /// </summary>
        /// <param name="cacheGroupName">缓存分组名/文件夹名</param>
        /// <returns></returns>
        public static string GetCacheDirPath(CacheType cacheType, string cacheGroupName)
        {
            switch (cacheType)
            {
                case CacheType.DataCache:
                    return Path.Combine(AppCacheDir, cacheGroupName, "DataCaches");
                case CacheType.ViewCache:
                    return Path.Combine(AppCacheDir, cacheGroupName, "ViewCaches");
                default:
                    throw new Exception("不在预期的缓存类型");
            }
        }
    }
}