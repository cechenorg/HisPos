using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.ProductPurchaseRecord;
using System.ComponentModel;
using System.Data;
using System.Windows.Media.Imaging;
using His_Pos.Class.StockTakingOrder;
using His_Pos.H2_STOCK_MANAGE.InventoryManagement;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// MedicineDetail.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineDetail : UserControl, INotifyPropertyChanged
    {
        #region ----- Declare Struct -----
        public struct InventoryMedicineDetail
        {
            public InventoryMedicineDetail(DataRow dataRow)
            {
                NHIPrice = dataRow[""].ToString();
                PackageAmount = dataRow[""].ToString();
                SingdePackagePrice = dataRow[""].ToString();
                SindePrice = dataRow[""].ToString();
                Form = dataRow[""].ToString();
                ATC = dataRow[""].ToString();
                Manufactory = dataRow[""].ToString();
                SC = dataRow[""].ToString();
            }

            public string NHIPrice { get; }
            public string PackageAmount { get; }
            public string SingdePackagePrice { get; }
            public string SindePrice { get; }
            public string Form { get; }
            public string ATC { get; }
            public string Manufactory { get; }
            public string SC { get; }
        }

        public struct InventoryDetailOverview
        {
            public InventoryDetailOverview(DataRow dataRow)
            {
                Id = dataRow[""].ToString();
                TypeIcon = dataRow[""].ToString();
                Type = dataRow[""].ToString();
                Time = dataRow[""].ToString();
                Principal = dataRow[""].ToString();
                Amount = dataRow[""].ToString();
                StockValueChange = dataRow[""].ToString();
            }

            public string Id { get; }
            public string TypeIcon { get; }
            public string Type { get; }
            public string Time { get; }
            public string Principal { get; }
            public string Amount { get; }
            public string StockValueChange { get; }
        }
        #endregion

        #region ----- Define Variables -----

        public InventoryMedicine InventoryMedicine { get; set; }
        private InventoryMedicine inventoryMedicine { get; }

        private ObservableCollection<InventoryDetailOverview> inventoryDetailOverviews;

        private ObservableCollection<InventoryDetailOverview> InventoryDetailOverviews
        {
            get { return inventoryDetailOverviews; }
            set
            {
                inventoryDetailOverviews = value;
                NotifyPropertyChanged("InventoryDetailOverviews");
            }
        }

        private ObservableCollection<ProductGroup> productGroupCollection;
        public ObservableCollection<ProductGroup> ProductGroupCollection
        {
            get { return productGroupCollection; }
            set
            {
                productGroupCollection = value;
                NotifyPropertyChanged("ProductGroupCollection");
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
        #endregion

        public MedicineDetail()
        {
            InitializeComponent();
            DataContext = this;

            InventoryMedicine = (InventoryMedicine)ProductDetail.NewProduct;
            inventoryMedicine = ((ICloneable)ProductDetail.NewProduct).Clone() as InventoryMedicine;
            ProductDetail.NewProduct = null;

            ProductGroupCollection = ProductDb.GetProductGroup(InventoryMedicine.Id, InventoryMedicine.WareHouseId);
        }
        
    }
}
