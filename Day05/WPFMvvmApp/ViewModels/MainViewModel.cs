using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFMvvmApp.Models;

namespace WPFMvvmApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // 이 값들을 View에서 사용하려고 생성. Models에서 만든건 DB에서 사용하기 위함.
        private string inFirstName;
        private string inLastName;
        private string inEmail;
        private DateTime inDate;

        private string outFirstName;
        private string outLastName;
        private string outEmail;
        private string outDate;

        private string outAdult;
        private string outBirthday;

        public string InFirstName
        {
            get { return inFirstName; }
            set
            {
                inFirstName = value;
                RaisePropertyChanged("inFirstName"); // 값이 바뀜 공지!!
            }
        }

        public string InLastName
        {
            get { return inLastName; }
            set
            {
                inLastName = value;
                RaisePropertyChanged("inLastName"); // 값이 바뀜 공지!!
            }
        }
        public string InEmail
        {
            get { return inEmail; }
            set
            {
                inEmail = value;
                RaisePropertyChanged("inEmail"); // 값이 바뀜 공지!!
            }
        }
        public DateTime InDate
        {
            get { return inDate; }
            set
            {
                inDate = value;
                RaisePropertyChanged("inDate"); // 값이 바뀜 공지!!
            }
        }
        public string OutFirstName
        {
            get { return outFirstName; }
            set
            {
                outFirstName = value;
                RaisePropertyChanged("outFirstName"); // 값이 바뀜 공지!!
            }
        }
        public string OutLastName
        {
            get { return outLastName; }
            set
            {
                outLastName = value;
                RaisePropertyChanged("outLastName"); // 값이 바뀜 공지!!
            }
        }


        public string OutEmail
        {
            get { return outEmail; }
            set
            {
                outEmail = value;
                RaisePropertyChanged("outEmail"); // 값이 바뀜 공지!!
            }
        }
        public string OutDate
        {
            get { return outDate; }
            set
            {
                outDate = value;
                RaisePropertyChanged("outDate"); // 값이 바뀜 공지!!
            }
        }
        public string OutAdult
        {
            get { return outAdult; }
            set
            {
                outAdult = value;
                RaisePropertyChanged("outAdult"); // 값이 바뀜 공지!!
            }
        }
        public string OutBirthday
        {
            get { return outBirthday; }
            set
            {
                outBirthday = value;
                RaisePropertyChanged("outBirthday"); // 값이 바뀜 공지!!
            }
        }

        // 값이 전부 적용되서 버튼을 활성화하기 위한 명령
        private ICommand proceedCommand;
        public ICommand ProceedCommand
        {
            get {
                return proceedCommand ?? (
                  proceedCommand = new RelayCommand<object>(
                      o => Proceed(), o => !string.IsNullOrEmpty(inFirstName) &&
                                           !string.IsNullOrEmpty(inLastName) &&
                                           !string.IsNullOrEmpty(inEmail) &&
                                           !string.IsNullOrEmpty(inDate.ToString())
                                            )
                  );
            }
        }

        // 버튼클릭시 일어나는 실제 명령의 실체
        private async void Proceed()
        {
            try
            {
                Person person = new Person(inFirstName, inLastName, inEmail, inDate);

                await Task.Run(() => OutFirstName = person.FirstName);
                await Task.Run(() => OutLastName = person.LastName);
                await Task.Run(() => OutEmail = person.Email);
                await Task.Run(() => OutDate = person.Date.ToString("yyyy-MM-dd"));
                await Task.Run(() => OutAdult = person.IsAdult.ToString());
                await Task.Run(() => OutBirthday = person.IsBirthday.ToString());

                //to do
            }
            catch (Exception ex)
            {

                MessageBox.Show($"예외발생 : {ex.Message}");
            }
        }

        public MainViewModel()
        {
            this.inDate = DateTime.Parse("1990-01-01");
        }
    }
}
