using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExtractor.Utils
{
    public class ResDicUtils
    {
        /// <summary>
        /// 自定义控件Style存放目录
        /// </summary>
        private const string CUSTOM_CONTROL_STYLE_DIR = "WpfControls/Styles";

        /// <summary>
        /// 获取自定义控件Style
        /// </summary>
        /// <typeparam name="TCustomControl"></typeparam>
        /// <returns></returns>
        public static Style GetCustomControlStyle<TCustomControl>()
        {
            return GetResourceFromCustomControlDefaultResourceDictionary<TCustomControl, Style>(x => x is Style && ((Style)x).TargetType == typeof(TCustomControl));
        }

        /// <summary>
        /// 获取自定义控件的默认资源字典
        /// </summary>
        /// <typeparam name="TCustomControl"></typeparam>
        /// <returns></returns>
        public static ResourceDictionary GetCustomControlDefaultResourceDictionary<TCustomControl>()
        {
            return GetResourceDictionary(CUSTOM_CONTROL_STYLE_DIR + $"/{typeof(TCustomControl).Name}.xaml");
        }

        /// <summary>
        /// 获取自定义控件的默认资源字典中的资源
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TResource GetResourceFromCustomControlDefaultResourceDictionary<TCustomControl, TResource>(Predicate<object> predicate)
        {
            var resDic = GetCustomControlDefaultResourceDictionary<TCustomControl>();
            return (TResource)resDic.Values.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 获取资源字典中的资源
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="xamlRelativePath"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TResource GetResource<TResource>(string xamlRelativePath, Predicate<object> predicate)
        {
            var resDic = GetResourceDictionary(xamlRelativePath);
            return (TResource)resDic.Values.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 获取资源字典对象
        /// </summary>
        /// <param name="xamlRelativePath"></param>
        /// <returns></returns>
        public static ResourceDictionary GetResourceDictionary(string xamlRelativePath)
        {
            xamlRelativePath = xamlRelativePath.Replace("\\", "/").TrimStart('/');
            return new ResourceDictionary()
            {
                Source = new Uri($"pack://application:,,,/{xamlRelativePath}")
            };
        }

        /// <summary>
        /// 获取资源字典对象
        /// </summary>
        /// <param name="xamlRelativePath"></param>
        /// <returns></returns>
        public static ResourceDictionary GetRelativeResourceDictionary(string xamlRelativePath)
        {
            xamlRelativePath = xamlRelativePath.Replace("\\", "/").TrimStart('/');
            var uri = $"./WpfControls/{xamlRelativePath}";
            return new ResourceDictionary()
            {
                Source = new Uri(uri)
            };
        }
    }
}
