using Caliburn.Micro;
using System.Windows;
using WPFCaliburnApp.ViewModels;

namespace WPFCaliburnApp
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize(); // BootstrapperBase에 들어가는 기본 메소드, Framework를 초기화 해줌 -> Caliburn을 쓰기 위해서 필요
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //base.OnStartup(sender, e);

            DisplayRootViewFor <MainViewModel> ();
        }
    }
}
