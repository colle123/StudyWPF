using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WPFSmartHomeMonitoringApp.Helpers;
using WPFSmartHomeMonitoringApp.Models;

namespace WPFSmartHomeMonitoringApp.ViewModels
{
    public class DataBaseViewModel : Conductor<object>
    {


        private string brokerUrl;
        public string BrokerUrl
        {
            get { return brokerUrl; }
            set
            {
                brokerUrl = value;
                NotifyOfPropertyChange(() => BrokerUrl);
            }
        }

        private string topic;
        public string Topic
        {
            get { return topic; }
            set
            {
                topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }

        private string connString;
        public string ConnString
        {
            get { return connString; }
            set
            {
                connString = value;
                NotifyOfPropertyChange(() => ConnString);
            }
        }

        private string dbLog;
        public string DbLog
        {
            get { return dbLog; }
            set
            {
                dbLog = value;
                NotifyOfPropertyChange(() => DbLog);
            }
        }

        private bool isConnected;

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
            }
        }

        public DataBaseViewModel()
        {
            BrokerUrl = Commons.BROKERHOST = "127.0.0.1"; // MQTT Broker IP 설정
            Topic = Commons.PUB_TOPIC = "home/device/fakedata/";
            // Single Level WildCard + 
            // Multi Level WildCard #
            ConnString = Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenAplLab;Integrated Security=True;";

            if (Commons.IS_CONNECT)
            {
                IsConnected = true;
                ConnectDb();
            }
        }
        /// <summary>
        /// DB 연결 + MQTT Broker 접속
        /// </summary>
        public void ConnectDb()
        {
            if (IsConnected)
            {
                Commons.MQTT_CLIENT = new MqttClient(BrokerUrl); // MqttClient(BrokerUrl) 말고 MqttClient(BrokerUrl, 1883)이
                                                                 // 정확하지만 1883은 Default 이기 때문에 생략해도 됨.
                try
                {
                    if (Commons.MQTT_CLIENT.IsConnected != true)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Connect("MONITOR");
                        Commons.MQTT_CLIENT.Subscribe(new string[] { Commons.PUB_TOPIC },
                            new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

                        UpdateText(">>> MQTT Broker Connected");
                        isConnected = Commons.IS_CONNECT = true;

                    }
                }
                catch (Exception ex)
                {
                    //Pass;
                }
            }
            else // 접속 끄기
            {
                try
                {
                    if (Commons.MQTT_CLIENT.IsConnected)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived -= MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Disconnect();
                        UpdateText(">>> MQTT Broker Disconnected");
                        IsConnected = Commons.IS_CONNECT = false;
                    }
                }
                catch (Exception ex)
                {

                    //Pass;
                }
            }
        }

        private void UpdateText(string message)
        {
            DbLog += $"{message}\n";
        }


        /// <summary>
        /// Subscribe한 메세지 처리해주는 EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MQTT_CLIENT_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var Message = Encoding.UTF8.GetString(e.Message);
            UpdateText(Message); // Sensor data 출력
            SetDataBase(Message); // DB에 저장하는 메소드
        }

        private void SetDataBase(string message)
        {
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message); // Json Data는 Dictionary Type으로 되어있음.(key값과 value값)
            var smartHomeModel = new SmartHomeModel();
            smartHomeModel.DevId = currDatas["DevId"];
            smartHomeModel.CurrTime = DateTime.Parse(currDatas["CurrTime"]);
            smartHomeModel.Temp = double.Parse(currDatas["Temp"]);
            smartHomeModel.Humid = double.Parse(currDatas["Humid"]);


            Debug.WriteLine(currDatas);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string strInQuery = @"INSERT INTO TblSmartHome
                                   (DevId
                                   , CurrTime
                                   , Temp
                                   , Humid)
                                  VALUES
                                   (@DevId,
                                   @CurrTime,
                                   @Temp,
                                   @Humid)";

                try
                {
                    SqlCommand cmd = new SqlCommand(strInQuery, conn);
                    SqlParameter paraDevId = new SqlParameter("@DevId", smartHomeModel.DevId);
                    cmd.Parameters.Add(paraDevId);
                    SqlParameter parmCurrTime = new SqlParameter("@CurrTime", smartHomeModel.CurrTime);
                    cmd.Parameters.Add(parmCurrTime);
                    SqlParameter parmTemp = new SqlParameter("@Temp", smartHomeModel.Temp);
                    cmd.Parameters.Add(parmTemp);
                    SqlParameter parmHumid = new SqlParameter("@Humid", smartHomeModel.Humid);
                    cmd.Parameters.Add(parmHumid);

                    if (cmd.ExecuteNonQuery() == 1)
                        UpdateText(">>> DB Inserted");
                    else
                        UpdateText(">>> DB Failed!!!!!!");
                }
                catch (Exception ex)
                {
                    UpdateText($">>> DB Error! {ex.Message}");
                }
            }
        }
    }
}
