using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchase.AddNewOrderTypeControl;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchase
{
    /// <summary>
    /// AddNewOrderDialog.xaml 的互動邏輯
    /// </summary>
    public partial class AddNewOrderDialog : Window, INotifyPropertyChanged
    {
        private PurchaseTypeControl purchaseTypeControl;
        private ReturnTypeControl returnTypeControl;

        private UserControl currentControl;

        public UserControl CurrentControl
        {
            get
            {
                return currentControl;
            }
            set
            {
                currentControl = value;
                NotifyPropertyChanged("CurrentControl");
            }
        }

        public AddOrderType AddOrderType;
        public Manufactory SelectedManufactory;
        public WareHouse SelectedWareHouse;
        public string SelectedOrderId;

        public ObservableCollection<WareHouse> WareHouseCollection { get; }

        public bool ConfirmButtonClicked = false;
        public AddNewOrderDialog( ObservableCollection<Manufactory> manufactoryAutoCompleteCollection)
        {
            InitializeComponent();
            DataContext = this;

            WareHouseCollection = WareHouseDb.GetWareHouseData();

            purchaseTypeControl = new PurchaseTypeControl(manufactoryAutoCompleteCollection, WareHouseCollection);
            returnTypeControl = new ReturnTypeControl(manufactoryAutoCompleteCollection, WareHouseCollection);

            CurrentControl = purchaseTypeControl;
        }
        private void ConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = RadioStack.Children.OfType<RadioButton>().Single(r => (bool) r.IsChecked);

            switch (radioButton.Tag.ToString())
            {
                case "P":
                    AddOrderType = purchaseTypeControl.GetOrderType();
                    SelectedManufactory = purchaseTypeControl.SelectedManufactory;
                    SelectedWareHouse = purchaseTypeControl.SelectedWareHouse;
                    break;
                case "R":
                    AddOrderType = returnTypeControl.GetOrderType();
                    SelectedManufactory = returnTypeControl.SelectedManufactory;
                    SelectedOrderId = returnTypeControl.SelectedOrderId;
                    SelectedWareHouse = returnTypeControl.SelectedWareHouse;
                    break;
            }

            if (AddOrderType == AddOrderType.ERROR) return;

            ConfirmButtonClicked = true;
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void NewOrderType_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            switch(radioButton.Tag.ToString())
            {
                case "P":
                    CurrentControl = purchaseTypeControl;
                    break;
                case "R":
                    CurrentControl = returnTypeControl;
                    break;
            }
        }
    }
}
