using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using Mirrors_All_in_One.Common;
using Mirrors_All_in_One.Utils;

namespace Mirrors_All_in_One.Data
{
    /// <summary>
    /// 序列化仓库数据工具
    /// </summary>
    public class DataMirrorRepositoryUtil
    {
        /// <summary>
        /// 用户保存数据路径
        /// </summary>
        public string UserDataPath;

        /// <summary>
        /// 用户保存数据的文件名
        /// </summary>
        public readonly string Filename = "MirrorRepository.json";

        private string AbsolutePath => Path.Combine(UserDataPath, Filename);

        /// <summary>
        /// 仓库数据
        /// </summary>
        public DataPackageManagerMirrorRepository DataPackageManagerMirrorRepository =
            new DataPackageManagerMirrorRepository();


        public DataMirrorRepositoryUtil(string userDataPath)
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
            if (!LoadData())
            {
                // 告知用户加载镜像仓库数据失败，请检查数据文件是否被占用
                MessageBox.Show("加载镜像仓库数据失败，请检查数据文件是否被占用", "错误", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 从数据文件中加载数据
        /// </summary>
        private bool LoadData()
        {
            string jsonData = "";
            try
            {
                if (File.Exists(AbsolutePath))
                {
                    jsonData = File.ReadAllText(AbsolutePath);
                    // 从 JSON 文件反序列化为对象
                    DataPackageManagerMirrorRepository data =
                        JsonSerializer.Deserialize<DataPackageManagerMirrorRepository>(jsonData);
                    DataPackageManagerMirrorRepository = data ?? new DataPackageManagerMirrorRepository();
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
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 序列化镜像仓库到文件
        /// </summary>
        public void SaveData()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

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
                FileUtil.CreateDirectoryByFilePath(AbsolutePath);
                StreamWriter sw = new StreamWriter(AbsolutePath);
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
        public bool RefreshData()
        {
            return LoadData();
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