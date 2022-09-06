using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetCore5WpfToolsApp.PrjLogger
{
    public class RollingDateTimeAppender : ILogAppender
    {
        private ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();
        private string _fileNamePattern;
        private string _dirNamePattern;

        public RollingDateTimeAppender(string dirNamePattern, string fileNamePattern)
        {
            _fileNamePattern = fileNamePattern;
            _dirNamePattern = dirNamePattern;
        }

        public void Log(string msg, LogLevel logLevel, string guid = null)
        {
            var nowTime = DateTime.Now;
            try
            {
                var dirName = ParseDateTimePattenStr(_dirNamePattern, nowTime);
                var fileName = ParseDateTimePattenStr(_fileNamePattern, nowTime);
                _readerWriterLockSlim.EnterWriteLock();

                //判断文件是否存在
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                if (!File.Exists(fileName))
                {
                    File.Create(fileName);
                }
                var filePath = Path.Combine(dirName, fileName);

                //信息
                string msgResult;
                if (guid != null)
                    msgResult = $"[{nowTime:yyy-MM-dd HH:mm:ss}] [{logLevel}] [{guid}] - {msg}";
                else
                    msgResult = $"[{nowTime:yyy-MM-dd HH:mm:ss}] [{logLevel}] - {msg}";

                //执行日志写入
                StreamWriter writer = new StreamWriter(filePath, true);
                writer.WriteLine(msgResult);
                writer.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }
        }

        private string ParseDateTimePattenStr(string oStr, DateTime time)
        {
            string str = oStr;
            while (true)
            {
                var match = Regex.Match(str, "\\$\\{.+?\\}");
                if (!match.Success) break;
                //var pattern = match.Value[2..^1];
                var pattern = match.Value.Substring(2, match.Value.Length - 3);
                //str = str[..match.Index] + time.ToString(pattern) + str[(match.Index + match.Length)..];
                str = str.Substring(0, match.Index) + time.ToString(pattern) + str.Substring(match.Index + match.Length);
            }
            return str;
        }
    }
}
