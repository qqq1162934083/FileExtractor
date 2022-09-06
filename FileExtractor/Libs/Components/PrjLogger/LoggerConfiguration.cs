using System;
using System.Collections.Generic;

namespace NetCore5WpfToolsApp.PrjLogger
{
    public class LoggerConfiguration
    {
        public List<ILogAppender> LogAppenderList { get; set; } = new List<ILogAppender>();
    }
}