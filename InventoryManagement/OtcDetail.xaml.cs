﻿using System;
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
using His_Pos.ViewModel;
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
        public ObservableCollection<ProductDetailManufactory> OTCManufactoryCollection;
        public HashSet<ManufactoryChanged> OTCManufactoryChangedCollection = new HashSet<ManufactoryChanged>();
        public ObservableCollection<ProductUnit> OTCUnitCollection;
        public ObservableCollection<string> OTCUnitChangdedCollection = new ObservableCollection<string>();
        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection = new ObservableCollection<Manufactory>();

        public event MouseButtonEventHandler mouseButtonEventHandler;
       
        public InventoryOtc InventoryOtc;
        private bool IsChanged = false;
        private bool IsFirst = true;
        private int LastSelectedIndex = -1;

        public OtcDetail(InventoryOtc inventoryOtc)
        {
            InitializeComponent();

            InventoryOtc = inventoryOtc;
            
            UpdateUi();
            CheckAuth();
            
            IsFirst = false;
            DataContext = this;
        }

        private void OtcManufactoryCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ProductDetailManufactory OldProductDetailManufactory = (e.OldItems is null)? null: e.OldItems[0] as ProductDetailManufactory;
            ProductDetailManufactory NewProductDetailManufactory = (e.NewItems is null) ? null : e.NewItems[0] as ProductDetailManufactory;

            if (ChangedFlagNotChanged())
                setChangedFlag();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    return;
                case NotifyCollectionChangedAction.Replace:
                    if (NewProductDetailManufactory.OrderId is null && OldProductDetailManufactory.Id is null)
                        OTCManufactoryChangedCollection.Add(new ManufactoryChanged(NewProductDetailManufactory, ProcedureProcessType.INSERT));
                    else
                    {
                        OTCManufactoryChangedCollection.Add(new ManufactoryChanged(NewProductDetailManufactory, ProcedureProcessType.UPDATE));
                        ManufactoryAutoCompleteCollection.Add(OldProductDetailManufactory);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OTCManufactoryChangedCollection.RemoveWhere(DeleteManufactoryChanged);
                    if (OldProductDetailManufactory.OrderId != null)
                        OTCManufactoryChangedCollection.Add(new ManufactoryChanged(OldProductDetailManufactory, ProcedureProcessType.DELETE));
                    ManufactoryAutoCompleteCollection.Add(OldProductDetailManufactory);
                    break;
            }

            bool DeleteManufactoryChanged(ManufactoryChanged manufactoryChanged)
            {
                if (OldProductDetailManufactory.Id == manufactoryChanged.ManufactoryId)
                    return true;
                return false;
            }
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
            OtcSaveAmount.Text = InventoryOtc.Stock.SafeAmount;
            OtcBasicAmount.Text = InventoryOtc.Stock.BasicAmount;
            OtcLocation.Text = InventoryOtc.Location;

            OTCNotes.Document.Blocks.Clear();
            OTCNotes.AppendText(InventoryOtc.Note);

            CusOrderOverviewCollection = OTCDb.GetOtcCusOrderOverviewByID(InventoryOtc.Id);
            OtcCusOrder.ItemsSource = CusOrderOverviewCollection;

            StoreOrderOverviewCollection = OTCDb.GetOtcStoOrderByID(InventoryOtc.Id);
            OtcStoOrder.ItemsSource = StoreOrderOverviewCollection;

            OTCStockOverviewCollection = ProductDb.GetProductStockOverviewById(InventoryOtc.Id);
            OtcStock.ItemsSource = OTCStockOverviewCollection;
            UpdateStockOverviewInfo();

            OTCUnitCollection = ProductDb.GetProductUnitById(InventoryOtc.Id);
            
            OTCManufactoryCollection = ManufactoryDb.GetManufactoryCollection(InventoryOtc.Id);
            OTCManufactoryCollection.Add(new ProductDetailManufactory());
            OtcManufactory.ItemsSource = OTCManufactoryCollection;
            OTCManufactoryCollection.CollectionChanged += OtcManufactoryCollectionOnCollectionChanged;

            foreach (DataRow row in MainWindow.ManufactoryTable.Rows)
            {
                bool keep = true;

                foreach (var detailManufactory in OTCManufactoryCollection)
                {
                    if (row["MAN_ID"].ToString() == detailManufactory.Id)
                        keep = false;
                }

                if ( keep )
                    ManufactoryAutoCompleteCollection.Add(new Manufactory(row, DataSource.MANUFACTORY));
            }

            UpdateChart();
            InitVariables();
            SetUnitValue();
        }
        
        private void InitVariables()
        {
            IsChangedLabel.Content = "未修改";
            IsChangedLabel.Foreground = (Brush)FindResource("ForeGround");
            
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
            else if (selectedItem is IDeletable)
            {
                if (selectedItem != OTCManufactoryCollection.Last())
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";
                OtcManufactory.SelectedItem = selectedItem;
                LastSelectedIndex = OtcManufactory.SelectedIndex;
            }
            
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
            {
                (leaveItem as ProductDetailManufactory).Source = "";
            }
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

            if (ManufactoryAuto.ItemsSource is null)
                ManufactoryAuto.ItemsSource = ManufactoryAutoCompleteCollection;

            foreach (Manufactory manufactory in ManufactoryAutoCompleteCollection)
            {
                if (manufactory.Id == OTCManufactoryCollection[OTCManufactoryCollection.Count - 2].Id)
                {
                    ManufactoryAutoCompleteCollection.Remove(manufactory);
                    break;
                }
            }

            ManufactoryAuto.PopulateComplete();
        }
        private void OtcManufactoryAuto_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var ManufactoryAuto = sender as AutoCompleteBox;
            if (ManufactoryAuto is null) return;

            if ((ManufactoryAuto.Text is null || ManufactoryAuto.Text == String.Empty) && LastSelectedIndex != OTCManufactoryCollection.Count - 1)
            {
                OTCManufactoryCollection.RemoveAt(LastSelectedIndex);
            }
        }

        private void OtcManufactoryAuto_OnDropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            var ManufactoryAuto = sender as AutoCompleteBox;

            if (ManufactoryAuto is null) return;
            if (ManufactoryAuto.SelectedItem is null) return;

            if (OTCManufactoryCollection.Count == OtcManufactory.SelectedIndex + 1)
            {
                OTCManufactoryCollection[OtcManufactory.SelectedIndex] = new ProductDetailManufactory(ManufactoryAuto.SelectedItem as Manufactory);
                OTCManufactoryCollection.Add(new ProductDetailManufactory());
            }
            else
            {
                OTCManufactoryCollection[LastSelectedIndex] = new ProductDetailManufactory(ManufactoryAuto.SelectedItem as Manufactory);
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
      
        private void SetOtcTextBoxChangedCollection(string name) {
            if (ChangedFlagNotChanged())
                setChangedFlag();
            if (name.Contains("OtcUnit")) {
                string index = name.Substring(name.Length - 1, 1);
                if (!OTCUnitChangdedCollection.Contains(index))
                    OTCUnitChangdedCollection.Add(index);
            }
        }
        private void OtcData_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsChangedLabel is null || IsFirst)
                return;
            TextBox txt = sender as TextBox;
            SetOtcTextBoxChangedCollection(txt.Name);
        }

        private void ButtonUpdateSubmmit_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ChangedFlagNotChanged()) return;

            InventoryOtc.Location = OtcLocation.Text;
            InventoryOtc.Stock.BasicAmount = OtcBasicAmount.Text;
            InventoryOtc.Stock.SafeAmount = OtcSaveAmount.Text;
            InventoryOtc.Note = new TextRange(OTCNotes.Document.ContentStart, OTCNotes.Document.ContentEnd).Text;
            
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

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OTCManufactoryCollection.RemoveAt(OtcManufactory.SelectedIndex);
        }

        private void DataGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            OTCStoreOrderOverview otcStoreOrderOverview = sender as OTCStoreOrderOverview;

            MainWindow.MainWindowInstance.AddNewTab("處理單紀錄");
          
        }
    }
}