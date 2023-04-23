using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Mirrors_All_in_One.Common;
using Mirrors_All_in_One.Data;
using Mirrors_All_in_One.UserControls;
using Mirrors_All_in_One.Utils;
using Mirrors_All_in_One.ViewModels;

namespace Mirrors_All_in_One.View
{
    public partial class PackageManagerCondaMirrorSettingPage : Page
    {
        public PackageManagerCondaMirrorSettingPage()
        {
            InitializeComponent();
            // 获取 NavigationWindow 或 Frame 实例
            if (Application.Current.Windows
                    .Cast<Window>()
                    .FirstOrDefault(window => window is MainWindow) is MainWindow mainWindow)
            {
                PackageManagerCondaMirrorSettingPageViewModel =
                    new PackageManagerCondaMirrorSettingPageViewModel(mainWindow);
                MainViewModel = mainWindow.MainViewModel;
            }

            DataContext = MainViewModel;
        }

        public PackageManagerCondaMirrorSettingPageViewModel PackageManagerCondaMirrorSettingPageViewModel { get; set; }

        private MainViewModel MainViewModel { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // DataContext = MainViewModel;
        }

        /// <summary>
        /// 位置：启用镜像 - 添加按钮
        /// 作用：将选择的镜像添加到已启用的镜像中
        /// 流程：
        /// 1. 获取选中的所有镜像
        /// 2. 添加到已启用镜像中
        /// 3. 清空在多选框中已选择的镜像
        /// 4. 同步到配置文件
        /// 
        /// </summary>
        private void AddChannelToEnabledChannelList(object sender, RoutedEventArgs routedEventArgs)
        {
            var allSelected = (List<KeyValuePair<string, Mirror>>)SelectToReadyEnableMirrorComboBox.SelectedItems;
            var currentSelectedPackageManager = (PackageManagerConda)MainViewModel.CurrentSelectedPackageManager;
            var currentChannels = currentSelectedPackageManager.Channels;

            var selectedPackageManagerCondaMirrorOptions = PackageManagerCondaMirrorSettingPageViewModel
                .SelectedPackageManagerCondaMirrorOptions;
        }

        /// <summary>
        /// 添加一个镜像到镜像仓库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMirrorToRepository(object sender, RoutedEventArgs e)
        {
            HashSet<string> existedMirrorPath = new HashSet<string>();
            foreach (Mirror mirror in MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                         .PackageManagerCondaMirrorRepository)
            {
                existedMirrorPath.Add(mirror.Channel);
            }

            // 创建一个新的 Window 对象
            var modalWindow = new EditMirrorDialog(existedMirrorPath)
            {
                // 设置窗口的标题
                Title = "新建镜像",
            };

            // 设置窗口的窗口样式为模态窗口
            if (modalWindow.ShowDialog() == true)
            {
                string mirrorPath = modalWindow.MirrorPath;
                string mirrorRemark = modalWindow.MirrorRemark;
                Mirror newMirror = new Mirror(mirrorPath.Trim(), mirrorRemark);
                var repository = MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                    .PackageManagerCondaMirrorRepository;
                repository
                    .Add(newMirror);
                MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                    .PackageManagerCondaMirrorRepository = repository;
            }

            ReloadEnableMirrorFromRepository();
        }

        /// <summary>
        /// 删除一个镜像
        /// </summary>
        private void DeleteMirrorFromRepository(object sender, RoutedEventArgs e)
        {
            var repository = MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                .PackageManagerCondaMirrorRepository;
            foreach (Mirror selectedItem in PackageManagerCondaMirrorRepositoryListBox.SelectedItems)
            {
                repository.Remove(selectedItem);
            }

            MainViewModel.PackageManagerCondaMirrorSettingPageViewModel.PackageManagerCondaMirrorRepository =
                repository;
            ReloadEnableMirrorFromRepository();
        }

        /// <summary>
        /// 移动镜像，并保存到数据文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        private void ShiftMirrorFromRepository(object sender, RoutedEventArgs e)
        {
            // 获得按钮绑定的CommandParameter
            var parameter = (string)((Button)sender).CommandParameter;
            int index = PackageManagerCondaMirrorRepositoryListBox.SelectedIndex;
            var repository = MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                .PackageManagerCondaMirrorRepository;
            switch (parameter)
            {
                case "UP":
                    // 如果选中的是第一个，就不执行任何操作
                    if (PackageManagerCondaMirrorRepositoryListBox.SelectedIndex <= 0) return;
                    // 交换选中的镜像和上一个镜像的位置
                    index--;
                    ObservableCollectionUtil.Swap(repository, index, index + 1);
                    break;
                case "DOWN":
                    // 如果选中的是最后一个，就不执行任何操作
                    if (PackageManagerCondaMirrorRepositoryListBox.SelectedIndex >= MainViewModel
                            .PackageManagerCondaMirrorSettingPageViewModel.PackageManagerCondaMirrorRepository
                            .Count - 1) return;
                    // 交换选中的镜像和下一个镜像的位置
                    index++;
                    ObservableCollectionUtil.Swap(repository, index, index - 1);
                    break;
                default:
                    throw new InvalidEnumArgumentException("命令参数错误，只能是 UP 或 DOWN");
                    break;
            }

            MainViewModel.PackageManagerCondaMirrorSettingPageViewModel.PackageManagerCondaMirrorRepository =
                repository;
            PackageManagerCondaMirrorRepositoryListBox.SelectedIndex = index;
        }

