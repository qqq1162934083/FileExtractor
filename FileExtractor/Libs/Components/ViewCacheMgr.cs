using MyTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyTool
{

    public class ViewCacheMgr<TBind, TDataCache, TViewCache> : IConfigControlCache
        where TBind : class
        where TDataCache : new()
        where TViewCache : new()
    {
        /// <summary>
        /// 视图的数据缓存
        /// </summary>
        public TDataCache DataCache { get; set; }
        /// <summary>
        /// 视图的视图设置缓存
        /// </summary>
        public TViewCache ViewCache { get; set; }

        /// <summary>
        /// 一般绑定类型
        /// </summary>
        protected TBind BindObj { get; set; }

        /// <summary>
        /// 如果绑定的对象实现IConfigControlCache则为视图
        /// </summary>
        protected IConfigControlCache BindView { get; set; }

        /// <summary>
        /// 缓存组名
        /// </summary>
        protected string GroupName { get; set; }
        /// <summary>
        /// 数据缓存名
        /// </summary>
        protected string DataCacheName { get; set; }
        /// <summary>
        /// 视图缓存名
        /// </summary>
        protected string ViewCacheName { get; set; }

        public ViewCacheMgr() : this(null)
        {

        }
        public ViewCacheMgr(string groupName, string dataCacheName, string viewCacheName) : this(null, groupName, dataCacheName, viewCacheName)
        {
        }
        public ViewCacheMgr(TBind bindObj) : this(bindObj, null, null, null)
        {
        }
        public ViewCacheMgr(TBind bindObj, string groupName, string dataCacheName, string viewCacheName)
        {
            //检查必要参数
            if (!ViewCacheMgrParams.Initialized)
                throw new Exception("你还没有初始化，请调用静态 Init 方法初始化");

            var type = typeof(TBind);
            BindObj = bindObj;
            if (BindObj != null && typeof(IConfigControlCache).IsAssignableFrom(type))
                BindView = (IConfigControlCache)BindObj;
            GroupName = groupName ?? type.Name;
            DataCacheName = dataCacheName ?? type.Name;
            ViewCacheName = viewCacheName ?? type.Name;

            LoadDataCache();
            LoadViewCache();

            if (DataCache == null) DataCache = new TDataCache();
            if (ViewCache == null) ViewCache = new TViewCache();

        }

        public void NotifyLoad()
        {
            BindView?.LoadDataCache();
            BindView?.LoadViewCache();
        }
        public void NotifySave()
        {
            BindView?.ApplyDataCache();
            BindView?.ApplyViewCache();
            ApplyDataCache();
            ApplyViewCache();
        }


        public void LoadDataCache()
        {
            var path = ViewCacheMgrParams.GetCacheFilePathFunc.Invoke(CacheType.DataCache, GroupName, DataCacheName);
            FileUtils.CreateFileIfNotExist(path);
            DataCache = ConfigUtils.ReadJsonConfig<TDataCache>(path);
        }
        public virtual void ApplyDataCache()
        {
            var path = ViewCacheMgrParams.GetCacheFilePathFunc.Invoke(CacheType.DataCache, GroupName, DataCacheName);
            FileUtils.CreateFileIfNotExist(path);
            ConfigUtils.WriteJsonConfig(path, DataCache);
        }
        public void LoadViewCache()
        {
            var path = ViewCacheMgrParams.GetCacheFilePathFunc.Invoke(CacheType.ViewCache, GroupName, ViewCacheName);
            FileUtils.CreateFileIfNotExist(path);
            ViewCache = ConfigUtils.ReadJsonConfig<TViewCache>(path);
        }
        public void ApplyViewCache()
        {
            var path = ViewCacheMgrParams.GetCacheFilePathFunc.Invoke(CacheType.ViewCache, GroupName, ViewCacheName);
            FileUtils.CreateFileIfNotExist(path);
            ConfigUtils.WriteJsonConfig(path, ViewCache);
        }
    }

    public static class ViewCacheMgrParams
    {
        public static bool Initialized => _initialized;
        private static bool _initialized = false;
        //获取缓存路径方法，入参分别是CacheType,GroupName,ViewCacheName
        public static Func<CacheType, string, string, string> GetCacheFilePathFunc => _getCacheFilePathFunc;
        private static Func<CacheType, string, string, string> _getCacheFilePathFunc;

        /// <summary>
        /// 初始化该类
        /// </summary>
        /// <param name="getCacheFilePathFunc"></param>
        public static void Init(Func<CacheType, string, string, string> getCacheFilePathFunc)
        {
            _getCacheFilePathFunc = getCacheFilePathFunc;
            _initialized = true;
        }
    }


    /// <summary>
    /// 配置数据缓存
    /// </summary>
    public interface IConfigDataCache
    {
        void LoadDataCache();
        void ApplyDataCache();
    }

    /// <summary>
    /// 配置视图缓存
    /// </summary>
    public interface IConfigViewCache
    {
        void LoadViewCache();
        void ApplyViewCache();
    }
    public interface IConfigControlCache : IConfigViewCache, IConfigDataCache
    {
    }
    public enum CacheType
    {
        DataCache,
        ViewCache
    }
}
