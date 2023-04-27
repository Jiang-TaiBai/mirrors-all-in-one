using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mirrors_All_in_One.Common;
using Mirrors_All_in_One.Enums;
using Mirrors_All_in_One.Utils;

namespace Mirrors_All_in_One.Data
{
    /// <summary>
    /// 包管理器数据工具类，用于处理所有包管理器的数据，包括存储、读取、更新等
    /// </summary>
    public class DataPackageManagerUtil
    {
        /// <summary>
        /// 用户数据所在文件夹
        /// </summary>
        public string UserDataPath { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Filename { get; set; } = "PackageManager.json";

        private string AbsolutePath => Path.Combine(UserDataPath, Filename);

        /// <summary>
        /// 所有包管理器的列表
        /// </summary>
        public IList<DataPackageManagerBase> DataPackageManagers { get; private set; }

        public DataPackageManagerUtil(string userDataPath)
        {
            UserDataPath = userDataPath;
            LoadData();
        }

        private bool LoadData()
        {
            string jsonData = "";
            try
            {
                if (File.Exists(AbsolutePath))
                {
                    jsonData = File.ReadAllText(AbsolutePath);
                    // 从 JSON 文件反序列化为对象
                    List<DataPackageManagerBase> data =
                        JsonSerializer.Deserialize<List<DataPackageManagerBase>>(jsonData);
                    DataPackageManagers = data;
                }
                else
                {
                    DataPackageManagers = new List<DataPackageManagerBase>();
                }
            }
            catch (FileNotFoundException)
            {
                DataPackageManagers = new List<DataPackageManagerBase>();
            }
            catch (Exception)
            {
                // throw new Exception("加载数据文件时出错：" + e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 刷新数据，与LoadData同样的目的
        /// 区别一：对外只能刷新，并非初始化加载，命名更友好。
        /// 区别二：LoadData为private，RefreshData为public
        /// </summary>
        /// <returns></returns>
        public bool RefreshData()
        {
            return LoadData();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        private bool SaveData()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            string jsonData =
                JsonSerializer.Serialize(DataPackageManagers, typeof(IList<DataPackageManagerBase>), options);
            try
            {
                FileUtil.CreateDirectoryByFilePath(AbsolutePath);
                StreamWriter sw = new StreamWriter(AbsolutePath);
                sw.WriteLine(jsonData);
                sw.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 将传入的包管理器列表保存到文件
        /// </summary>
        /// <param name="packageManagers"></param>
        /// <returns></returns>
        public bool SaveData(IList<PackageManagerBase> packageManagers)
        {
            DataPackageManagers = new List<DataPackageManagerBase>();
            foreach (PackageManagerBase packageManager in packageManagers)
            {
                DataPackageManagerBase newDataPackageManagerBase =
                    DataPackageManagerBase.ToDataPackageManager(packageManager);
                DataPackageManagers.Add(newDataPackageManagerBase);
            }

            return SaveData();
        }

        /// <summary>
        /// 添加一个包管理器
        /// </summary>
        /// <param name="packageManager"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool AddPackageManager(PackageManagerBase packageManager)
        {
            DataPackageManagerBase newDataPackageManagerBase =
                DataPackageManagerBase.ToDataPackageManager(packageManager);
            DataPackageManagers.Add(newDataPackageManagerBase);
            return SaveData();
        }

        /// <summary>
        /// 修改一个包管理器，根据uuid来判断
        /// </summary>
        /// <param name="packageManager"></param>
        /// <returns></returns>
        public bool ModifyPackageManager(PackageManagerBase packageManager)
        {
            DataPackageManagerBase newDataPackageManagerBase =
                DataPackageManagerBase.ToDataPackageManager(packageManager);
            for (int i = 0; i < DataPackageManagers.Count; i++)
            {
                if (DataPackageManagers[i].Uuid == newDataPackageManagerBase.Uuid)
                {
                    DataPackageManagers[i] = newDataPackageManagerBase;
                    return SaveData();
                }
            }

            return false;
        }
    }

    /// <summary>
    /// 包管理器的基类（仅存储与本软件有关的数据，与实际数据无关，例如存储类型、备注、配置文件的地址等）
    /// </summary>
    [JsonDerivedType(typeof(DataPackageManagerBase), typeDiscriminator: "DataPackageManagerBase")]
    [JsonDerivedType(typeof(DataPackageManagerConda), typeDiscriminator: "DataPackageManagerConda")]
    [JsonDerivedType(typeof(DataPackageManagerNpm), typeDiscriminator: "DataPackageManagerNpm")]
    [JsonDerivedType(typeof(DataPackageManagerPip), typeDiscriminator: "DataPackageManagerPip")]
    public class DataPackageManagerBase
    {
        /// <summary>
        /// 包管理器的唯一性ID
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// 包管理器的类型
        /// </summary>
        public PackageManagerType PackageManagerType { get; set; }

        /// <summary>
        /// 包管理器的备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 提供一个无参构造函数，用于反序列化
        /// </summary>
        public DataPackageManagerBase()
        {
        }

        protected DataPackageManagerBase(string uuid, PackageManagerType packageManagerType, string remark)
        {
            Uuid = uuid;
            PackageManagerType = packageManagerType;
            Remark = remark;
        }

        /// <summary>
        /// 将PackageManagerBase转换为DataPackageManager
        /// </summary>
        /// <param name="packageManager"></param>
        public static DataPackageManagerBase ToDataPackageManager(PackageManagerBase packageManager)
        {
            DataPackageManagerBase newDataPackageManagerBase = null;
            switch (packageManager.Type)
            {
                case PackageManagerType.Conda:
                    newDataPackageManagerBase = new DataPackageManagerConda(packageManager.Uuid,
                        packageManager.Remark, ((PackageManagerConda)packageManager).PropertyPath);
                    break;
                case PackageManagerType.Npm:
                    newDataPackageManagerBase = new DataPackageManagerPip(packageManager.Uuid,
                        packageManager.Remark, ((PackageManagerNpm)packageManager).PropertyPath);
                    break;
                case PackageManagerType.Pip:
                    newDataPackageManagerBase = new DataPackageManagerPip(packageManager.Uuid,
                        packageManager.Remark, ((PackageManagerPip)packageManager).PropertyPath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("镜像类型不正确");
            }

            return newDataPackageManagerBase;
        }
    }

    /// <summary>
    /// Conda的附加属性，虽然实际在使用时是用PackageManagerConda类，但是该类是为了方便存储数据而设计的。
    /// 仅保留与本软件有关的数据，与实际数据无关，例如具体镜像列表、各配置项等
    /// </summary>
    public class DataPackageManagerConda : DataPackageManagerBase
    {
        public string PropertyFilePath { get; set; }

        public DataPackageManagerConda(string uuid, string remark,
            string propertyFilePath) :
            base(uuid, PackageManagerType.Conda, remark)
        {
            PropertyFilePath = propertyFilePath;
        }
    }

    /// <summary>
    /// Pip的附加属性，虽然实际在使用时是用PackageManagerPip类，但是该类是为了方便存储数据而设计的。
    /// 仅保留与本软件有关的数据，与实际数据无关，例如具体镜像列表、各配置项等
    /// </summary>
    public class DataPackageManagerPip : DataPackageManagerBase
    {
        public string PropertyFilePath { get; set; }

        public DataPackageManagerPip(string uuid, string remark,
            string propertyFilePath) : base(uuid, PackageManagerType.Pip, remark)
        {
            PropertyFilePath = propertyFilePath;
        }
    }

    /// <summary>
    /// Npm的附加属性，虽然实际在使用时是用PackageManagerNpm类，但是该类是为了方便存储数据而设计的。
    /// 仅保留与本软件有关的数据，与实际数据无关，例如具体镜像列表、各配置项等
    /// </summary>
    public class DataPackageManagerNpm : DataPackageManagerBase
    {
        public string PropertyFilePath { get; set; }

        public DataPackageManagerNpm(string uuid, string remark,
            string propertyFilePath) : base(uuid, PackageManagerType.Npm, remark)
        {
            PropertyFilePath = propertyFilePath;
        }

        public DataPackageManagerNpm(DataPackageManagerBase dataPackageManagerBase) :
            base(dataPackageManagerBase.Uuid, dataPackageManagerBase.PackageManagerType, dataPackageManagerBase.Remark)
        {
            PropertyFilePath = ((DataPackageManagerNpm)dataPackageManagerBase).PropertyFilePath;
        }
    }
}