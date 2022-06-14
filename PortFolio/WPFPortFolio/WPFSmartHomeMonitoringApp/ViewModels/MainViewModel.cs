using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WPFSmartHomeMonitoringApp.Helpers;

namespace WPFSmartHomeMonitoringApp.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public MainViewModel()
        {
            //MessageBox.Show("MVVM Start!");
            DisplayName = "SmartHome Monitoring v2.0"; // Window Title
         
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (Commons.MQTT_CLIENT.IsConnected)
            {
                Commons.MQTT_CLIENT.Disconnect();
                Commons.MQTT_CLIENT = null;
            }

            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public void LoadDataBaseView()
        {
            ActivateItemAsync(new DataBaseViewModel());
        }

        public void LoadRealTimeView()
        {
            ActivateItemAsync(new RealTimeViewModel());
        }

        public void LoadHistoryView()
        {
            ActivateItemAsync(new HistoryViewModel());
        }

        public void ExitProgram()
        {
            Environment.Exit(0); // 프로그램 종료
        }

        public void ExitToolbar()
        {
            Environment.Exit(0); // 프로그램 종료
        }

        // Start 메뉴, Icon을 눌렀을 때 처리할 이벤트
        public void PopInfoDialog()
        {
            TaskPopup();
        }

        public void StartSubscribe()
        {
            TaskPopup();
        }

        public void ToolBarStopSubscribe()
        {
            StopSubscribe();
        }


        public void MenuStopSubscribe()
        {
            StopSubscribe();
        }

        private void TaskPopup()
        {
            // CustomPopupView
            var winManager = new WindowManager();
            var result = winManager.ShowDialogAsync(new CustomPopupViewModel("New Broker"));

            if (result.Result == true)
            {
                ActivateItemAsync(new DataBaseViewModel()); // 화면전환
            }
        }

        public void PopInfoView()
        {
            var winManager = new WindowManager();
            var result = winManager.ShowDialogAsync(new CustomInfoViewModel("About"));
        }
        private void StopSubscribe()
        {
            if(this.ActiveItem is DataBaseViewModel)
            {
                DataBaseViewModel activeModel = (this.ActiveItem as DataBaseViewModel);
                try
                {
                    if (Commons.MQTT_CLIENT.IsConnected)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived -= activeModel.MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Disconnect();
                        activeModel.IsConnected = Commons.IS_CONNECT = false;
                    }
                }
                catch (Exception ex)
                {

                    
                }

                DeactivateItemAsync(this.ActiveItem, true);
            }
        }
    }
}
