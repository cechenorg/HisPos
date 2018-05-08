using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
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
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using His_Pos.Interface;
using His_Pos.ProductPurchase;
using His_Pos.ProductPurchaseRecord;
using His_Pos.ViewModel;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// OtcDetail.xaml 的互動邏輯
    /// </summary>
    public partial class OtcDetail : UserControl
    {
        public SeriesCollection SalesCollection { get; set; }
        public string[] Months { get; set; }

        public ObservableCollection<CusOrderOverview> CusOrderOverviewCollection;
        public ObservableCollection<OTCStoreOrderOverview> StoreOrderOverviewCollection;
        public ObservableCollection<OTCStockOverview> OTCStockOverviewCollection;
        public ObservableCollection<ProductDetailManufactory> OTCManufactoryCollection;
        public HashSet<ManufactoryChanged> OTCManufactoryChangedCollection = new HashSet<ManufactoryChanged>();
        public ObservableCollection<ProductUnit> OTCUnitCollection;
        public ObservableCollection<string> OTCUnitChangdedCollection = new ObservableCollection<string>();
        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection = new ObservableCollection<Manufactory>();
        public ListCollectionView ProductTypeCollection;
        public event MouseButtonEventHandler mouseButtonEventHandler;
       
        public InventoryOtc InventoryOtc;
        private bool IsChanged = false;
        private bool IsFirst = true;

        public OtcDetail(InventoryOtc inventoryOtc)
        {
            InitializeComponent();

            
            
            InventoryOtc = inventoryOtc;
            
            UpdateUi();
            CheckAuth();
            
            IsFirst = false;
        }
        

        private void ChangedCancelButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUi();
            CheckAuth();
        }
        private void CheckAuth()
        {
            
        }

        private void UpdateChart()
        {
            SalesCollection = new SeriesCollection();
            SalesCollection.Add(GetSalesLineSeries());
            AddMonths();
        }

        private LineSeries GetSalesLineSeries()
        {
            ChartValues<double> chartValues = OTCDb.GetOtcSalesByID(InventoryOtc.Id);

            return new LineSeries
            {
                Title = "銷售量",
                Values = chartValues,
                PointGeometrySize = 10,
                LineSmoothness = 0,
                DataLabels = true
            };
        }

        private void AddMonths()
        {
            DateTime today = DateTime.Today.Date;

            Months = new string[12];
            for (int x = 0; x < 12; x++)
            {
                Months[x] = today.AddMonths(-11 + x).Date.ToString("yyyy/MM"); 
            }
        }

        private void UpdateUi()
        {
            if (InventoryOtc is null) return;

            OtcName.Content = InventoryOtc.Name;
            OtcId.Content = InventoryOtc.Id;
            
            OtcStatus.Text = (InventoryOtc.Status)? "啟用":"已停用";
            OtcSafeAmount.Text = InventoryOtc.Stock.SafeAmount;
            OtcBasicAmount.Text = InventoryOtc.Stock.BasicAmount;
            OtcLocation.Text = InventoryOtc.Location;

            OTCNotes.Document.Blocks.Clear();
            OTCNotes.AppendText(InventoryOtc.Note.Trim());

            CusOrderOverviewCollection = OTCDb.GetOtcCusOrderOverviewByID(InventoryOtc.Id);
            OtcCusOrder.ItemsSource = CusOrderOverviewCollection;

            StoreOrderOverviewCollection = OTCDb.GetOtcStoOrderByID(InventoryOtc.Id);
            OtcStoOrder.ItemsSource = StoreOrderOverviewCollection;

            OTCStockOverviewCollection = ProductDb.GetProductStockOverviewById(InventoryOtc.Id);
            OtcStock.ItemsSource = OTCStockOverviewCollection;
            UpdateStockOverviewInfo();

            OTCUnitCollection = ProductDb.GetProductUnitById(InventoryOtc.Id);
            
            OTCManufactoryCollection = ManufactoryDb.GetManufactoryCollection(InventoryOtc.Id);
            OtcManufactory.ItemsSource = OTCManufactoryCollection;

            ProductTypeCollection = ProductDb.GetProductType();
            OtcType.ItemsSource = ProductTypeCollection;
            if(OtcType.Items.Contains(InventoryOtc.ProductType.Name))
                 OtcType.SelectedValue = InventoryOtc.ProductType.Name;
            
            UpdateChart();
            InitVariables();
            SetUnitValue();
        }
        
        private void InitVariables()
        {
            IsChangedLabel.Content = "未修改";
           // IsChangedLabel.Foreground = (Brush)FindResource("ForeGround");
            
            IsChanged = false;
        }

        private void UpdateStockOverviewInfo()
        {
            TotalStock.Content = InventoryOtc.Stock.Inventory.ToString();
            StockTotalPrice.Content = "$" + InventoryOtc.StockValue;

        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if ( selectedItem is CusOrderOverview )
                OtcCusOrder.SelectedItem = selectedItem;
            else if( selectedItem is OTCStoreOrderOverview)
                OtcStoOrder.SelectedItem = selectedItem;
            else if (selectedItem is OTCStockOverview)
                OtcStock.SelectedItem = selectedItem;
            else if (selectedItem is ProductDetailManufactory)
                OtcManufactory.SelectedItem = selectedItem;

        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;
            if (leaveItem is CusOrderOverview)
                OtcCusOrder.SelectedItem = null;
            else if (leaveItem is OTCStoreOrderOverview)
                OtcStoOrder.SelectedItem = null;
            else if (leaveItem is OTCStockOverview)
                OtcStock.SelectedItem = null;
            else if (leaveItem is ProductDetailManufactory)
                OtcManufactory.SelectedItem = null;
        }
        
        private void setChangedFlag()
        {
            IsChanged = true;
            IsChangedLabel.Content = "已修改";
            IsChangedLabel.Foreground = Brushes.Red;
        }

        private bool ChangedFlagNotChanged()
        {
            return !IsChanged;
        }
      
        private void SetUnitValue() {
            int count = 0;
            string index = "";
            foreach (var row in OTCUnitCollection) {
                index = count.ToString();
                ((TextBox)DockUnit.FindName("OtcUnitName" + index)).Text = row.Unit;
                ((TextBox)DockUnit.FindName("OtcUnitAmount" + index)).Text = row.Amount;
                ((TextBox)DockUnit.FindName("OtcUnitPrice" + index)).Text = row.Price;
                ((TextBox)DockUnit.FindName("OtcUnitVipPrice" + index)).Text = row.VIPPrice;
                ((TextBox)DockUnit.FindName("OtcUnitEmpPrice" + index)).Text = row.EmpPrice;
                count++;
            }
        }
      
        private void SetOtcTextBoxChangedCollection(string name) {
            if (ChangedFlagNotChanged())
                setChangedFlag();
            if (name.Contains("OtcUnit")) {
                string index = name.Substring(name.Length - 1, 1);
                if (!OTCUnitChangdedCollection.Contains(index))
                    OTCUnitChangdedCollection.Add(index);
            }
        }
        private void OtcData_TextChanged(object sender,EventArgs e)
        {
            if (IsChangedLabel is null || IsFirst)
                return;
            if(sender is TextBox){
                TextBox txt = sender as TextBox;
                SetOtcTextBoxChangedCollection(txt.Name);
            }
            if (sender is ComboBox) {
                ComboBox txt = sender as ComboBox;
                SetOtcTextBoxChangedCollection(txt.Name);
            }
        }
        private bool CheckValue() {
            List<string> _errorList = new List<string>();
            bool check = true;
            
            if (Convert.ToInt32(OtcBasicAmount.Text) < Convert.ToInt32(OtcSafeAmount.Text)) {
                _errorList.Add("安全量不可高於基準量");
                check = false;
            }
            if (!check) {
                var errors = _errorList.Aggregate(string.Empty, (current, error) => current + (error + "\n"));
                MessageWindow messageWindow = new MessageWindow(errors, MessageType.ERROR);
                messageWindow.ShowDialog();
            }
            return check;
        }
        private void ButtonUpdateSubmmit_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ChangedFlagNotChanged()) return;
            if (!CheckValue()) return;
            InventoryOtc.Location = OtcLocation.Text;
            InventoryOtc.Stock.BasicAmount = OtcBasicAmount.Text;
            InventoryOtc.Stock.SafeAmount = OtcSafeAmount.Text;
            InventoryOtc.Note = new TextRange(OTCNotes.Document.ContentStart, OTCNotes.Document.ContentEnd).Text;
            InventoryOtc.Status = OtcStatus.Text =="啟用" ? true : false;
            ProductDb.UpdateOtcDataDetail(InventoryOtc, "InventoryOtc");

            foreach (var manufactoryChanged in OTCManufactoryChangedCollection)
            {
                ManufactoryDb.UpdateProductManufactory(InventoryOtc.Id, manufactoryChanged);
            }
            foreach (string index in OTCUnitChangdedCollection)
            {
                ProductUnit prounit = new ProductUnit(Convert.ToInt32(index), ((TextBox)DockUnit.FindName("OtcUnitName" + index)).Text,
                                         ((TextBox)DockUnit.FindName("OtcUnitAmount" + index)).Text, ((TextBox)DockUnit.FindName("OtcUnitPrice" + index)).Text,
                                          ((TextBox)DockUnit.FindName("OtcUnitVipPrice" + index)).Text, ((TextBox)DockUnit.FindName("OtcUnitEmpPrice" + index)).Text);
                ProductDb.UpdateOtcUnit(prounit, InventoryOtc.Id);
            }
            InitVariables();
            MouseButtonEventHandler handler = mouseButtonEventHandler;

            handler(this, e);
        }
        
        private void DataGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectitem = OtcStoOrder.SelectedItem;
            ProductPurchaseRecordView.Proid = ((OTCStoreOrderOverview)selectitem).StoreOrderId;
            MainWindow.Instance.AddNewTab("處理單紀錄");
        }

        private void OTCNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsChangedLabel is null || IsFirst)
                return;
            if (ChangedFlagNotChanged())
                setChangedFlag();
        }
    }
}