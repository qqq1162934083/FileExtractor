using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCore5WpfToolsApp.PrjLogger
{
    internal class Logger : ILog
    {
        private List<ILogAppender> LogAppenderList { get; set; } = new List<ILogAppender>();
        private string Name { get; set; }

        private LoggerConfiguration Configuration { get; set; }
        public Logger(string name, LoggerConfiguration configuration)
        {
            Name = name;
            Configuration = configuration;
            LoadConfig();
        }

        public string GetLoggerName()
        {
            return Name;
        }

        private void LoadConfig()
        {
            LogAppenderList = Configuration.LogAppenderList;
        }

        public void Debug(string msg) => WriteLog(msg, LogLevel.Debug);

        public void Error(string msg) => WriteLog(msg, LogLevel.Error);

        public void Info(string msg) => WriteLog(msg, LogLevel.Info);

        public void Warn(string msg) => WriteLog(msg, LogLevel.Warn);

        private void WriteLog(string msg, LogLevel logLevel, string guid = null)
        {
            if (LogAppenderList != null && LogAppenderList.Count > 0)
            {
                LogAppenderList.ForEach(x =>
                {
                    try { x.Log(msg, logLevel, guid); }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
                });
            }
        }

        public void Log(string msg, LogLevel level, string guid)
        {
            WriteLog(msg, level, guid);
        }
    }
}
