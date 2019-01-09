using System.Windows;
using His_Pos.Class.ProductType;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage
{
    /// <summary>
    /// DeleteTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DeleteTypeWindow : Window
    {
        public ProductType DeleteType;

        private ProductTypeManageMaster Master;
        private ProductTypeManageDetail Detail;

        public DeleteTypeWindow(ProductTypeManageMaster master, ProductTypeManageDetail detail)
        {
            InitializeComponent();

            BigType.Content = master.Name;

            if (detail is null)
            {
                SmallTypeRadioButton.IsEnabled = false;
                BigTypeRadioButton.IsChecked = true;
            }
            else
            {
                SmallType.Content = detail.Name;
            }

            Master = master;
            Detail = detail;
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            DeleteType = ((bool)BigTypeRadioButton.IsChecked) ? (ProductType)Master : (ProductType)Detail;
            Close();
        }
    }
}
