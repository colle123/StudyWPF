using MahApps.Metro.Controls;
using WPFMvvmApp.ViewModels;

namespace WPFMvvmApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}
