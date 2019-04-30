﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.Class.Product;
using His_Pos.Class.StockTakingOrder;
using His_Pos.FunctionWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking;
namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.InventoryManagement.OtcControl
{
    /// <summary>
    /// OtcDetail.xaml 的互動邏輯
    /// </summary>
    public partial class OtcDetail : UserControl, INotifyPropertyChanged
    {
        public class WareStcok {
            public WareStcok(DataRow row ) {
                warId = row["PROWAR_ID"].ToString();
                warName = row["PROWAR_NAME"].ToString();
                warStock = row["PRO_INVENTORY"].ToString();
            }
           public string warId { get; set; }
           public string warName { get; set; }
           public string warStock { get; set; }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        //public SeriesCollection SalesCollection { get; set; }
        public string[] Months { get; set; }

        public ObservableCollection<CusOrderOverview> CusOrderOverviewCollection;
        public ObservableCollection<OTCStoreOrderOverview> StoreOrderOverviewCollection;
        public ObservableCollection<OTCStockOverview> OTCStockOverviewCollection;
        //public ObservableCollection<ProductDetailManufactory> OTCManufactoryCollection;
        //public HashSet<ManufactoryChanged> OTCManufactoryChangedCollection = new HashSet<ManufactoryChanged>();
        public ObservableCollection<ProductUnit> OTCUnitCollection;
        public ObservableCollection<string> OTCUnitChangdedCollection = new ObservableCollection<string>();
        public ObservableCollection<StockTakingOverview> StockTakingOverviewCollection;
        public ListCollectionView ProductTypeCollection;
        private ObservableCollection<ProductGroup> productGroupCollection;
        public ObservableCollection<ProductGroup> ProductGroupCollection {
            get { return productGroupCollection; }
            set
            {
                productGroupCollection = value;
                NotifyPropertyChanged("ProductGroupCollection");
            }

        }
        private ObservableCollection<WareStcok> wareStcokCollection;
        public ObservableCollection<WareStcok> WareStcokCollection
        {
            get { return wareStcokCollection; }
            set
            {
                wareStcokCollection = value;
                NotifyPropertyChanged("WareStcokCollection");
            }

        }
        public InventoryOtc InventoryOtc;
        private bool IsChanged = false;
        private bool IsFirst = true;
        public OtcDetail()
        {
            InitializeComponent();

            InventoryOtc = (InventoryOtc)ProductDetail.NewProduct;
            ProductDetail.NewProduct = null;

            UpdateUi();
            CheckAuth();

            IsFirst = false;
            DataContext = this;
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
            //SalesCollection = new SeriesCollection();
            //SalesCollection.Add(GetSalesLineSeries());
            AddMonths();
        }
        //private LineSeries GetSalesLineSeries()
        //{
        //    ChartValues<double> chartValues = null;///OTCDb.GetOtcSalesByID(InventoryOtc.Id);

        //    return new LineSeries
        //    {
        //        Title = "銷售量",
        //        Values = chartValues,
        //        PointGeometrySize = 10,
        //        LineSmoothness = 0,
        //        DataLabels = true
        //    };
        //}

        private void AddMonths()
        {
            DateTime today = DateTime.Today.Date;

            Months = new string[12];
            for (int x = 0; x < 12; x++)
            {
                Months[x] = today.AddMonths(-11 + x).Date.ToString("yyyy/MM"); 
            }
        }

        public void UpdateUi()
        {
            if (InventoryOtc is null) return;

            OtcId.Content = InventoryOtc.Id;
            OtcChiName.Text = InventoryOtc.ChiName.Trim();
            OtcEngName.Text = InventoryOtc.EngName.Trim();

            OtcStatus.Text = (InventoryOtc.Status)? "啟用":"已停用";
            OtcSafeAmount.Text = InventoryOtc.Stock.SafeAmount;
            OtcBasicAmount.Text = InventoryOtc.Stock.BasicAmount;
            OtcLocation.Text = InventoryOtc.Location;

            OTCNotes.Document.Blocks.Clear();
            OTCNotes.AppendText(InventoryOtc.Note.Trim());

            CusOrderOverviewCollection = null;/// OTCDb.GetOtcCusOrderOverviewByID(InventoryOtc.Id);
            OtcCusOrder.ItemsSource = CusOrderOverviewCollection;

            StoreOrderOverviewCollection = null;/// OTCDb.GetOtcStoOrderByID(InventoryOtc.Id);
            OtcStoOrder.ItemsSource = StoreOrderOverviewCollection;

            OTCStockOverviewCollection = null;/// ProductDb.GetProductStockOverviewById(InventoryOtc.Id);
            OtcStock.ItemsSource = OTCStockOverviewCollection;
            UpdateStockOverviewInfo();

            OTCUnitCollection = null;/// ProductDb.GetProductUnitById(InventoryOtc.Id);

            //OTCManufactoryCollection = null;/// ManufactoryDb.GetManufactoryCollection(InventoryOtc.Id);
            //OtcManufactory.ItemsSource = OTCManufactoryCollection;

            ProductTypeCollection = null;/// ProductDb.GetProductType();
            OtcType.ItemsSource = ProductTypeCollection;
            if(OtcType.Items.Contains(InventoryOtc.ProductType.Name))
                 OtcType.SelectedValue = InventoryOtc.ProductType.Name;

            StockTakingOverviewCollection = null;/// ProductDb.GetProductStockTakingDate(InventoryOtc.Id);
            if (StockTakingOverviewCollection.Count != 0)
                LastCheckTime.Content = StockTakingOverviewCollection[0].StockTakingDate;

            WareStcokCollection = null;///WareHouseDb.GetWareStockById(InventoryOtc.Id);
            ProductGroupCollection = null;///ProductDb.GetProductGroup(InventoryOtc.Id, InventoryOtc.WareHouseId);
             
            UpdateChart();
            InitVariables();
            SetUnitValue();
        }
        
        private void InitVariables()
        {
            IsChangedLabel.Content = "未修改";
            IsChangedLabel.Foreground = (Brush)FindResource("ForeGround");
            ButtonCancel.IsEnabled = false;
            ButtonUpdateSubmmit.IsEnabled = false;
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

            //if ( selectedItem is CusOrderOverview )
            //    OtcCusOrder.SelectedItem = selectedItem;
            //else if( selectedItem is OTCStoreOrderOverview)
            //    OtcStoOrder.SelectedItem = selectedItem;
            //else if (selectedItem is OTCStockOverview)
            //    OtcStock.SelectedItem = selectedItem;
            //else if (selectedItem is ProductDetailManufactory)
            //    OtcManufactory.SelectedItem = selectedItem;

        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;
            //if (leaveItem is CusOrderOverview)
            //    OtcCusOrder.SelectedItem = null;
            //else if (leaveItem is OTCStoreOrderOverview)
            //    OtcStoOrder.SelectedItem = null;
            //else if (leaveItem is OTCStockOverview)
            //    OtcStock.SelectedItem = null;
            //else if (leaveItem is ProductDetailManufactory)
            //    OtcManufactory.SelectedItem = null;
        }
        
        private void setChangedFlag()
        {
            IsChanged = true;
            IsChangedLabel.Content = "已修改";
            ButtonCancel.IsEnabled = true;
            ButtonUpdateSubmmit.IsEnabled = true;
            IsChangedLabel.Foreground = Brushes.Red;
        }

        private bool ChangedFlagNotChanged()
        {
            return !IsChanged;
        }
      
        private void SetUnitValue() {
            //int count = 0;
            //string index = "";
            //foreach (var row in OTCUnitCollection) {
            //    index = count.ToString();
            //    ((TextBox)DockUnit.FindName("OtcUnitName" + index)).Text = row.Unit;
            //    ((TextBox)DockUnit.FindName("OtcUnitAmount" + index)).Text = row.Amount;
            //    ((TextBox)DockUnit.FindName("OtcUnitPrice" + index)).Text = row.Price;
            //    ((TextBox)DockUnit.FindName("OtcUnitVipPrice" + index)).Text = row.VIPPrice;
            //    ((TextBox)DockUnit.FindName("OtcUnitEmpPrice" + index)).Text = row.EmpPrice;
            //    count++;
            //}
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
                MessageWindow.ShowMessage(errors, MessageType.ERROR);
                
            }
            return check;
        }
        private void ButtonUpdateSubmmit_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ChangedFlagNotChanged()) return;
            if (!CheckValue()) return;
            InventoryOtc.ChiName = OtcChiName.Text;
            InventoryOtc.EngName = OtcEngName.Text;
            InventoryOtc.Name = InventoryOtc.EngName.Contains(" ") ? InventoryOtc.EngName.Split(' ')[0] + " " + InventoryOtc.EngName.Split(' ')[1] + "... " + InventoryOtc.ChiName : InventoryOtc.ChiName;
            InventoryOtc.Location = OtcLocation.Text;
            InventoryOtc.Stock.BasicAmount = OtcBasicAmount.Text;
            InventoryOtc.Stock.SafeAmount = OtcSafeAmount.Text;
            InventoryOtc.Note = new TextRange(OTCNotes.Document.ContentStart, OTCNotes.Document.ContentEnd).Text;
            InventoryOtc.Status = OtcStatus.Text =="啟用" ? true : false;
            ///ProductDb.UpdateOtcDataDetail(InventoryOtc, "InventoryOtc");

            //foreach (var manufactoryChanged in OTCManufactoryChangedCollection)
            //{
            //    ///ManufactoryDb.UpdateProductManufactory(InventoryOtc.Id, manufactoryChanged);
            //}
            foreach (string index in OTCUnitChangdedCollection)
            {
                ProductUnit prounit = new ProductUnit("s");
                ///ProductDb.UpdateOtcUnit(prounit, InventoryOtc.Id);
            }
            MessageWindow.ShowMessage("商品修改成功!", MessageType.SUCCESS);
            
            InitVariables();
        }
        
        private void DataGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            //var selectitem = OtcStoOrder.SelectedItem;
            //ProductPurchaseRecordView.Proid = ((OTCStoreOrderOverview)selectitem).StoreOrderId;
            //MainWindow.Instance.AddNewTab("處理單紀錄");
        }

        private void OTCNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsChangedLabel is null || IsFirst)
                return;
            if (ChangedFlagNotChanged())
                setChangedFlag();
        }

        private void LastCheckTime_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StockTakingHistory stockTakingHistory = new StockTakingHistory(StockTakingOverviewCollection);

            stockTakingHistory.Show();
        }

        private void ButtonMergeStock_Click(object sender, RoutedEventArgs e)
        {
            MergeStockWindow mergeStockWindow = new MergeStockWindow(InventoryOtc);
            mergeStockWindow.ShowDialog();
            UpdateUi();
        }

        private void ButtonDemolition_Click(object sender, RoutedEventArgs e)
        {
            DemolitionWindow demolitionWindow = new DemolitionWindow(ProductGroupCollection, InventoryOtc);
            demolitionWindow.ShowDialog();
            UpdateUi();
        }

        private void OtcWareHouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OtcStock.Items.Filter = product => ((OTCStockOverview)product).warId == ((WareStcok)(sender as DataGrid).SelectedItem).warId;
        }
    }
}