using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using GalaSoft.MvvmLight;
using Mirrors_All_in_One.Data;
using Mirrors_All_in_One.Enums;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Mirrors_All_in_One.Common
{
    /// <summary>
    /// 包管理工具基本类
    /// </summary>
    public class PackageManagerBase : ViewModelBase
    {
        /// <summary>
        /// 镜像的类型
        /// </summary>
        private PackageManagerType _type;

        /// <summary>
        /// 镜像的备注
        /// </summary>
        private string _remark;

        /// <summary>
        /// 在主面板-左侧栏-已添加的镜像列表中所展示的名称
        /// 1. 若type=Conda，remark=“镜像1”，那么将显示Conda(镜像1)
        /// 2. 若type=Conda，remark=“”，那么将显示Conda。即不会展示后面的备注和多余的括号
        /// </summary>
        private string _displayName;

        public PackageManagerType Type
        {
            get => _type;
            set
            {
                _type = value;
                RaisePropertyChanged();
            }
        }

        public string Remark
        {
            get => _remark;
            set
            {
                _remark = value;
                this.DisplayName = Remark != "" ? $"{Type.ToString()}({Remark})" : Type.ToString();
                RaisePropertyChanged();
            }
        }


        public string DisplayName
        {
            get => Remark != "" ? $"{Type.ToString()}({Remark})" : Type.ToString();
            set
            {
                _displayName = value;
                RaisePropertyChanged();
            }
        }

        protected PackageManagerBase(PackageManagerType type, string remark)
        {
            _type = type;
            _remark = remark;
        }
    }

    /// <summary>
    /// Conda包管理工具信息
    /// </summary>
    public class PackageManagerConda : PackageManagerBase
    {
        /// <summary>
        /// Conda镜像的配置地址，默认为 "C:\Users\{current_username}\.condarc"
        /// </summary>
        private string _propertyPath;

        /// <summary>
        /// 配置文件的状态，三种状态
        /// 1. 文件不存在    => NotFound
        /// 2. 文件存在，解析失败    => Error
        /// 3. 文件存在，解析成功，数据加载成功 => Valid
        /// </summary>
        private CondaPropertyState _propertyFileState;

        /// <summary>
        /// Conda配置项：(list)channels = ["defaults"]
        /// 在.condarc文件中列出通道位置将覆盖conda默认值，从而使conda仅按照给定的顺序搜索这里列出的通道。
        /// Listing channel locations in the .condarc file overrides conda defaults,
        /// causing conda to search only the channels listed here, in the order given.
        /// </summary>
        private ObservableCollection<Mirror> _channels;

        /// <summary>
        /// Conda配置项：(bool)show_channel_urls = false
        /// 在conda列表中显示将要下载的内容时显示通道url。
        /// 默认为False。
        /// Show channel URLs (show_channel_urls)
        /// Show channel URLs when displaying what is going to be downloaded and in conda list. The default is False.
        /// </summary>
        private bool _showChannelUrls = false;

        /// <summary>
        /// Conda配置项：(bool)ssl_verify=true
        /// 默认情况下，此变量为True，这意味着使用SSL验证，conda验证SSL连接的证书。
        /// 将此变量设置为False将禁用连接的正常安全性，不建议这样做
        /// By default this variable is True,
        /// which means that SSL verification is used and conda verifies certificates for SSL connections.
        /// Setting this variable to False disables the connection's normal security and is not recommended.
        /// 备注：该配置项还可设置为证书的字符串路径(string)，但不太常用，因此不进行设置
        /// </summary>
        private bool _sslVerify = true;

        /// <summary>
        /// Conda配置项：(bool)always_yes=false
        /// 当被要求继续时，选择yes选项，例如在安装时。与在命令行中使用——yes标志相同。默认为False。
        /// Choose the yes option whenever asked to proceed, such as when installing. Same as using the --yes flag at the command line. The default is False.
        /// </summary>
        private bool _alwaysYes = false;

        public string PropertyPath
        {
            get => _propertyPath;
            set
            {
                _propertyPath = value;
                RaisePropertyChanged();
            }
        }

        public CondaPropertyState PropertyFileState
        {
            get => _propertyFileState;
            set
            {
                _propertyFileState = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Mirror> Channels
        {
            get
            {
                // 从镜像仓库中加载备注
                List<Mirror> condaMirrorRepository = UserDataUtil.GetInstance().DataPackageManagerMirrorRepositoryUtil
                    .DataPackageManagerMirrorRepository
                    .CondaMirrorRepository;
                Dictionary<string, Mirror> dictionary =
                    UserDataUtil.MirrorListConvertToDictionary(condaMirrorRepository);
                foreach (var channel in _channels)
                {
                    if (dictionary.ContainsKey(channel.Channel))
                    {
                        channel.Remark = dictionary[channel.Channel].Remark;
                    }
                }

                return _channels;
            }
            set
            {
                _channels = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowChannelUrls
        {
            get => _showChannelUrls;
            set
            {
                _showChannelUrls = value;
                RaisePropertyChanged();
            }
        }

        public bool SslVerify
        {
            get => _sslVerify;
            set
            {
                _sslVerify = value;
                RaisePropertyChanged();
            }
        }

        public bool AlwaysYes
        {
            get => _alwaysYes;
            set
            {
                _alwaysYes = value;
                RaisePropertyChanged();
            }
        }

        public PackageManagerConda(string remark = "", string path = "") : base(PackageManagerType.Conda, remark)
        {
            if (path.Equals(""))
            {
                var username = Environment.UserName;
                path = Path.Combine(Path.Combine("C:\\Users\\", username), ".condarc");
            }

            _propertyPath = path;

            // 每次应当从Path中读取，而非上次保存的数据，确保拿去的信息为最新的
            PropertyFileState = LoadProperty();
        }

        /// <summary>
        /// 根据PropertyPath属性，读取配置文件，并更新至类中
        /// </summary>
        /// <returns>
        /// CondaPropertyState类型，需要确保调用此方法要接收解析结果
        /// </returns>
        private CondaPropertyState LoadProperty()
        {
            // 准备工作1：初始化yaml文件解析器
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            // 准备工作2：获取yaml文件内容
            string yamlString = "";
            try
            {
                yamlString = File.ReadAllText(PropertyPath);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return CondaPropertyState.NotFound;
            }

            // 准备工作3：解析yaml文本为字典对象
            Dictionary<string, object> dictionary;
            try
            {
                dictionary = deserializer.Deserialize<Dictionary<string, object>>(yamlString);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return CondaPropertyState.Error;
            }

            // 解析1. 解析channels
            // 首先获取镜像仓库
            UserDataUtil userDataUtil = UserDataUtil.GetInstance();
            DataPackageManagerMirrorRepository dataPackageManagerMirrorRepository =
                userDataUtil.DataPackageManagerMirrorRepositoryUtil.DataPackageManagerMirrorRepository;
            List<Mirror> condaMirrorRepository = dataPackageManagerMirrorRepository.CondaMirrorRepository;
            Dictionary<string, Mirror> condaMirrorRepositoryDictionary =
                UserDataUtil.MirrorListConvertToDictionary(condaMirrorRepository);
            // 如果解析失败，则用官方默认值["default"]代替
            if (dictionary.ContainsKey("channels") && dictionary["channels"] is List<object> channelsInPropertyFile)
            {
                Channels = new ObservableCollection<Mirror>();
                foreach (string channel in channelsInPropertyFile)
                {
                    Channels.Add(new Mirror(channel));
                }
            }
            else
            {
                Channels = new ObservableCollection<Mirror> { new Mirror("default") };
            }

            // 解析2. 解析show_channel_urls
            if (dictionary.ContainsKey("show_channel_urls") &&
                dictionary["show_channel_urls"] is string showChannelUrlsInPropertyFile)
            {
                ShowChannelUrls = Convert.ToBoolean(showChannelUrlsInPropertyFile);
            }
            else
            {
                ShowChannelUrls = false;
            }

            // 解析3. 解析ssl_verify
            if (dictionary.ContainsKey("ssl_verify") && dictionary["ssl_verify"] is string sslVerifyInPropertyFile)
            {
                SslVerify = Convert.ToBoolean(sslVerifyInPropertyFile);
            }
            else
            {
                SslVerify = false;
            }

            // 解析4. 解析always_yes
            if (dictionary.ContainsKey("always_yes") && dictionary["always_yes"] is string alwaysYesInPropertyFile)
            {
                AlwaysYes = Convert.ToBoolean(alwaysYesInPropertyFile);
            }
            else
            {
                AlwaysYes = true;
            }

            // 容许解析失败，使用了官方默认值来代替解析失败的值
            // 解析失败的情况有：属性不存在、属性类型不一致
            return CondaPropertyState.Valid;
        }

        /// <summary>
        /// Conda配置文件的状态枚举类
        /// 1. 文件不存在    => NotFound
        /// 2. 文件存在，解析失败    => Error
        /// 3. 文件存在，解析成功，数据加载成功 => Valid
        /// </summary>
        public enum CondaPropertyState
        {
            /// <summary>
            /// 文件存在，且解析成功
            /// </summary>
            Valid,

            /// <summary>
            /// 文件不存在
            /// </summary>
            NotFound,

            /// <summary>
            /// 文件存在，但解析错误
            /// </summary>
            Error,
        }
    }

    /// <summary>
    /// Npm包管理工具信息
    /// </summary>
    public class PackageManagerNpm : PackageManagerBase
    {
        /// <summary>
        /// Conda镜像的配置地址，默认为 "C:\Users\{current_username}\.npmrc"
        /// </summary>
        private string _propertyPath;

        public string PropertyPath
        {
            get => _propertyPath;
            set
            {
                _propertyPath = value;
                RaisePropertyChanged();
            }
        }

        public PackageManagerNpm(string remark = "", string path = "") : base(PackageManagerType.Npm, remark)
        {
            if (path.Equals(""))
            {
                var username = Environment.UserName;
                path = Path.Combine(Path.Combine("C:\\Users\\", username), ".npmrc");
            }

            _propertyPath = path;
        }
    }

    /// <summary>
    /// Pip包管理工具信息
    /// </summary>
    public class PackageManagerPip : PackageManagerBase
    {
        public PackageManagerPip(string remark = "") : base(PackageManagerType.Pip, remark)
        {
        }
    }
}