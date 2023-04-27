using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Mirrors_All_in_One.Common;
using Mirrors_All_in_One.Data;
using Mirrors_All_in_One.UserControls;
using Mirrors_All_in_One.Utils;
using Mirrors_All_in_One.ViewModels;

namespace Mirrors_All_in_One.View
{
    public partial class PackageManagerCondaMirrorSettingPage : Page
    {
        private readonly Window _mainWindow;
        private MainViewModel MainViewModel { get; set; }

        public PackageManagerCondaMirrorSettingPage()
        {
            InitializeComponent();
            // 获取 NavigationWindow 或 Frame 实例
            if (Application.Current.Windows
                    .Cast<Window>()
                    .FirstOrDefault(window => window is MainWindow) is MainWindow mainWindow)
            {
                _mainWindow = mainWindow;
                PackageManagerCondaMirrorSettingPageViewModel =
                    new PackageManagerCondaMirrorSettingPageViewModel(mainWindow);
                MainViewModel = mainWindow.MainViewModel;
            }

            DataContext = MainViewModel;
        }

        public PackageManagerCondaMirrorSettingPageViewModel PackageManagerCondaMirrorSettingPageViewModel { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // DataContext = MainViewModel;
        }

        /// <summary>
        /// 位置：镜像仓库 - 添加镜像
        /// 作用：添加一个镜像到镜像仓库
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
            // 设置窗口的位置
            ResetModalWindowPosition(modalWindow);

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

            ReloadEnableMirrorRemarkFromRepository();
        }

        /// <summary>
        /// 位置：镜像仓库 - 删除镜像
        /// 作用：删除一个镜像
        /// </summary>
        private void RemoveMirrorFromRepository(object sender, RoutedEventArgs e)
        {
            var repository = MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                .PackageManagerCondaMirrorRepository;
            foreach (Mirror selectedItem in PackageManagerCondaMirrorRepositoryListBox.SelectedItems)
            {
                repository.Remove(selectedItem);
            }

            MainViewModel.PackageManagerCondaMirrorSettingPageViewModel.PackageManagerCondaMirrorRepository =
                repository;
            ReloadEnableMirrorRemarkFromRepository();
        }

