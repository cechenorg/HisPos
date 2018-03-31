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
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// OtcDetail.xaml 的互動邏輯
    /// </summary>
    public partial class OtcDetail : Window
    {
        public SeriesCollection SalesCollection { get; set; }
        public string[] Months { get; set; }

        public ObservableCollection<CusOrderOverview> CusOrderOverviewCollection;
        public ObservableCollection<OTCStoreOrderOverview> StoreOrderOverviewCollection;
        public ObservableCollection<OTCStockOverview> OTCStockOverviewCollection;
        public ObservableCollection<Manufactory> OTCManufactoryCollection;
        public HashSet<int> OTCManufactoryChangedCollection = new HashSet<int>();
        public ObservableCollection<ProductUnit> OTCUnitCollection;
        public ObservableCollection<string> OTCUnitChangdedCollection = new ObservableCollection<string>();
        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection = new ObservableCollection<Manufactory>();

        public event MouseButtonEventHandler mouseButtonEventHandler;


        public Otc otc;
        private bool IsChanged = false;
        private bool IsFirst = true;

        public OtcDetail(string proId)
        {
            InitializeComponent();

            otc = OTCDb.GetOtcDetail(proId);
            
            UpdateUi();
            CheckAuth();

            OTCManufactoryCollection.CollectionChanged += OtcManufactoryCollectionOnCollectionChanged;

            IsFirst = false;
            DataContext = this;
        }

        private void OtcManufactoryCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if( e.Action == NotifyCollectionChangedAction.Add ) return;

            OTCManufactoryChangedCollection.Add(e.NewStartingIndex);
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
            ChartValues<double> chartValues = OTCDb.GetOtcSalesByID(otc.Id);

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
            if (otc is null) return;

            OtcName.Content = otc.Name;
            OtcId.Content = otc.Id;
            
            OtcType.Text = otc.Type;
            OtcStatus.Text = (otc.Status)? "啟用":"已停用";
            OtcSaveAmount.Text = otc.SafeAmount;
            OtcBasicAmount.Text = otc.BasicAmount;
            OtcLocation.Text = otc.Location;

            OTCNotes.Document.Blocks.Clear();
            OTCNotes.AppendText(otc.Note);

            CusOrderOverviewCollection = OTCDb.GetOtcCusOrderOverviewByID(otc.Id);
            OtcCusOrder.ItemsSource = CusOrderOverviewCollection;

            StoreOrderOverviewCollection = OTCDb.GetOtcStoOrderByID(otc.Id);
            OtcStoOrder.ItemsSource = StoreOrderOverviewCollection;

            OTCStockOverviewCollection = ProductDb.GetProductStockOverviewById(otc.Id);
            OtcStock.ItemsSource = OTCStockOverviewCollection;
            UpdateStockOverviewInfo();

            OTCUnitCollection = ProductDb.GetProductUnitById(otc.Id);

            OTCManufactoryCollection = GetManufactoryCollection();
            OTCManufactoryCollection.Add(new Manufactory());
            OtcManufactory.ItemsSource = OTCManufactoryCollection;
            
            UpdateChart();
            InitVariables();
            SetUnitValue();
        }

        private ObservableCollection<Manufactory> GetManufactoryCollection()
        {
            ObservableCollection<Manufactory> manufactories = new ObservableCollection<Manufactory>();

            var man = MainWindow.ProManTable.Select("PRO_ID = '" + otc.Id + "'");

            foreach (var m in man)
            {
                manufactories.Add(new Manufactory(m,DataSource.PROMAN));
            }

            return manufactories;
        }

        private void InitVariables()
        {
            IsChangedLabel.Content = "未修改";
            IsChangedLabel.Foreground = (Brush)FindResource("ForeGround");
            
            IsChanged = false;
        }

        private void UpdateStockOverviewInfo()
        {
            int totalStock = 0;
            double totalPrice = 0;

            foreach (var Otc in OTCStockOverviewCollection)
            {
                totalStock += Int32.Parse(Otc.Amount);
                totalPrice += Double.Parse(Otc.Price) * Int32.Parse(Otc.Amount);
            }

            TotalStock.Content = totalStock.ToString();
            StockTotalPrice.Content = "$" + totalPrice.ToString("0.00");

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
        private void OtcManufactoryAuto_OnPopulating(object sender, PopulatingEventArgs e)
        {
            var ManufactoryAuto = sender as AutoCompleteBox;

            if (ManufactoryAuto is null) return;

            var tmp = MainWindow.ManufactoryTable.Select("MAN_ID LIKE '%" + ManufactoryAuto.Text + "%' OR MAN_NAME LIKE '%" + ManufactoryAuto.Text + "%'");
            ManufactoryAutoCompleteCollection.Clear();
            foreach (var row in tmp)
            {
                ManufactoryAutoCompleteCollection.Add(new Manufactory(row, DataSource.MANUFACTORY));
            }
            ManufactoryAuto.ItemsSource = ManufactoryAutoCompleteCollection;
            ManufactoryAuto.PopulateComplete();
        }

        private void OtcManufactoryAuto_OnDropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            var ManufactoryAuto = sender as AutoCompleteBox;

            if (ManufactoryAuto is null) return;
            if (ManufactoryAuto.SelectedItem is null) return;

            if (OTCManufactoryCollection.Count == OtcManufactory.SelectedIndex + 1)
            {
                OTCManufactoryCollection[OtcManufactory.SelectedIndex] = (Manufactory) ManufactoryAuto.SelectedItem;
                OTCManufactoryCollection.Add(new Manufactory());
            }
            else
            {
                OTCManufactoryCollection[OtcManufactory.SelectedIndex] = (Manufactory)ManufactoryAuto.SelectedItem;
            }
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
        private void ButtonUpdateSubmmit_Click(object sender, RoutedEventArgs e)
        {
            OTCDb.UpdateOtcDataDetail(otc.Id,OtcSaveAmount.Text,OtcBasicAmount.Text ,OtcLocation.Text, new TextRange(OTCNotes.Document.ContentStart, OTCNotes.Document.ContentEnd).Text);
           
            foreach (var changedIndex in OTCManufactoryChangedCollection)
            {
                ProductDb.UpdateProductManufactory(otc.Id, OTCManufactoryCollection[changedIndex].Id, changedIndex);
            }
            foreach (string index in OTCUnitChangdedCollection) {
                ProductUnit prounit = new ProductUnit (Convert.ToInt32(index), ((TextBox)DockUnit.FindName("OtcUnitName" + index)).Text,
                                         ((TextBox)DockUnit.FindName("OtcUnitAmount" + index)).Text, ((TextBox)DockUnit.FindName("OtcUnitPrice" + index)).Text,
                                          ((TextBox)DockUnit.FindName("OtcUnitVipPrice" + index)).Text, ((TextBox)DockUnit.FindName("OtcUnitEmpPrice" + index)).Text);
                OTCDb.UpdateOtcUnit(prounit,otc.Id);
            }
            InitVariables();
        }
        private void SetOTCUnitChangedCollection(string name) {
            if (ChangedFlagNotChanged())
                setChangedFlag();
            string index = name.Substring(name.Length-1,1);
            if (!OTCUnitChangdedCollection.Contains(index))
                OTCUnitChangdedCollection.Add(index);
        }
        private void OtcData_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsChangedLabel is null || IsFirst)
                return;
            TextBox txt = sender as TextBox;
            SetOTCUnitChangedCollection(txt.Name);
        }

        private void ButtonUpdateSubmmit_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseButtonEventHandler handler = mouseButtonEventHandler;

            handler(this, e);
        }
    }
}