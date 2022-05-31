using System.Windows.Controls;
using WpfBikeShop;

namespace WPFBikeShop
{
    /// <summary>
    /// ProductManagement.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProductManagement : Page
    {
        ProductsFactory factory = new ProductsFactory();
        public ProductManagement()
        {
            InitializeComponent();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            dgrProduct.ItemsSource = factory.FindProducts(txtSearch.Text);
        }
    }
}
