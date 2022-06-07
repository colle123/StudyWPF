using Caliburn.Micro;
using System.Windows;
using WPFCaliburnApp.ViewModels;

namespace WPFCaliburnApp
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize(); // Bootstrapper 자체를 초기화 해주는 메소드
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //base.OnStartup(sender, e);
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
