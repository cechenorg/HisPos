using His_Pos.Class.Product;
using His_Pos.InventoryManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace His_Pos.H2_STOCK_MANAGE.InventoryManagement
{
    /// <summary>
    /// DemolitionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DemolitionWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        private string aterDemolition;
        public string AterDemolition
        {
            get { return aterDemolition; }
            set
            {
                aterDemolition = value;
                NotifyPropertyChanged("AterDemolition");
            }
        }
        private ObservableCollection<ProductGroup> productGroups;
        public ObservableCollection<ProductGroup> ProductGroups
        {
            get { return productGroups; }
            set
            {
                productGroups = value;
                NotifyPropertyChanged("ProductGroups");
            }

        }
        public DemolitionWindow()
        {
            InitializeComponent();
            InitData();
            DataContext = this;
        }
        private void InitData() {
            ProductGroups = OtcDetail.Instance.ProductGroupCollection;
            ComboBoxProduct.ItemsSource = ProductGroups;
            ComboBoxProduct.Text = OtcDetail.Instance.InventoryOtc.Name;
            LabelStock.Content = OtcDetail.Instance.InventoryOtc.Stock.Inventory;
            AterDemolition = OtcDetail.Instance.InventoryOtc.Stock.Inventory.ToString();
        }

        private void TextAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if( String.IsNullOrEmpty((sender as TextBox).Text) ) return;
            AterDemolition = (Convert.ToInt32(OtcDetail.Instance.InventoryOtc.Stock.Inventory) - Convert.ToInt32((sender as TextBox).Text)).ToString();
        }

        private void ButtonSubnmmit_Click(object sender, RoutedEventArgs e)
        {
            ProductDb.DemolitionProduct();
        }
    }
}
