using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore5WpfToolsApp.PrjLogger
{
    public static class LogManager
    {
        private static LoggerConfiguration LoggerConfiguration { get; set; }

        private static List<ILog> LoggerList = new List<ILog>();

        public static void Config(LoggerConfiguration loggerConfiguration)
        {
            LoggerConfiguration = loggerConfiguration;
        }

        public static ILog GetLogger(string name)
        {
            if (LoggerConfiguration == null)
                throw new Exception("请先进行日志配置");
            name = name.Trim();
            //找到Logger，如果没有则创建一个
            var Logger = LoggerList.FirstOrDefault(x => x.GetLoggerName().Equals(name));
            if (Logger == null)
            {
                LoggerList.Add(Logger = new Logger(name, LoggerConfiguration));
            }
            return Logger;
        }

        public static ILog GetLogger(Type type)
        {
            return GetLogger(type.Name);
        }
    }
}
