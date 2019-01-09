using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.StoreOrder;
using His_Pos.FunctionWindow;
using His_Pos.Struct.StoreOrder;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchase.AddNewOrderTypeControl
{
    /// <summary>
    /// ReturnTypeControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnTypeControl : UserControl
    {
        public Manufactory SelectedManufactory { get; set; }
        public WareHouse SelectedWareHouse { get; set; }
        public string SelectedOrderId { get; set; }
        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection { get; }
        
        public ObservableCollection<WareHouse> WareHouseComboCollection { get; }

        public Collection<StoreOrderOverview> StoreOrderOverviewCollection { get; }

        public ReturnTypeControl(ObservableCollection<Manufactory> manufactoryAutoCompleteCollection, ObservableCollection<WareHouse> wareHouseComboCollection)
        {
            InitializeComponent();
            DataContext = this;

            ManufactoryAutoCompleteCollection = manufactoryAutoCompleteCollection;
            WareHouseComboCollection = wareHouseComboCollection;

            StoreOrderOverviewCollection = StoreOrderDb.GetStoreOrderOverview();

            WareHouseCombo.SelectedIndex = 0;
        }
        
        internal AddOrderType GetOrderType()
        {
            if (WareHouseCombo.SelectedItem is null)
            {
                MessageWindow messageWindow = new MessageWindow("請輸入庫存名稱", MessageType.ERROR, true);
                messageWindow.ShowDialog();
                return AddOrderType.ERROR;
            }

            SelectedWareHouse = (WareHouse)WareHouseCombo.SelectedItem;

            if ((bool)TargetManufactory.IsChecked)
            {
                if(SelectedManufactory is null)
                {
                    MessageWindow messageWindow = new MessageWindow("請輸入廠商名稱", MessageType.ERROR, true);
                    messageWindow.ShowDialog();

                    return AddOrderType.ERROR;
                }
                else
                    return AddOrderType.RETURNBYMANUFACTORY;
            }
            else if((bool)TargetOrder.IsChecked)
            {
                if (OrderOverviewDataGrid.SelectedItem is null)
                {
                    MessageWindow messageWindow = new MessageWindow("請選擇單號", MessageType.ERROR, true);
                    messageWindow.ShowDialog();

                    return AddOrderType.ERROR;
                }
                else
                {
                    SelectedOrderId = ((StoreOrderOverview)OrderOverviewDataGrid.SelectedItem).Id;
                    return AddOrderType.RETURNBYORDER;
                }
            }
            
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

        public AutoCompleteFilterPredicate<object> ManFilter
        {
            get
            {
                return (searchText, obj) =>
                    !((obj as Manufactory)?.Id is null) && (((Manufactory)obj).Id.ToLower().Contains(searchText.ToLower())
                                                                || ((Manufactory)obj).Name.ToLower().Contains(searchText.ToLower()));
            }
        }

        private void OrderOverview_GotFocus(object sender, RoutedEventArgs e)
        {
            TargetOrder.IsChecked = true;
        }
    }
}
