using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Mirrors_All_in_One.Common;
using Mirrors_All_in_One.Data;
using Mirrors_All_in_One.Enums;
using Mirrors_All_in_One.ViewModels;

namespace Mirrors_All_in_One
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainViewModel _mainViewModel;

        public MainViewModel MainViewModel
        {
            get => _mainViewModel;
            set => _mainViewModel = value;
        }

        public MainWindow()
        {
            InitializeComponent();
            MainViewModel = new MainViewModel(this);
            DataContext = MainViewModel;
        }

        /// <summary>
        /// 在主页面的右侧，加载不同的镜像管理页面
        /// </summary>
        /// <param name="packageManagerType"></param>
        public void LoadPackageManagerSettingPage(PackageManagerType packageManagerType)
        {
            string path;
            switch (packageManagerType)
            {
                case PackageManagerType.Conda:
                    path = "/View/PackageManagerCondaMirrorSettingPage.xaml";
                    break;
                case PackageManagerType.Npm:
                    path = "/View/PackageManagerNpmMirrorSettingPage.xaml";
                    break;
                case PackageManagerType.Pip:
                    path = "/View/PackageManagerPipMirrorSettingPage.xaml";
                    break;
                default:
                    path = "/View/PackageManagerNoneMirrorSettingPage.xaml";
                    break;
            }

            PackageManagerSettingPage.Navigate(new Uri(path, UriKind.Relative));
        }

        /// <summary>
        /// 当AddedPackageManagerListBox（用于放置已经添加的包管理列表）加载完毕时，触发该函数
        /// 使得AddedPackageManagerListBox自动选中第一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddedPackageManagerListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.Items.Count > 0)
            {
                listBox.SelectedIndex = 0;
                // 如果第一个存在，且类型为PackageManagerBase的子类
                if (listBox.Items[0] is PackageManagerBase item)
                {
                    // 那么就加载相对应的管理页面到PackageManagerSettingPage
                    LoadPackageManagerSettingPage(item.Type);
                }
            }
        }

        /// <summary>
        /// 当AddedPackageManagerListBox选中不同的镜像时触发
        /// 1. 设置PackageManagerSettingPage到对应的页面
        /// 2. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AddedPackageManagerListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 得到当前选中的Item
            int selectedIndex = AddedPackageManagerListBox.SelectedIndex;
            if (0 <= selectedIndex && selectedIndex < MainViewModel.PackageManagerList.Count)
            {
                // 得到所指向的包管理工具的父对象
                PackageManagerBase packageManagerBase =
                    MainViewModel.PackageManagerList[selectedIndex] as PackageManagerBase;
                // 加载当前对象对应的页面到PackageManagerSettingPage
                LoadPackageManagerSettingPage(packageManagerBase.Type);
                // 同步当前所选择的数据到MVM
                _mainViewModel.SelectedIndex = selectedIndex;
                _mainViewModel.CurrentSelectedPackageManager = packageManagerBase;
            }
            else
            {
                // 加载当前对象对应的页面到PackageManagerSettingPage
                LoadPackageManagerSettingPage(PackageManagerType.None);
                // 同步当前所选择的数据到MVM
                _mainViewModel.SelectedIndex = selectedIndex;
                _mainViewModel.CurrentSelectedPackageManager = null;
            }
        }

        /// <summary>
        /// 点击OpenPackageManagerSupportedListButton后，打开添加包管理工具菜单（PackageManagerSupportedListMenu）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OpenPackageManagerSupportedListMenu(object sender, RoutedEventArgs e)
        {
            PackageManagerSupportedListMenu.IsOpen = true;
        }

        /// <summary>
        /// 主窗口加载前的动作
        /// 1. 加载数据文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            // 1. 加载数据文件：只需要实例化即可，数据自动加载
            UserDataUtil.GetInstance();
        }
    }
}