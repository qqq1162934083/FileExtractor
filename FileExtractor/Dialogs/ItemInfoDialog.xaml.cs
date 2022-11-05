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
    public partial class ItemInfoDialog : Window
    {
        /// <summary>
        /// 指示当前窗体为哪个功能服务：0添加文件映射、1添加目录映射、2添加环境变量
        /// </summary>
        public int FuncIndex { get; set; }
        private ItemInfoDialog(int funcIndex)
        {
            FuncIndex = funcIndex;
            InitializeComponent();
        }
        public static void ShowDialog(int funcIndex, Action<ItemInfoDialog> handle)
        {
            var dialog = new ItemInfoDialog(funcIndex);
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                handle(dialog);
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
