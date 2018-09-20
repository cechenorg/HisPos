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
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using His_Pos.Class.StoreOrder;
using His_Pos.ProductPurchase;
using His_Pos.Struct.Manufactory;
using His_Pos.Struct.Product;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl
{
    /// <summary>
    /// ReturnControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnControl : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Variables -----
        public Collection<PurchaseProduct> ProductCollection { get; set; }

        private Collection<PurchasePrincipal> principalCollection;
        public Collection<PurchasePrincipal> PrincipalCollection
        {
            get
            {
                return principalCollection;
            }
            set
            {
                principalCollection = value;
                NotifyPropertyChanged("PrincipalCollection");
            }
        }
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

        public DataGrid CurrentDataGrid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public ReturnControl()
        {
            InitializeComponent();

            DataContext = this;

            CurrentDataGrid = PStoreOrderDetail;
        }

        internal void SetDataContext(StoreOrder storeOrder)
        {
            StoreOrderData = storeOrder;

            InitPrincipal();

            UpdateOrderDetailUi();

            StoreOrderData.IsDataChanged = false;
        }

        private void InitPrincipal()
        {
            PrincipalCollection = ManufactoryDb.GetPrincipal(StoreOrderData.Manufactory.Id);

            if (StoreOrderData.Principal.Id == "")
                PrincipalCombo.SelectedIndex = 0;
        }

        #region ----- DataGrid Functions -----
        private void UpdateOrderDetailUi()
        {
            CurrentDataGrid.ItemsSource = null;

            switch (StoreOrderData.Type)
            {
                case OrderType.PROCESSING:
                    MainGrid.RowDefinitions[3].Height = new GridLength(0);
                    MainGrid.RowDefinitions[4].Height = new GridLength(1, GridUnitType.Star);
                    MainGrid.RowDefinitions[5].Height = new GridLength(0);
                    MainGrid.RowDefinitions[6].Height = new GridLength(50);

                    CurrentDataGrid = GStoreOrderDetail;
                    break;
                case OrderType.UNPROCESSING:
                    MainGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
                    MainGrid.RowDefinitions[4].Height = new GridLength(0);
                    MainGrid.RowDefinitions[5].Height = new GridLength(50);
                    MainGrid.RowDefinitions[6].Height = new GridLength(0);

                    CurrentDataGrid = PStoreOrderDetail;
                    break;
            }

            CurrentDataGrid.ItemsSource = StoreOrderData.Products;

            UpdatePricipalStackUi();
        }
        private void UpdatePricipalStackUi()
        {
            if (StoreOrderData.Principal.Name.Equals("新增負責人"))
            {
                HasPrincipalStack.Visibility = Visibility.Collapsed;
                DontHasPrincipalStack.Visibility = Visibility.Visible;
            }
            else
            {
                HasPrincipalStack.Visibility = Visibility.Visible;
                DontHasPrincipalStack.Visibility = Visibility.Collapsed;
            }
        }
        
        internal void ClearControl()
        {
            StoreOrderData = null;
            CurrentDataGrid.ItemsSource = null;
        }
        #endregion

        private void NewProduct(object sender, RoutedEventArgs e)
        {
            NewItemDialog newItemDialog = new NewItemDialog(ProductCollection, StoreOrderData.Manufactory.Id, StoreOrderData.Warehouse.Id);

            newItemDialog.ShowDialog();

            if (newItemDialog.ConfirmButtonClicked)
            {
                //SetChanged();
                if (newItemDialog.SelectedItem.Type.Equals("M"))
                    StoreOrderData.Products.Add(new ProductPurchaseMedicine(newItemDialog.SelectedItem));
                else
                    StoreOrderData.Products.Add(new ProductPurchaseOtc(newItemDialog.SelectedItem));
            }
        }

        private void Principal_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePricipalStackUi();

            StoreOrderData.IsDataChanged = true;
        }

        private void ShowDeclareDataOverview(object sender, MouseButtonEventArgs e)
        {
            DeclareDataDetailOverview declareDataDetailOverview = new DeclareDataDetailOverview();
            declareDataDetailOverview.Show();
        }
    }
}
