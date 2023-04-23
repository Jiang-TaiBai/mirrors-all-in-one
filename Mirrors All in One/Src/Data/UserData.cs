using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Documents;
using Mirrors_All_in_One.Common;
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

        public readonly DataPackageManagerMirrorRepositoryUtil DataPackageManagerMirrorRepositoryUtil;

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
        /// </summary>
        /// <param name="userDataPath"></param>
        public void SetUserDataPath(string userDataPath)
        {
            _userDataPath = userDataPath;
            // 更新 包管理器镜像仓库数据
            // 操作1：修改保存地址
            // 操作2：删除原来的数据文件
            // 操作3：重新保存数据
            DataPackageManagerMirrorRepositoryUtil.UserDataPath = userDataPath;
            DataPackageManagerMirrorRepositoryUtil.DeleteDataFile();
            DataPackageManagerMirrorRepositoryUtil.SaveData();
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
            _userDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            DataPackageManagerMirrorRepositoryUtil = new DataPackageManagerMirrorRepositoryUtil(_userDataPath);
        }
    }

    /// <summary>
    /// 序列化仓库数据工具
    /// </summary>
    public class DataPackageManagerMirrorRepositoryUtil
    {
        /// <summary>
        /// 用户保存数据路径
        /// </summary>
        public string UserDataPath;

        /// <summary>
        /// 仓库数据
        /// </summary>
        public DataPackageManagerMirrorRepository DataPackageManagerMirrorRepository =
            new DataPackageManagerMirrorRepository();

        /// <summary>
        /// 用户保存数据的文件名
        /// </summary>
        private readonly string _filename = "MirrorsAllInOneDataFile-PackageManagerMirrorRepository.txt";

        private string AbsolutePath => Path.Combine(UserDataPath, _filename);

        public DataPackageManagerMirrorRepositoryUtil(string userDataPath)
        {
            ChangeUserDataPath(userDataPath);
        }

        /// <summary>
        /// 改变用户数据所在文件夹，并且自动更新数据
        /// </summary>
        /// <param name="userDataPath">用户数据所在文件夹</param>
        private void ChangeUserDataPath(string userDataPath)
        {
            UserDataPath = userDataPath;
            LoadData();
        }

        /// <summary>
        /// 从数据文件中加载数据
        /// </summary>
        private void LoadData()
        {
            string jsonData = "";
            try
            {
                jsonData = File.ReadAllText(AbsolutePath);
                // 从 JSON 文件反序列化为对象
                DataPackageManagerMirrorRepository data =
                    JsonSerializer.Deserialize<DataPackageManagerMirrorRepository>(jsonData);
                if (data != null)
                {
                    DataPackageManagerMirrorRepository = data;
                }
                else
                {
                    DataPackageManagerMirrorRepository = new DataPackageManagerMirrorRepository();
                }
            }
            catch (FileNotFoundException)
            {
                DataPackageManagerMirrorRepository = new DataPackageManagerMirrorRepository();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "获取数据文件时错误");
            }
        }

        /// <summary>
        /// 序列化镜像仓库到文件
        /// </summary>
        public void SaveData()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            Dictionary<string, List<SerializableMirror>> data = new Dictionary<string, List<SerializableMirror>>()
            {
                { "CondaMirrorRepository", new List<SerializableMirror>() },
                { "NpmMirrorRepository", new List<SerializableMirror>() },
                { "PipMirrorRepository", new List<SerializableMirror>() },
            };
            foreach (Mirror mirror in DataPackageManagerMirrorRepository.CondaMirrorRepository)
            {
                data["CondaMirrorRepository"].Add(new SerializableMirror(mirror));
            }

            foreach (Mirror mirror in DataPackageManagerMirrorRepository.NpmMirrorRepository)
            {
                data["CondaMirrorRepository"].Add(new SerializableMirror(mirror));
            }

            foreach (Mirror mirror in DataPackageManagerMirrorRepository.PipMirrorRepository)
            {
                data["CondaMirrorRepository"].Add(new SerializableMirror(mirror));
            }

            string jsonData = JsonSerializer.Serialize(data, options);
            try
            {
                StreamWriter sw = new StreamWriter(Path.Combine(UserDataPath, _filename));
                sw.WriteLine(jsonData);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, "存储数据文件时错误");
            }
        }

        /// <summary>
        /// 从数据文件中加载数据，与LoadData同样的目的
        /// 区别一：对外只能刷新，并非初始化加载，命名更友好。
        /// 区别二：LoadData为private，RefreshData为public
        /// </summary>
        public void RefreshData()
        {
            LoadData();
        }

        /// <summary>
        /// 删除数据文件
        /// </summary>
        public void DeleteDataFile()
        {
            // 1、首先判断文件或者文件路径是否存在
            if (File.Exists(AbsolutePath))
            {
                // 2、根据路径字符串判断是文件还是文件夹
                FileAttributes attr = File.GetAttributes(AbsolutePath);
                // 3、确认是文件才删除
                if (attr != FileAttributes.Directory)
                {
                    File.Delete(AbsolutePath);
                }
            }
        }

        class SerializableMirror
        {
            public string Channel { get; set; }
            public string Remark { get; set; }

            public SerializableMirror(Mirror mirror)
            {
                Channel = mirror.Channel;
                Remark = mirror.Remark;
            }
        }
    }

    /// <summary>
    /// 所有包管理器的镜像数据
    /// </summary>
    public class DataPackageManagerMirrorRepository
    {
        /// <summary>
        /// Conda的镜像仓库列表
        /// </summary>
        public List<Mirror> CondaMirrorRepository { get; set; } = new List<Mirror>();

        /// <summary>
        /// Npm的镜像仓库列表
        /// </summary>
        public List<Mirror> NpmMirrorRepository { get; set; } = new List<Mirror>();

        /// <summary>
        /// Pip的镜像仓库列表
        /// </summary>
        public List<Mirror> PipMirrorRepository { get; set; } = new List<Mirror>();
    }
}