using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Common.Libs
{
    public static class FileDialogUtils
    {
        public static void SelectFolderByFileDialog(Action<SaveFileDialog> init, Action<SaveFileDialog> handle, bool multiselect = false)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "选择文件夹";
            dialog.Filter = "当前目录|.文件夹";
            dialog.FileName = "当前目录";
            init?.Invoke(dialog);
            var showResult = dialog.ShowDialog();
            if (showResult == true)
            {
                dialog.FileName = Regex.Replace(dialog.FileName, "[\\\\][^\\\\]*$", x => string.Empty);
                handle?.Invoke(dialog);
            }
        }

        public static void SelectFolderByFileDialog(Action<SaveFileDialog> handle, bool multiselect = false) => SelectFolderByFileDialog(null, handle, multiselect);

        public static void SelectFolder(Action<FolderBrowserDialog> handle)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var showResult = dialog.ShowDialog();
                if (showResult == DialogResult.OK)
                {
                    handle?.Invoke(dialog);
                }
            }
        }

        public static void SelectOpenFile(Action<OpenFileDialog> handle, bool multiselect = false) => SelectOpenFile(null, handle, multiselect);

        public static void SelectOpenFile(Action<OpenFileDialog> init, Action<OpenFileDialog> handle, bool multiselect = false)
        {
            var dialog = new OpenFileDialog();
            init?.Invoke(dialog);
            dialog.Multiselect = multiselect;
            var showResult = dialog.ShowDialog();
            if (showResult == true)
            {
                handle?.Invoke(dialog);
            }
        }

        public static void SelectSaveFile(Action<SaveFileDialog> init, Action<SaveFileDialog> handle, bool multiselect = false)
        {
            var dialog = new SaveFileDialog();
            init?.Invoke(dialog);
            var showResult = dialog.ShowDialog();
            if (showResult == true)
            {
                handle?.Invoke(dialog);
            }
        }

        public static void SelectSaveFile(Action<SaveFileDialog> handle, bool multiselect = false) => SelectSaveFile(null, handle, multiselect);
    }
}