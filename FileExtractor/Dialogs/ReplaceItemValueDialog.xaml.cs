using FileExtractor.Models;
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

namespace FileExtractor.Dialogs
{
    /// <summary>
    /// AddItemDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ReplaceItemValueDialog : Window
    {
        /// <summary>
        /// 指示当前窗体为哪个功能服务：0添加文件映射、1添加目录映射、2添加环境变量
        /// </summary>
        public int FuncIndex { get; set; }
        private ReplaceItemValueDialog(int funcIndex)
        {
            FuncIndex = funcIndex;
            switch (funcIndex)
            {
                case 0: tb_title.Text = "批量替换-文件映射"; break;
                case 1: tb_title.Text = "批量替换-目录映射"; break;
                case 2: tb_title.Text = "批量替换-环境变量"; break;
                default:throw new Exception("未在预期范围的值");
            }
            InitializeComponent();
        }
        public static void ShowDialog(int funcIndex, Action<ItemInfoDialog> initHandle, Action<ItemInfoDialog> handle)
        {
            var dialog = new ItemInfoDialog(funcIndex);
            initHandle?.Invoke(dialog);
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                handle?.Invoke(dialog);
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
