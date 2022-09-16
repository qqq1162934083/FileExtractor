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
            return GetResource<Style>(CUSTOM_CONTROL_STYLE_DIR + $"/{typeof(TCustomControl).Name}.xaml", x => x is Style && ((Style)x).TargetType == typeof(TCustomControl));
        }

        /// <summary>
        /// 获取资源字典中的资源
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="xamlRelaticePath"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TResource GetResource<TResource>(string xamlRelaticePath, Predicate<object> predicate)
        {
            var resDic = GetResourceDictionary(xamlRelaticePath);
            return (TResource)resDic.Values.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 获取资源字典对象
        /// </summary>
        /// <param name="xamlRelaticePath"></param>
        /// <returns></returns>
        public static ResourceDictionary GetResourceDictionary(string xamlRelaticePath)
        {
            xamlRelaticePath = xamlRelaticePath.Replace("\\", "/").TrimStart('/');
            return new ResourceDictionary()
            {
                Source = new Uri($"pack://application:,,,/{xamlRelaticePath}")
            };
        }
    }
}
