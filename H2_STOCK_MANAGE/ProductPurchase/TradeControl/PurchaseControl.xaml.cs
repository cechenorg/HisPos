using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Class;
using His_Pos.Class.StoreOrder;
using His_Pos.Interface;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl
{
    /// <summary>
    /// PurchaseControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseControl : UserControl, INotifyPropertyChanged
    {
        private StoreOrder storeOrderData;

        public StoreOrder StoreOrderData
        {
            get
            {
                return storeOrderData;
            }
            set
            {
                storeOrderData = value;
                NotifyPropertyChanged("StoreOrderData");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public PurchaseControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        internal void SetDataContext(StoreOrder storeOrder)
        {
            StoreOrderData = storeOrder;

            UpdateOrderDetailUi();

            Init
        }

        private void UpdateOrderDetailUi()
        {
            AddNewProduct.IsEnabled = true;
            DeleteOrder.IsEnabled = true;
            ConfirmToProcess.IsEnabled = true;
            Confirm.IsEnabled = true;

            switch (StoreOrderData.Type)
            {
                case OrderType.PROCESSING:
                    Confirm.Visibility = Visibility.Visible;
                    ConfirmToProcess.Visibility = Visibility.Collapsed;
                    DeleteOrder.Visibility = Visibility.Collapsed;
                    EmptySpace.Width = 400;
                    StoreOrderDetail.Columns[11].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[12].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[13].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[5].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[6].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[7].Visibility = Visibility.Collapsed;
                    break;
                case OrderType.UNPROCESSING:
                    Confirm.Visibility = Visibility.Collapsed;
                    ConfirmToProcess.Visibility = Visibility.Visible;
                    DeleteOrder.Visibility = Visibility.Visible;
                    EmptySpace.Width = 270;
                    StoreOrderDetail.Columns[11].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[12].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[13].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[5].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[6].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[7].Visibility = Visibility.Visible;
                    break;
            }
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is IDeletable)
            {
                if (StoreOrderData.Products.Contains(selectedItem))
                {
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";
                }

                StoreOrderDetail.SelectedItem = selectedItem;
                return;
            }

            StoreOrderDetail.SelectedIndex = StoreOrderData.Products.Count;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;

            if (leaveItem is IDeletable)
            {
                (leaveItem as IDeletable).Source = string.Empty;
            }
        }
    }
}
