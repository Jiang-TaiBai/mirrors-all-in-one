using System.Text.Json.Serialization;
using GalaSoft.MvvmLight;

namespace Mirrors_All_in_One.Common
{
    /// <summary>
    /// 镜像信息，一般是通用的，由一个字符串和一个备注
    /// </summary>
    public class Mirror : ViewModelBase
    {
        /// <summary>
        /// 镜像的地址，在Conda中可以是Url，也可以是File本地地址，故此处命名为更为通用的channel
        /// </summary>
        private string _channel;

        /// <summary>
        /// 该镜像的备注，默认为空
        /// </summary>
        private string _remark;

        private string _displayName;

        /// <summary>
        /// 所展示给用户的名称：(备注)镜像路径
        /// 如果备注为空，则为：镜像路径
        /// </summary>
        public string DisplayName
        {
            get => Remark.Trim() != "" ? $"({Remark}){Channel}" : Channel;
            set
            {
                _displayName = value;
                RaisePropertyChanged();
            }
        }

        public string Channel
        {
            get => _channel == null ? "" : _channel.Trim();
            set
            {
                _channel = value;
                DisplayName = Remark.Trim() != "" ? $"({Remark}){Channel}" : Channel;
                RaisePropertyChanged();
            }
        }

        public string Remark
        {
            get => _remark == null ? "" : _remark.Trim();
            set
            {
                _remark = value;
                DisplayName = Remark.Trim() != "" ? $"({Remark}){Channel}" : Channel;
                RaisePropertyChanged();
            }
        }

        public Mirror(string channel, string remark = "")
        {
            Channel = channel;
            Remark = remark;
            DisplayName = Remark.Trim() != "" ? $"({Remark}){Channel}" : Channel;
        }
    }
}