using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libs
{
    public class FileUtils
    {
        /// <summary>
        /// 在目录集合中找到第一个存在的文件路径
        /// </summary>
        /// <param name="fileName">目标文件名</param>
        /// <param name="dirPaths">在该目录集合中查找文件</param>
        /// <returns>返回找到存在的第一个文件路径，找不到返回空字符串</returns>
        public static string FindExistsFirstPath(string fileName, string[] dirPaths)
        {
            foreach (var dir in dirPaths)
            {
                var filePath = Path.Combine(dir, fileName);
                if (File.Exists(filePath))
                    return filePath;
            }
            return string.Empty;
        }

        /// <summary>
        /// 创建文件如果不存在
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateFileIfNotExist(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }
                fileInfo.Create().Dispose();
            }
        }

        /// <summary>
        /// 复制整个目录
        /// </summary>
        /// <param name="srcDirPath"></param>
        /// <param name="destDirPath"></param>
        public static void CopyDirRecursively(string srcDirPath, string destDirPath)
        {
            //创建所有新目录
            foreach (string dirPath in Directory.GetDirectories(srcDirPath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(srcDirPath, destDirPath));
            }

            //复制所有文件 & 保持文件名和路径一致
            foreach (string filePath in Directory.GetFiles(srcDirPath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(filePath, filePath.Replace(srcDirPath, destDirPath), true);
            }
        }
    }
}
