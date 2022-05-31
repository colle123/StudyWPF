using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFBikeShop
{
    public class Notifier : INotifyPropertyChanged
    {
        //오른쪽 하단에 Property(속성)이 바뀌는 이벤트가 발생할 때 어떻게 할지 처리하는 Class
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                //Property가 바뀌면 나 자신(this, WPFBikeShop)에게 알려준다는 뜻 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
