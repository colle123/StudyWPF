using System.Windows;

namespace WPFBikeShop
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitClass();
        }

        private void InitClass()
        {
            Car car = new Car();
        }
    }
}
