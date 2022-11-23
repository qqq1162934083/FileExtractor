//using AutoUpdaterDotNET;
//using MyTool;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Common
//{
//    public class AppUpdater
//    {
//        private static System.Timers.Timer Timer { get; set; }

//        public static Form CurrentForm { get; set; }

//        static AppUpdater()
//        {
//            AutoUpdater.AppTitle = GlobalConfig.ShortCutName;
//            AutoUpdater.InstalledVersion = new Version(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
//            AutoUpdater.ShowSkipButton = true;
//            AutoUpdater.ShowRemindLaterButton = true; 
//            //AutoUpdater.Mandatory = false;//区别待验证
//            //AutoUpdater.Mandatory = true;
//            //AutoUpdater.UpdateMode = Mode.Forced;
//            //AutoUpdater.HttpUserAgent = "AutoUpdater";
//            //AutoUpdater.ReportErrors = true;//没有可用更新或者其他错误时报告
//            AutoUpdater.ReportErrors = false;
//            //AutoUpdater.RunUpdateAsAdmin = false;
//            //AutoUpdater.OpenDownloadPage = true;//更新前显示下载页
//            //AutoUpdater.LetUserSelectRemindLater = false;
//            //AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
//            //AutoUpdater.RemindLaterAt = 2;
//            //var currentDirectory = new DirectoryInfo(Application.StartupPath);
//            //AutoUpdater.InstallationPath = currentDirectory.Parent.FullName;//指定下载的位置
//            AutoUpdater.RunUpdateAsAdmin = true;
//            //指定更新覆盖位置
//            //AutoUpdater.DownloadPath 
//            //清理安装目录
//            AutoUpdater.ClearAppDirectory = true;
//            //更新窗体大小
//            AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600);
//            //应用如何退出
//            AutoUpdater.ApplicationExitEvent += delegate
//            {
//                Application.Exit();
//            };

//            //手动处理更新逻辑,会覆盖默认弹出对话框
//            //AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;
//            //解析更新信息逻辑,比如使用json更新文件
//            //AutoUpdater.ParseUpdateInfoEvent+=
//        }

//        public static void Start()
//        {
//            Stop();

//            lock (typeof(AppUpdater))
//            {
//                Timer = new System.Timers.Timer
//                {
//                    Interval = 5 * 1000,
//                    SynchronizingObject = CurrentForm
//                };
//                Timer.Elapsed += delegate
//                {
//                    CheckUpdate();
//                };
//                Timer.Start();
//            }
//        }
        
//        public static void Stop()
//        {
//            lock (typeof(AppUpdater))
//            {
//                if (Timer != null)
//                {
//                    Timer.Stop();
//                    Timer.Dispose();
//                    Timer = null;
//                }
//            }
//        }

//        public static void CheckUpdate()
//        {
//            AutoUpdater.Start("http://127.0.0.1:6546/ml.xml");
//            //AutoUpdater.Start("ftp://rbsoft.org/updates/AutoUpdaterTest.xml", new NetworkCredential("FtpUserName", "FtpPassword"));
//        }
//    }
//}
