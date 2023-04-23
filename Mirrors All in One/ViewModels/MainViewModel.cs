using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using HandyControl.Controls;
using Mirrors_All_in_One.Common;
using Mirrors_All_in_One.Enums;
using Mirrors_All_in_One.Utils;
using MessageBox = HandyControl.Controls.MessageBox;

namespace Mirrors_All_in_One.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private MainWindow MainWindow { get; }

        /// <summary>
        /// 所有已经添加的包管理工具
        /// </summary>
        private ObservableCollection<PackageManagerBase> _packageManagerList;

        /// <summary>
        /// 当前所选择的包管理工具，在PackageManagerList的位置索引
        /// </summary>
        private int _selectedIndex;

        /// <summary>
        /// 当前所选择的包管理工具对象
        /// </summary>
        private PackageManagerBase _currentSelectedPackageManager;

        /// <summary>
        /// Conda页面的ViewModel
        /// </summary>
        private PackageManagerCondaMirrorSettingPageViewModel _packageManagerCondaMirrorSettingPageViewModel;

        /// <summary>
        /// 添加一个包管理工具，用于绑定在Button上
        /// </summary>
        public ICommand AddPackageManagerCommand => new RelayCommand<string>(AddPackageManager);

        /// <summary>
        /// 删除一个包管理工具，用于绑定在Button上
        /// </summary>
        public ICommand RemovePackageManagerCommand => new RelayCommand(RemovePackageManager);

        /// <summary>
        /// 移动某一个包管理器在列表中的位置，用于绑定在Button上
        /// </summary>
        public ICommand ShiftPackageManagerMirrorPropertyCommand =>
            new RelayCommand<string>(ShiftPackageManagerMirrorProperty);

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                RaisePropertyChanged();
            }
        }

        public PackageManagerBase CurrentSelectedPackageManager
        {
            get => _currentSelectedPackageManager;
            set
            {
                _currentSelectedPackageManager = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PackageManagerBase> PackageManagerList
        {
            get => _packageManagerList;
            set
            {
                _packageManagerList = value;
                RaisePropertyChanged();
            }
        }

        public PackageManagerCondaMirrorSettingPageViewModel PackageManagerCondaMirrorSettingPageViewModel
        {
            get => _packageManagerCondaMirrorSettingPageViewModel;
            set
            {
                _packageManagerCondaMirrorSettingPageViewModel = value;
                RaisePropertyChanged();
            }
        }

        public MainViewModel(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            PackageManagerList = new ObservableCollection<PackageManagerBase>()
            {
                // TODO:测试数据，以后要从配置文件中加载
                new PackageManagerConda("第1个镜像"),
                new PackageManagerNpm("第2个镜像"),
                new PackageManagerPip("第3个镜像"),
            };
            PackageManagerCondaMirrorSettingPageViewModel =
                new PackageManagerCondaMirrorSettingPageViewModel(mainWindow);
        }

        /// <summary>
        /// 添加一个包管理工具
        /// </summary>
        /// <param name="parameter"></param>
        private void AddPackageManager(object parameter)
        {
            string packageManagerType = (string)parameter;
            switch (packageManagerType)
            {
                case "Conda":
                    PackageManagerList.Add(new PackageManagerConda());
                    break;
                case "Npm":
                    PackageManagerList.Add(new PackageManagerNpm());
                    break;
                case "Pip":
                    PackageManagerList.Add(new PackageManagerPip());
                    break;
                default:
                    break;
            }

            if (MainWindow.FindName("PackageManagerSupportedListMenu") is Popup popup) popup.IsOpen = false;
        }

        /// <summary>
        /// 删除一个包管理工具
        /// </summary>
        private void RemovePackageManager()
        {
            // 弹出确认框，询问用户是否确认删除
            if (MessageBox.Show("是否删除该包管理器镜像配置？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
                MessageBoxResult.OK)
            {
                // 第一步：删除该配置
                PackageManagerList.RemoveAt(SelectedIndex);
                // 第二步：如果当前还有配置项，就选中第一个，否则不选中，显示默认页面
                if (MainWindow.FindName("AddedPackageManagerListBox") is ListBox listBox)
                {
                    if (PackageManagerList.Count > 0)
                    {
                        listBox.SelectedIndex = 0;
                        // 如果第一个存在，且类型为PackageManagerBase的子类
                        if (listBox.Items[0] is PackageManagerBase item)
                        {
                            // 那么就加载相对应的管理页面到PackageManagerSettingPage
                            MainWindow.LoadPackageManagerSettingPage(item.Type);
                        }
                        else
                        {
                            // 否则就加载默认页面
                            MainWindow.LoadPackageManagerSettingPage(PackageManagerType.None);
                        }
                    }
                    else
                    {
                        // 否则就加载默认页面
                        MainWindow.LoadPackageManagerSettingPage(PackageManagerType.None);
                    }
                }
            }
        }

        /// <summary>
        /// 移动某一个包管理器在列表中的位置
        /// </summary>
        private void ShiftPackageManagerMirrorProperty(object parameter)
        {
            ListBox listBox = MainWindow.FindName("AddedPackageManagerListBox") as ListBox;
            if (listBox == null) return;
            // 拷贝当前选中的index，因为如果PackageManagerList发生变化，视图层就会更新
            // 而SelectedIndex会因为AddedPackageManagerListBox更新后未选中而变成-1
            int index = SelectedIndex;
            switch (parameter)
            {
                case "UP":
                    // 仅当不是选择第一个的时候有效
                    if (index > 0)
                    {
                        index--;
                        ObservableCollectionUtil.Swap(PackageManagerList, index, index + 1);
                    }

                    break;
                case "DOWN":
                    if (index < PackageManagerList.Count - 1)
                    {
                        index++;
                        ObservableCollectionUtil.Swap(PackageManagerList, index, index - 1);
                    }

                    break;
                default:
                    break;
            }

            listBox.SelectedIndex = index;
            // 尽管SelectedIndex会因为视图层的变化触发AddedPackageManagerListBox_OnSelectionChanged，
            // 而在AddedPackageManagerListBox_OnSelectionChanged函数中SelectedIndex会更新未视图层所选择的坐标
            // 但此处做一个注释和显示赋值，避免歧义
            SelectedIndex = index;
            if (PackageManagerList[SelectedIndex] is PackageManagerBase item)
            {
                // 那么就加载相对应的管理页面到PackageManagerSettingPage
                MainWindow.LoadPackageManagerSettingPage(item.Type);
            }
        }
    }
}