using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Mirrors_All_in_One.Common;
using Mirrors_All_in_One.Data;
using Mirrors_All_in_One.View;

namespace Mirrors_All_in_One.ViewModels
{
    public class PackageManagerCondaMirrorSettingPageViewModel : ViewModelBase
    {
        /// <summary>
        /// 主框的MainWindow，用于访问控件
        /// </summary>
        public MainWindow MainWindow { get; }

        /// <summary>
        /// 名称：Conda软件包镜像仓库
        /// 实现方式：字典，key为镜像的实际值，value为镜像的备注
        /// </summary>
        public ObservableCollection<Mirror> PackageManagerCondaMirrorRepository
        {
            get
            {
                return new ObservableCollection<Mirror>(UserDataUtil.GetInstance()
                    .DataMirrorRepositoryUtil.DataPackageManagerMirrorRepository.CondaMirrorRepository);
            }
            set
            {
                // 修改并保存
                UserDataUtil.GetInstance()
                    .DataMirrorRepositoryUtil.DataPackageManagerMirrorRepository
                    .CondaMirrorRepository = new List<Mirror>(value);
                UserDataUtil.GetInstance().DataMirrorRepositoryUtil.SaveData();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 已选择的镜像，用于保存"待"激活的镜像列表。
        /// 不设置此项，直接从控件中获取
        /// </summary>
        // private ObservableCollection<Mirror> _selectedPackageManagerCondaMirrorOptions =
        //     new ObservableCollection<Mirror>();
        public PackageManagerCondaMirrorSettingPageViewModel(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }


        // public ObservableCollection<Mirror> SelectedPackageManagerCondaMirrorOptions
        // {
        //     get => _selectedPackageManagerCondaMirrorOptions;
        //     set
        //     {
        //         _selectedPackageManagerCondaMirrorOptions = value;
        //         RaisePropertyChanged();
        //     }
        // }
    }
}