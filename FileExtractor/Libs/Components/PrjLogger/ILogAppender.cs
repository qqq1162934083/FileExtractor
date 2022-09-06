using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore5WpfToolsApp.PrjLogger
{
    public interface ILogAppender
    {
        void Log(string msg, LogLevel logLevel, string guid = null);
    }
}
