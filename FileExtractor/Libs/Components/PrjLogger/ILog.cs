using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore5WpfToolsApp.PrjLogger
{
    public interface ILog
    {
        string GetLoggerName();
        void Info(string msg);
        void Debug(string msg);
        void Error(string msg);
        void Warn(string msg);
        void Log(string msg, LogLevel level, string guid);
    }
}
