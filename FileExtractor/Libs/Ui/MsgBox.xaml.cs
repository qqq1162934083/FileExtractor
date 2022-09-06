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
    public enum MsgBoxBtnOption
    {
        Default,
        HasOkBtn,
        HasOkCancelBtn
    }

    /// <summary>
    /// MsgBox.xaml 的交互逻辑
    /// </summary>
    public partial class MsgBox : Window
    {
        private MsgBox(string msg, string title, MsgBoxBtnOption msgBoxBtnOption = MsgBoxBtnOption.Default)
        {
            InitializeComponent();
            Title = title;
            tb_msg.Text = msg;

            switch (msgBoxBtnOption)
            {
                case MsgBoxBtnOption.Default:
                    btn_ok.Visibility = Visibility.Hidden;
                    btn_cancel.Visibility = Visibility.Hidden;
                    break;
                case MsgBoxBtnOption.HasOkBtn:
                    btn_cancel.Visibility = Visibility.Hidden;
                    break;
                case MsgBoxBtnOption.HasOkCancelBtn:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void Show(string msg, string title)
        {
            new MsgBox(msg, title).ShowDialog();
        }
        public static void Show(string msg)
        {
            new MsgBox(msg, string.Empty).ShowDialog();
        }

        public static bool Show(string msg, string title,MsgBoxBtnOption msgBoxBtnOption)
        {
            return new MsgBox(msg, title, msgBoxBtnOption).ShowDialog().GetValueOrDefault();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
