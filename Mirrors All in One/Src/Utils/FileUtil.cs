using System;
using System.IO;

namespace Mirrors_All_in_One.Utils
{
    /// <summary>
    /// 文件工具类
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// 根据文件路径创建文件所在的目录
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void CreateDirectoryByFilePath(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath == null) throw new ArgumentException("文件路径不合法");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}