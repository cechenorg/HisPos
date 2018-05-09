using His_Pos.ViewModel;
using System;
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
using System.Windows.Shapes;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// ProductDetail.xaml 的互動邏輯
    /// </summary>
    public partial class ProductDetail : Window
    {
        public ProductDetail()
        {
            InitializeComponent();
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.SelectedItem is null) return;
            ((ViewModelProductDetailWindow)DataContext).AddProductDetailTabAction(((TabBase)Tabs.SelectedItem).TabName);
        }
        
        public void AddNewTab()
        {
           
            ((ViewModelProductDetailWindow)DataContext).AddProductDetailTabAction("處理單紀錄");
            this.Focus();
        }
    }
}
