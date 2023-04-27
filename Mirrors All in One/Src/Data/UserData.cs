using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Documents;
using Mirrors_All_in_One.Common;
using Mirrors_All_in_One.Enums;
using Mirrors_All_in_One.Utils;
using Mirrors_All_in_One.ViewModels;

namespace Mirrors_All_in_One.Data
{
    /// <summary>
    /// 用于保存用户数据，将镜像仓库以友好地方式保存在本地（用户也可以用文本地形式打开数据文件修改）
    /// 全局唯一
    /// </summary>
    public class UserDataUtil
    {
        private static UserDataUtil _userDataUtil;

        private string _userDataPath;

        public readonly DataMirrorRepositoryUtil DataMirrorRepositoryUtil;

        public readonly DataPackageManagerUtil DataPackageManagerUtil;

        /// <summary>
        /// 单例模式
        /// </summary>
        /// <returns></returns>
        public static UserDataUtil GetInstance()
        {
            if (_userDataUtil == null) _userDataUtil = new UserDataUtil();
            return _userDataUtil;
        }

        /// <summary>
        /// 设置保存数据的所在文件夹
        /// 目前并未提供修改配置文件的文件夹功能，因此先设置为private
        /// </summary>
        /// <param name="userDataPath"></param>
        private void SetUserDataPath(string userDataPath)
        {
            _userDataPath = userDataPath;
            // 更新 包管理器镜像仓库数据
            // 操作1：修改保存地址
            // 操作2：删除原来的数据文件
            // 操作3：重新保存数据
            DataMirrorRepositoryUtil.UserDataPath = userDataPath;
            DataMirrorRepositoryUtil.DeleteDataFile();
            DataMirrorRepositoryUtil.SaveData();

            // 更新 包管理器数据
            DataPackageManagerUtil.UserDataPath = userDataPath;
        }

        /// <summary>
        /// 将Mirror列表转换成字典，key为channel，value为Mirror
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Dictionary<string, Mirror> MirrorListConvertToDictionary(List<Mirror> list)
        {
            Dictionary<string, Mirror> dictionary = new Dictionary<string, Mirror>();
            foreach (Mirror mirror in list)
            {
                dictionary.Add(mirror.Channel, mirror);
            }

            return dictionary;
        }

        private UserDataUtil()
        {
            // 方案一：使用用户的AppData文件夹
            // 优点：可以确保用户有权限访问
            // 缺点：用户可能不知道数据文件在哪里
            // _userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mirrors All in One");

            // 方案二：使用程序所在文件夹
            // 优点：用户知道数据文件在哪里
            // 缺点：用户可能没有权限访问
            _userDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            // 解决方案：如果用户没有权限访问程序所在文件夹，则使用默认数据文件夹
            try
            {
                if (!Directory.Exists(_userDataPath))
                {
                    Directory.CreateDirectory(_userDataPath);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("无法访问数据文件夹，请检查是否有权限访问当前程序所在文件夹", "错误", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                _userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Mirrors All in One");
                // 告知用户已使用默认数据文件夹
                MessageBox.Show($"由于无权限访问当前程序所在文件夹，已使用默认数据文件夹：{_userDataPath}", "提示", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                if (!Directory.Exists(_userDataPath))
                {
                    Directory.CreateDirectory(_userDataPath);
                }
            }

            DataMirrorRepositoryUtil = new DataMirrorRepositoryUtil(_userDataPath);
            DataPackageManagerUtil = new DataPackageManagerUtil(_userDataPath);
        }
    }
}