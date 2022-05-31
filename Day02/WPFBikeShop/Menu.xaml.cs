﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFBikeShop
{
    /// <summary>
    /// Menu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Menu : Page
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(
                new Uri("/ProductManagement.xaml", UriKind.RelativeOrAbsolute)
                );
        }

        private void btnLiveSupport_Click(object sender, RoutedEventArgs e)
        {
            //Uri는 우리가 인터넷에서 알고있는 URL의 상위속성
            NavigationService.Navigate(
                new Uri("/Bindings.xaml", UriKind.RelativeOrAbsolute)
                );
        }

        private void btnEmailSupport_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(
                new Uri("/Contact.xaml", UriKind.RelativeOrAbsolute)
                );
        }
    }
}
