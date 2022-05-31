using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFBikeShop
{
    /// <summary>
    /// Bidings.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Bindings : Page
    {
        public Bindings()
        {
            InitializeComponent();

            Car c = new Car {
                Speed = 550,
                Color = Colors.Crimson,
                Driver = new Human
                {
                    FirstName = "Nick",
                    HasDrivingLicense = true
                }
            };

            // WPF는 다중상속이 안되기 때문에 Interface를 사용하여 상속을 해줌.

            this.DataContext = c;
            //stxPanel.DataContext = c; // 상위 StackPanel에 데이터를 넣어버리면 하위항목에서 데이터를 다 쓸 수 있
            //txtSpeed.DataContext = c;
            //txtColor.DataContext = c;
            //txtFirstName.DataContext = c; // 필수 !! 안넣으면 Data가 안 넘어옴. 이 데이터를 xaml로 보내야 함.
            // txtSpeed.Text = c.Speed.ToString(); // 고전적인 WinForm 방식
            //Data를 담아두는 공간에 c라는 이름의 Car라는 객체를 넣어준다는 뜻.

            var carlist = new List<Car>();
            for (int i = 0; i < 10 ; i++)
            {
                carlist.Add(new Car { 
                    Speed = i * 10,                    
                    Color = Colors.Purple
                });
            }

            lbxCars.DataContext = carlist;
        }
    }
}
