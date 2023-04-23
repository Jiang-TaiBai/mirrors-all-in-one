using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mirrors_All_in_One.ViewModels;

namespace Mirrors_All_in_One.View
{
    public partial class PackageManagerPipMirrorSettingPage : Page
    {
        public PackageManagerPipMirrorSettingPage()
        {
            InitializeComponent();
        }

        private MainViewModel MainViewModel { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 获取 NavigationWindow 或 Frame 实例
            if (Application.Current.Windows
                    .Cast<Window>()
                    .FirstOrDefault(window => window is MainWindow) is MainWindow mainWindow)
                MainViewModel = mainWindow.MainViewModel;
            DataContext = MainViewModel;
        }
    }
}