        /// <summary>
        /// 位置：镜像仓库 - 移动镜像
        /// 作用：移动镜像，并保存到数据文件
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
            }

            MainViewModel.PackageManagerCondaMirrorSettingPageViewModel.PackageManagerCondaMirrorRepository =
                repository;
            PackageManagerCondaMirrorRepositoryListBox.SelectedIndex = index;
        }

        /// <summary>
        /// 位置：镜像仓库 - 修改镜像
        /// 作用：更改镜像信息
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
            // 设置窗口的位置
            ResetModalWindowPosition(modalWindow);
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
                        }

                        // 因为修改的只有一个镜像，所以只要找到一个就可以退出循环
                        break;
                    }
                }

                // 重新加载已启用的镜像列表，因为镜像的备注可能发生了改变
                ReloadEnableMirrorRemarkFromRepository();
            }
        }

        /// <summary>
        /// 重新加载已启用的镜像列表，因为镜像的备注可能发生了改变
        /// </summary>
        private void ReloadEnableMirrorRemarkFromRepository()
        {
            // 获取当前的镜像列表
            ObservableCollection<Mirror> channels =
                ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).Channels;
            Dictionary<string, Mirror> mirrorListConvertToDictionary = UserDataUtil.MirrorListConvertToDictionary(
                UserDataUtil.GetInstance()
                    .DataMirrorRepositoryUtil.DataPackageManagerMirrorRepository
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

        /// <summary>
        /// 位置：启用镜像 - 添加按钮
        /// 作用：将选择的镜像添加到已启用的镜像中
        /// 流程：
        /// 1. 获取选中的所有镜像
        /// 2. 添加到已启用镜像中
        /// 3. 清空在多选框中已选择的镜像
        /// 4. 设置脏位
        /// 
        /// </summary>
        private void AddChannelToEnabledChannelList(object sender, RoutedEventArgs routedEventArgs)
        {
            var allSelectedMirrors = SelectToReadyEnableMirrorComboBox.SelectedItems;
            var currentSelectedPackageManager = (PackageManagerConda)MainViewModel.CurrentSelectedPackageManager;
            var currentChannels = currentSelectedPackageManager.Channels;
            foreach (Mirror mirror in allSelectedMirrors)
            {
                currentChannels.Add(mirror);
            }

            SelectToReadyEnableMirrorComboBox.SelectedItems.Clear();
            MainViewModel.HasChanged = true;
        }

        /// <summary>
        /// 位置：启用镜像 - 删除按钮
        /// 作用：将选择的镜像从已启用的镜像中删除
        /// 流程：
        /// 1. 获取当前已启用的镜像列表
        /// 2. 删除选中的镜像
        /// 3. 设置脏位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveChannelFromEnableChannelList(object sender, RoutedEventArgs e)
        {
            var channels = ((PackageManagerConda)(MainViewModel.CurrentSelectedPackageManager)).Channels;
            channels.RemoveAt(PackageManagerCondaMirrorEnableChannelListBox.SelectedIndex);
            MainViewModel.HasChanged = true;
        }

        /// <summary>
        /// 位置：启用镜像 - 修改按钮
        /// 作用：修改选择的已启用的镜像
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyChannelFromEnableChannelList(object sender, RoutedEventArgs e)
        {
            Mirror selectedMirror = (Mirror)PackageManagerCondaMirrorEnableChannelListBox.SelectedItem;
            // 不允许重复的镜像列表为：除了当前选中的镜像之外的所有镜像，映射为string类型的channel组成的HashSet
            HashSet<string> notAllowDuplicateMirrorList = UserDataUtil.GetInstance()
                .DataMirrorRepositoryUtil
                .DataPackageManagerMirrorRepository.CondaMirrorRepository
                .Where(mirror => !mirror.Channel.Trim().Equals(selectedMirror.Channel.Trim()))
                .Select(mirror => mirror.Channel.Trim()).ToHashSet();
            // 打开修改镜像的窗口
            var modalWindow = new EditMirrorDialog(notAllowDuplicateMirrorList)
            {
                // 设置窗口的标题
                Title = "编辑镜像",
                MirrorPath = selectedMirror.Channel,
                MirrorRemark = selectedMirror.Remark,
            };
            // 设置窗口的位置
            ResetModalWindowPosition(modalWindow);

            // 如果用户点击了确定按钮
            if (modalWindow.ShowDialog() == true)
            {
                // 需要的额外操作分几种情况：
                // 1. 如果用户没有修改镜像的Channel
                // 1.1 如果该镜像在镜像仓库中存在，那么需要同步修改镜像仓库中对应镜像的备注
                // 1.2 如果该镜像在镜像仓库中不存在
                // 1.2.1 如果用户修改了备注，提示用户当前镜像不存在于镜像仓库中，是否将当前镜像添加到镜像仓库中
                // 1.2.2 如果用户没有修改备注，那么不需要做任何操作

                // 2. 如果用户修改了镜像的Channel
                // 2.1 如果新Channel在镜像仓库中存在，这种情况不存在，因为之前已经判断过了
                // 2.2 如果新Channel在镜像仓库中不存在，那么提示用户是否需要将当前镜像添加到镜像仓库中
                // 2.2.1 如果用户选择了是，那么将当前镜像添加到镜像仓库中
                // 2.2.2 如果用户选择了否，那么不需要做任何操作

                // 对于主要操作，分以下几种情况：
                // 1. 如果用户没有修改镜像的Channel，那么只需要修改备注即可
                // 2. 如果用户修改了镜像的Channel，那么需要删除原来的镜像，添加新的镜像

                // 额外操作
                // 第一种情况
                if (selectedMirror.Channel.Trim().Equals(modalWindow.MirrorPath.Trim()))
                {
                    Dictionary<string, Mirror> dictionary = UserDataUtil.MirrorListConvertToDictionary(UserDataUtil
                        .GetInstance()
                        .DataMirrorRepositoryUtil.DataPackageManagerMirrorRepository
                        .CondaMirrorRepository);
                    if (dictionary.ContainsKey(selectedMirror.Channel.Trim()))
                    {
                        var packageManagerCondaMirrorRepository = MainViewModel
                            .PackageManagerCondaMirrorSettingPageViewModel.PackageManagerCondaMirrorRepository;
                        foreach (Mirror mirror in packageManagerCondaMirrorRepository)
                        {
                            if (mirror.Channel.Trim().Equals(selectedMirror.Channel.Trim()))
                            {
                                mirror.Remark = modalWindow.MirrorRemark;
                                break;
                            }
                        }

                        // 重新赋值，可以刷新界面，同时保存到本地
                        MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                            .PackageManagerCondaMirrorRepository = packageManagerCondaMirrorRepository;
                    }
                    else
                    {
                        if (!selectedMirror.Remark.Equals(modalWindow.MirrorRemark))
                        {
                            // 提示用户当前镜像不存在于镜像仓库中，是否将当前镜像添加到镜像仓库中
                            var messageBoxResult = MessageBox.Show(
                                $"当前镜像{modalWindow.MirrorPath}不存在于镜像仓库中，是否将当前镜像添加到镜像仓库中？若不添加，设置的备注将在下次刷新时清除。",
                                "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (messageBoxResult.Equals(MessageBoxResult.Yes))
                            {
                                var packageManagerCondaMirrorRepository = MainViewModel
                                    .PackageManagerCondaMirrorSettingPageViewModel.PackageManagerCondaMirrorRepository;
                                packageManagerCondaMirrorRepository.Add(new Mirror(modalWindow.MirrorPath.Trim(),
                                    modalWindow.MirrorRemark));
                                // 重新赋值，可以刷新界面，同时保存到本地
                                MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                                    .PackageManagerCondaMirrorRepository = packageManagerCondaMirrorRepository;
                            }
                        }
                        else
                        {
                            // 什么都不做
                        }
                    }
                }
                // 第二种情况
                else
                {
                    // 如果新Channel在镜像仓库中不存在，那么提示用户是否需要将当前镜像添加到镜像仓库中
                    MessageBoxResult messageBoxResult = MessageBox.Show(
                        $"新镜像({modalWindow.MirrorPath.Trim()})不存在于镜像仓库中，是否将当前镜像添加到镜像仓库中？若不添加，设置的备注将在下次刷新时清除。",
                        "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (messageBoxResult.Equals(MessageBoxResult.Yes))
                    {
                        // 将当前镜像添加到镜像仓库中
                        var packageManagerCondaMirrorRepository = MainViewModel
                            .PackageManagerCondaMirrorSettingPageViewModel.PackageManagerCondaMirrorRepository;
                        packageManagerCondaMirrorRepository.Add(new Mirror(modalWindow.MirrorPath.Trim(),
                            modalWindow.MirrorRemark));
                        // 重新赋值，可以刷新界面，同时保存到本地
                        MainViewModel.PackageManagerCondaMirrorSettingPageViewModel
                            .PackageManagerCondaMirrorRepository = packageManagerCondaMirrorRepository;
                    }
                }

                // 主要操作
                // 第一种情况
                if (selectedMirror.Channel.Trim().Equals(modalWindow.MirrorPath.Trim()))
                {
                    Mirror mirror = ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).Channels[
                        PackageManagerCondaMirrorEnableChannelListBox.SelectedIndex];
                    mirror.Remark = modalWindow.MirrorRemark;
                    ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).Channels[
                        PackageManagerCondaMirrorEnableChannelListBox.SelectedIndex] = mirror;
                }
                // 第二种情况
                else
                {
                    ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).Channels.RemoveAt(
                        PackageManagerCondaMirrorEnableChannelListBox.SelectedIndex);
                    ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).Channels.Add(
                        new Mirror(modalWindow.MirrorPath.Trim(), modalWindow.MirrorRemark));
                }

                // 设置脏位
                MainViewModel.HasChanged = true;
            }
        }

        /// <summary>
        /// 位置：启用镜像 - 移动按钮
        /// 作用：移动镜像，通过传入的CommandParameter来判断是上移还是下移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShiftMirrorFromEnableChannelList(object sender, RoutedEventArgs e)
        {
            // 获得CommandParameter
            string parameter = ((Button)sender).CommandParameter.ToString();
            int index = PackageManagerCondaMirrorEnableChannelListBox.SelectedIndex;
            var channels = ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).Channels;
            switch (parameter)
            {
                case "UP":
                    // 如果选中的是第一个，就不执行任何操作
                    if (PackageManagerCondaMirrorEnableChannelListBox.SelectedIndex <= 0) return;
                    // 交换选中的镜像和上一个镜像的位置
                    index--;
                    ObservableCollectionUtil.Swap(channels, index, index + 1);
                    MainViewModel.HasChanged = true;
                    break;
                case "DOWN":
                    // 如果选中的是最后一个，就不执行任何操作
                    if (PackageManagerCondaMirrorEnableChannelListBox.SelectedIndex >= channels.Count - 1) return;
                    // 交换选中的镜像和下一个镜像的位置
                    index++;
                    ObservableCollectionUtil.Swap(channels, index, index - 1);
                    MainViewModel.HasChanged = true;
                    break;
                default:
                    throw new InvalidEnumArgumentException("命令参数错误，只能是 UP 或 DOWN");
            }

            ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).Channels = channels;
            PackageManagerCondaMirrorEnableChannelListBox.SelectedIndex = index;
        }

        /// <summary>
        /// 重新设置弹窗位于主窗口的中心
        /// </summary>
        /// <param name="modalWindow"></param>
        private void ResetModalWindowPosition(Window modalWindow)
        {
            // 重置窗口位置
            // 设置窗口的位置
            modalWindow.Owner = _mainWindow;
            modalWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            modalWindow.Left = _mainWindow.Left + (_mainWindow.Width - modalWindow.Width) / 2;
            modalWindow.Top = _mainWindow.Top + (_mainWindow.Height - modalWindow.Height) / 2;
        }

        /// <summary>
        /// 位置：启用镜像 - 应用
        /// 将当前已启用镜像生效
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyChangesToPropertyFile(object sender, RoutedEventArgs e)
        {
            PackageManagerConda packageManagerConda =
                ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager);
            if (!packageManagerConda.SaveChannelsToPropertyFile())
            {
                // 告诉用户保存失败
                MessageBox.Show("保存失败，请检查文件是否被占用", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // 保存成功后，将脏位设置为false
                MainViewModel.HasChanged = false;
                // 告诉用户保存成功
                MessageBox.Show("保存成功", "提示", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }

        /// <summary>
        /// 从配置文件中获取镜像列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadEnableMirrorListFromPropertyFile(object sender, RoutedEventArgs e)
        {
            // 首先询问用户是否需要重置
            MessageBoxResult messageBoxResult = MessageBox.Show("是否需要重置？", "提示", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (messageBoxResult.Equals(MessageBoxResult.Yes))
            {
                var packageManagerConda = (PackageManagerConda)MainViewModel.CurrentSelectedPackageManager;
                packageManagerConda.ReloadProperty();
                switch (packageManagerConda.PropertyFileState)
                {
                    case CondaPropertyState.FileError:
                        // 如果文件不存在，那么就提示用户
                        MessageBox.Show("重置错误，请检查文件是否被占用或者文件不存在。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case CondaPropertyState.Error:
                        // 如果重置错误，那么就提示用户
                        MessageBox.Show("重置错误，请检查文件是否被占用或者文件不存在。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case CondaPropertyState.Valid:
                        // 重新加载已启用的镜像列表，因为镜像的备注可能发生了改变
                        ReloadEnableMirrorRemarkFromRepository();
                        // 重置成功后，将脏位设置为false
                        MainViewModel.HasChanged = false;
                        // 如果重置成功，那么就提示用户
                        MessageBox.Show("重置成功", "提示", MessageBoxButton.OK, MessageBoxImage.None);
                        break;
                }
            }
        }

        /// <summary>
        /// 修改配置文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyPropertyFilePath(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Path.GetFullPath(
                ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).PropertyPath);
            openFileDialog.Filter = "Anaconda配置文件|.condarc";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == true)
            {
                var newPropertyFileName = openFileDialog.FileName;
                if (!Path.IsPathRooted(newPropertyFileName))
                {
                    // 告知用户文件路径无效
                    MessageBox.Show("文件路径无效", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (!File.Exists(newPropertyFileName))
                {
                    // 告知用户文件不存在
                    MessageBox.Show("文件不存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).PropertyPath = newPropertyFileName;
                ((PackageManagerConda)MainViewModel.CurrentSelectedPackageManager).ReloadProperty();
                // 修改路径后，将脏位设置为false
                MainViewModel.HasChanged = false;
            }
        }
    }
}