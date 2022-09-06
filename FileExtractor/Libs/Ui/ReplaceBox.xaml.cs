using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyTool
{
    /// <summary>
    /// ReplaceBox.xaml 的交互逻辑
    /// </summary>
    public partial class ReplaceBox : Window
    {
        private Action<ReplaceBoxInfo> handleReplace;
        private ReplaceBoxInfo Info { get; set; }
        private ReplaceBox(object initPattern, object initResult, bool? enableRegex, Action<ReplaceBoxInfo> handleReplace, string title)
        {
            InitializeComponent();
            Info = new ReplaceBoxInfo();
            this.handleReplace = handleReplace;
            Info.PatternStr = initPattern == null ? string.Empty : initPattern.ToString();
            Info.ResultStr = initResult == null ? string.Empty : initResult.ToString();
            Info.EnableRegex = enableRegex.GetValueOrDefault(false);
            if (title != null) Title = title;
            Data2View();
        }

        public static void Show(object initPattern, object initResult, bool enableRegex, Action<ReplaceBoxInfo> handleReplace, string title = null)
        {
            new ReplaceBox(initPattern, initResult, enableRegex, handleReplace, title).ShowDialog();
        }

        public static void Show(object initPattern, object initResult, Action<ReplaceBoxInfo> handleReplace, string title = null)
        {
            new ReplaceBox(initPattern, initResult, null, handleReplace, title).ShowDialog();
        }

        public static void Show(Action<ReplaceBoxInfo> handleReplace, string title = null)
        {
            new ReplaceBox(null, null, null, handleReplace, title).ShowDialog();
        }

        private void cb_enableRegex_Click(object sender, RoutedEventArgs e)
        {
            if (!Info.EnableRegex.Equals(cb_enableRegex.IsChecked)) Info.EnableRegex = cb_enableRegex.IsChecked.GetValueOrDefault(false);
        }

        private void Data2View()
        {
            cb_enableRegex.IsChecked = Info.EnableRegex;
            tbx_patternStr.Text = Info.PatternStr;
            tbx_resultStr.Text = Info.ResultStr;
        }

        private void View2Data()
        {
            Info.EnableRegex = cb_enableRegex.IsChecked.GetValueOrDefault(false);
            Info.PatternStr = tbx_patternStr.Text;
            Info.ResultStr = tbx_resultStr.Text;
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            View2Data();
            handleReplace?.Invoke(Info);
            Close();
        }
    }
    public class ReplaceBoxInfo
    {
        public bool EnableRegex { get; set; } = false;
        public string PatternStr { get; set; } = string.Empty;
        public string ResultStr { get; set; } = string.Empty;
    }
}
