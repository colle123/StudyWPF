﻿using Caliburn.Micro;
using OxyPlot;
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
        //SmartHomeModel

        private BindableCollection<DivisionModel> divisions;
        private DivisionModel selectedDivision;
        private string startDate;
        private string initStartDate;
        private string endDate;
        private string initEndDate;
        private int totalCount;
        private PlotModel smartHomeModel; // oxyplot

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

        public PlotModel SmartHomeModel
        {
            get { return smartHomeModel; }
            set
            {
                smartHomeModel = value;
                NotifyOfPropertyChange(() => SmartHomeModel);
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
                string strQuery = @"SELECT Id, CurrTime, Temp, Humid
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

                    var i = 0;

                    while (reader.Read())
                    {
                        var temp = reader["Temp"];
                        // Temp, Humid 차트 데이터를 생성

                        i++;
                    }

                    TotalCount = i;
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
