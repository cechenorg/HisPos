using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using His_Pos.Class;
using His_Pos.Class.Manufactory;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase.AddNewOrderTypeControl
{
    /// <summary>
    /// ReturnTypeControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnTypeControl : UserControl
    {
        public Manufactory SelectedManufactory { get; set; }
        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection { get; }
        
        public ObservableCollection<WareHouse> WareHouseComboCollection { get; }

        public ReturnTypeControl(ObservableCollection<Manufactory> manufactoryAutoCompleteCollection, ObservableCollection<WareHouse> wareHouseComboCollection)
        {
            InitializeComponent();
            DataContext = this;

            ManufactoryAutoCompleteCollection = manufactoryAutoCompleteCollection;
            WareHouseComboCollection = wareHouseComboCollection;
        }

        internal AddOrderType GetOrderType()
        {
            return AddOrderType.ERROR;
        }

        private void ManufactoryAuto_OnDropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;

            if (autoCompleteBox is null || autoCompleteBox.SelectedItem is null) return;

            SelectedManufactory = autoCompleteBox.SelectedItem as Manufactory;
        }

        private void ManufactoryAuto_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TargetManufactory.IsChecked = true;
        }
    }
}
