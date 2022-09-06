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
    /// VariableBox.xaml 的交互逻辑
    /// </summary>
    public partial class VariableBox : Window
    {
        private Action<string> handleModificationAction;
        private VariableBox(object initValue, Action<string> handleModification, string title)
        {
            InitializeComponent();
            handleModificationAction = handleModification;
            tb_destVarValue.Text = initValue == null ? string.Empty : initValue.ToString();
            if (title != null) Title = title;
        }
        public static void Show(object initValue, Action<string> handleModification, string title = null)
        {
            new VariableBox(initValue, handleModification, title).ShowDialog();
        }
        public static void Show(Action<string> handleModification, string title = null)
        {
            new VariableBox(string.Empty, handleModification, title).ShowDialog();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            handleModificationAction?.Invoke(tb_destVarValue.Text);
            Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
