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
    public partial class ValueBox : Window
    {
        public string SrcValue;
        private ValueBox()
        {
            InitializeComponent();
        }
        public static void Show(string tipText, string initValue, Action<string, string> handle)
        {
            var dialog = new ValueBox();
            dialog.SrcValue = initValue;
            dialog.tb_tipText.Text = tipText;
            dialog.tbx_value.Text = dialog.SrcValue;
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                handle?.Invoke(dialog.SrcValue, dialog.tbx_value.Text);
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