        /// <summary>
        /// 更改镜像信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ModifyMirrorInfoFromRepository(object sender, RoutedEventArgs e)
        {
            // 获取选中的镜像
            var selectedItem = (Mirror)PackageManagerCondaMirrorRepositoryListBox.SelectedItem;
            HashSet<string> existedMirrorPath = new HashSet<string>();
            foreach (Mirror mirror in MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                         .PackageManagerCondaMirrorRepository)
            {
                if (!mirror.Channel.Equals(selectedItem.Channel)) existedMirrorPath.Add(mirror.Channel);
            }

            // 弹出一个窗口，显示镜像的信息
            var modalWindow = new EditMirrorDialog(existedMirrorPath)
            {
                // 设置窗口的标题
                Title = "编辑镜像",
                MirrorPath = selectedItem.Channel,
                MirrorRemark = selectedItem.Remark,
            };
            // 设置窗口的窗口样式为模态窗口
            if (modalWindow.ShowDialog() == true)
            {
                // 备份当前已启用的镜像列表
                ObservableCollection<Mirror> channels =
                    ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).Channels;
                // 将修改同步到镜像仓库中
                string mirrorPath = modalWindow.MirrorPath;
                string mirrorRemark = modalWindow.MirrorRemark;
                Mirror newMirror = new Mirror(mirrorPath.Trim(), mirrorRemark);
                var repository = MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                    .PackageManagerCondaMirrorRepository;
                repository[PackageManagerCondaMirrorRepositoryListBox.SelectedIndex] = newMirror;
                MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                    .PackageManagerCondaMirrorRepository = repository;
                // 检查该镜像是否已经启用
                foreach (Mirror mirror in channels)
                {
                    // 如果该已启用的镜像列表存在当前修改的镜像
                    if (mirror.Channel.Trim().Equals(selectedItem.Channel.Trim()))
                    {
                        // 如果镜像的path没发生变化，就直接同步修改
                        bool syncMirrorInfoToCondaProperty =
                            selectedItem.Channel.Trim().Equals(modalWindow.MirrorPath.Trim());
                        // 如果镜像的路径发生了变化，就检查该镜像是否已经启用，因为需要提示用户是否需要同步到已启用的镜像中
                        if (!syncMirrorInfoToCondaProperty)
                        {
                            MessageBoxResult messageBoxResult = MessageBox.Show(
                                $"原来的镜像为{selectedItem.Channel.Trim()}，检测到您已启用该镜像，是否将启用的镜像也修改为{modalWindow.MirrorPath.Trim()}？若点击取消将原来的镜像保存到镜像仓库。",
                                "提示",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);
                            syncMirrorInfoToCondaProperty = messageBoxResult == MessageBoxResult.Yes;
                            // 如果用户选择不同步修改
                            if (messageBoxResult == MessageBoxResult.No)
                            {
                                // 那么需要将原来的配置保存到数据文件中
                                var packageManagerCondaMirrorRepository = MainViewModel
                                    .PackageManagerCondaMirrorSettingPageViewModel
                                    .PackageManagerCondaMirrorRepository;
                                packageManagerCondaMirrorRepository.Add(new Mirror(mirror.Channel, mirror.Remark));
                                MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                                    .PackageManagerCondaMirrorRepository = packageManagerCondaMirrorRepository;
                            }
                        }

                        if (syncMirrorInfoToCondaProperty)
                        {
                            // 同步修改到已启用的镜像列表中
                            mirror.Channel = modalWindow.MirrorPath.Trim();
                            mirror.Remark = modalWindow.MirrorRemark;
                            // 重新赋值，以达到刷新的效果
                            mirror.DisplayName = mirror.DisplayName;
                        }

                        // 因为修改的只有一个镜像，所以只要找到一个就可以退出循环
                        break;
                    }
                }

                // 重新加载已启用的镜像列表，因为镜像的备注可能发生了改变
                ReloadEnableMirrorFromRepository();
            }
        }

        /// <summary>
        /// 重新加载已启用的镜像列表，因为镜像的备注可能发生了改变
        /// </summary>
        private void ReloadEnableMirrorFromRepository()
        {
            // 获取当前的镜像列表
            ObservableCollection<Mirror> channels =
                ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).Channels;
            Dictionary<string, Mirror> mirrorListConvertToDictionary = UserDataUtil.MirrorListConvertToDictionary(
                UserDataUtil.GetInstance()
                    .DataPackageManagerMirrorRepositoryUtil.DataPackageManagerMirrorRepository
                    .CondaMirrorRepository);
            foreach (Mirror mirror in channels)
            {
                mirror.Remark = mirrorListConvertToDictionary.TryGetValue(mirror.Channel, out var value)
                    ? value.Remark
                    : "";
                // 重新赋值，以达到刷新的效果
                mirror.DisplayName = mirror.DisplayName;
            }
        }
    }
}