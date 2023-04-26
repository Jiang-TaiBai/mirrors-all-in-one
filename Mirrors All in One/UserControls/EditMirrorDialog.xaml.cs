using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using Mirrors_All_in_One.Common;
using Mirrors_All_in_One.Data;
using Mirrors_All_in_One.ViewModels;
using MessageBox = HandyControl.Controls.MessageBox;
using Window = System.Windows.Window;

namespace Mirrors_All_in_One.UserControls
{
    public partial class EditMirrorDialog : Window
    {
        public MainViewModel MainViewModel;

        public static readonly DependencyProperty MirrorPathProperty =
            DependencyProperty.Register(nameof(MirrorPath), typeof(string), typeof(EditMirrorDialog),
                new PropertyMetadata(null));

        public static readonly DependencyProperty MirrorRemarkProperty =
            DependencyProperty.Register(nameof(MirrorRemark), typeof(string), typeof(EditMirrorDialog),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ConfirmCommandProperty =
            DependencyProperty.Register(nameof(ConfirmCommand), typeof(ICommand), typeof(EditMirrorDialog),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(EditMirrorDialog),
                new PropertyMetadata(null));

        /// <summary>
        /// 已经存在的镜像路径，不允许重复添加
        /// </summary>
        private readonly HashSet<string> _notAllowDuplicateMirrorChannelList;

        public string MirrorPath
        {
            get => (string)GetValue(MirrorPathProperty);
            set => SetValue(MirrorPathProperty, value);
        }

        public string MirrorRemark
        {
            get => (string)GetValue(MirrorRemarkProperty);
            set => SetValue(MirrorRemarkProperty, value);
        }

        public ICommand ConfirmCommand
        {
            get => (ICommand)GetValue(ConfirmCommandProperty);
            set => SetValue(ConfirmCommandProperty, value);
        }

        public ICommand CancelCommand
        {
            get => (ICommand)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }

        private void Confirm()
        {
            // 执行确认逻辑，比如验证必填项是否填写
            // 如果必填项未填写，则可以弹出一个消息框提示用户
            // 如果必填项填写了，则可以将输入的内容保存到属性中
            // 通过关闭弹窗的方式退出弹窗，返回确认的结果
            if (String.IsNullOrWhiteSpace(MirrorPath))
            {
                MessageBox.Show("镜像内容不得为空", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (_notAllowDuplicateMirrorChannelList.Contains(MirrorPath.Trim()))
            {
                MessageBox.Show($"镜像仓库已添加相同的镜像：{MirrorPath}，不允许被重复添加", "提示", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }

            DialogResult = true;
            this.Close();
        }

        private void Cancel()
        {
            // 关闭弹窗，返回取消的结果
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notAllowDuplicateMirrorChannelList">出现在这里的Mirror都不允许被添加</param>
        public EditMirrorDialog(HashSet<string> notAllowDuplicateMirrorChannelList)
        {
            InitializeComponent();
            this._notAllowDuplicateMirrorChannelList = notAllowDuplicateMirrorChannelList;
            // 获取 NavigationWindow 或 Frame 实例
            if (Application.Current.Windows
                    .Cast<Window>()
                    .FirstOrDefault(window => window is MainWindow) is MainWindow mainWindow)
            {
                MainViewModel = mainWindow.MainViewModel;
            }

            DataContext = this;
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }
    }
}