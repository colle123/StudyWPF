using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFSmartHomeMonitoringApp.Helpers;
using WPFSmartHomeMonitoringApp.Models;

namespace WPFSmartHomeMonitoringApp.ViewModels
{
    public class HistoryViewModel : Conductor<object>
    {
        //Divisions
        //DivisionVal
        //SeletedDivision
        //StartDate
        //InitStartDate
        //EndDate
        //InitEndDate
        //TotalCount
        //SearchIoTData()
        //HistoryModel

        private BindableCollection<DivisionModel> divisions;
        private DivisionModel selectedDivision;
        private string startDate;
        private string initStartDate;
        private string endDate;
        private string initEndDate;
        private int totalCount;
        private PlotModel historyModel; // oxyplot 220613, MSG. smartHomeModel -> historyModel로 변경

        public BindableCollection<DivisionModel> Divisions
        {
            get { return divisions; }
            set
            {
                divisions = value;
                NotifyOfPropertyChange(() => Divisions);
            }
        }

        public DivisionModel SelectedDivision
        {
            get { return selectedDivision; }
            set
            {
                selectedDivision = value;
                NotifyOfPropertyChange(() => SelectedDivision);
            }
        }

        public string StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }

        public string InitStartDate
        {
            get { return initStartDate; }
            set
            {
                initStartDate = value;
                NotifyOfPropertyChange(() => InitStartDate);
            }
        }

        public string EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }

        public string InitEndDate
        {
            get { return initEndDate; }
            set
            {
                initEndDate = value;
                NotifyOfPropertyChange(() => InitEndDate);
            }
        }

        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                totalCount = value;
                NotifyOfPropertyChange(() => TotalCount);
            }
        }

        public PlotModel HistoryModel
        {
            get { return historyModel; }
            set
            {
                historyModel = value;
                NotifyOfPropertyChange(() => HistoryModel);
            }
        }

        public HistoryViewModel()
        {
            Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenAplLab;Integrated Security=True;";
            InitControl();
        }

        private void InitControl()
        {
            Divisions = new BindableCollection<DivisionModel> //콤보박스용 데이터 생성
            {
                new DivisionModel { KeyVal = 0, DivisionVal = "-- Select --"},
                new DivisionModel { KeyVal = 1, DivisionVal = "DINNING"},
                new DivisionModel { KeyVal = 2, DivisionVal = "LIVING"},
                new DivisionModel { KeyVal = 3, DivisionVal = "BED"},
                new DivisionModel { KeyVal = 4, DivisionVal = "BATH"}
            };

            //Select를 선택해서 초기화
            SelectedDivision = Divisions.Where(v => v.DivisionVal.Contains("Select")).FirstOrDefault();

            InitStartDate = DateTime.Now.ToShortDateString(); // 2022-06-10

            InitEndDate = DateTime.Now.AddDays(1).ToShortDateString();

        }

        public void SearchIoTData()
        {
            if (SelectedDivision.KeyVal == 0)// Select
            {
                MessageBox.Show("검색할 방을 선택하세요.");
                return;
            }

            if (DateTime.Parse(StartDate) > DateTime.Parse(EndDate))
            {
                MessageBox.Show("시작일이 종료일보다 최신일 수 없습니다.");
                return;
            }

            TotalCount = 0;

            using(SqlConnection conn = new SqlConnection(Commons.CONNSTRING))
            {
                string strQuery = @"SELECT Id, DevId, CurrTime, Temp, Humid
                                      FROM tblSmartHome
                                     WHERE DevId = @DevId    
                                       AND CurrTime BETWEEN @StartDate AND @EndDate
                                     ORDER BY Id ASC ";

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(strQuery, conn);

                    SqlParameter parmDevId = new SqlParameter("@DevId", SelectedDivision.DivisionVal);
                    cmd.Parameters.Add(parmDevId);
                    SqlParameter parmStartDate = new SqlParameter("@StartDate", StartDate);
                    cmd.Parameters.Add(parmStartDate);
                    SqlParameter parmEndDate = new SqlParameter("@EndDate", EndDate);
                    cmd.Parameters.Add(parmEndDate);

                    SqlDataReader reader = cmd.ExecuteReader();

                    int i = 0;
                    // start of chart procee 220613 추가
                    PlotModel tmp = new PlotModel() { Title = $"{SelectedDivision.DivisionVal} Histories" }; // 임시 플롯모델 -> 사용하기 위해서보단 다른 곳에 전달하기 위함.

                    var l = new Legend // 범례, OxyPlot.Wpf > LegendsDemo 참조
                    {
                        LegendBorder = OxyColors.Black,
                        LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                        LegendPosition = LegendPosition.RightTop,
                        LegendPlacement = LegendPlacement.Inside
                    };
                    tmp.Legends.Add(l);

                    LineSeries seriesTemp = new LineSeries
                    {
                        Color = OxyColor.FromRgb(255, 100, 100),
                        Title = "Temparature",
                        MarkerSize = 3,
                        MarkerType = MarkerType.Circle

                    }; // 온도값을 Linechart로 담을 객체

                    LineSeries seriesHumid = new LineSeries
                    {
                        Color = OxyColor.FromRgb(150, 150, 255),
                        Title = "Humidity",
                        MarkerSize = 3,
                        MarkerType = MarkerType.Triangle
                    };

                    while (reader.Read())
                    {

                        var model = new SmartHomeModel();
                        model.DevId = reader["DevId"].ToString();
                        model.CurrTime = DateTime.Parse(reader["CurrTime"].ToString());
                        model.Temp = Convert.ToDouble(reader["Temp"]);
                        model.Humid = Convert.ToDouble(reader["Humid"]);

                        //seriesTemp.Points.Add(new DataPoint(i, model.Temp));
                        //seriesHumid.Points.Add(new DataPoint(i, model.Humid));

                        //var temp = reader["Temp"];
                        // Temp, Humid 차트 데이터를 생성

                        seriesTemp.Points.Add(new DataPoint(i, Convert.ToDouble(reader["Temp"])));
                        seriesHumid.Points.Add(new DataPoint(i, Convert.ToDouble(reader["Humid"])));

                        i++;
                    }

                    TotalCount = i;

                    tmp.Series.Add(seriesTemp);
                    tmp.Series.Add(seriesHumid);


                    HistoryModel = tmp;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error {ex.Message}");
                    return;
                }
            }
        }
    }
}
