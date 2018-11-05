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
using His_Pos.Struct.Product;

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
                NHIPrice = dataRow["HISMED_PRICE"].ToString();
                PackageAmount = dataRow["PRO_PACKAGEQTY"].ToString();
                SingdePackagePrice = dataRow["PRO_SPACKAGEPRICE"].ToString();
                SindePrice = dataRow["PRO_SPRICE"].ToString();
                Form = dataRow["HISMED_FORM"].ToString();
                ATC = dataRow["HISMED_ATC"].ToString();
                Manufactory = dataRow["HISMED_MANUFACTORY"].ToString();
                SC = "單方";
                Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
            }

            public string NHIPrice { get; }
            public string PackageAmount { get; }
            public string SingdePackagePrice { get; }
            public string SindePrice { get; }
            public string Form { get; }
            public string ATC { get; }
            public string Manufactory { get; }
            public string SC { get; }
            public string Ingredient { get; }
        }
        #endregion

        #region ----- Define Variables -----

        public InventoryMedicine InventoryMedicine { get; set; }
        private InventoryMedicine inventoryMedicine { get; }

        public InventoryMedicineDetail MedicineDetails { get; }

        private ObservableCollection<InventoryDetailOverview> inventoryDetailOverviews;

        public ObservableCollection<InventoryDetailOverview> InventoryDetailOverviews
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
            MedicineDetails = ProductDb.GetInventoryMedicineDetail(InventoryMedicine.Id);
            InventoryDetailOverviews = ProductDb.GetInventoryDetailOverviews(InventoryMedicine.Id);

            CalculateStock();

            SideEffectBox.AppendText(InventoryMedicine.SideEffect);
            IndicationBox.AppendText(InventoryMedicine.Indication);
            NoteBox.AppendText(InventoryMedicine.Note);
        }

        private void CalculateStock()
        {
            InventoryDetailOverviews[InventoryDetailOverviews.Count - 1].Stock = InventoryMedicine.Stock.Inventory;

            for (int x = InventoryDetailOverviews.Count - 2; x >= 0; x--)
            {
                InventoryDetailOverviews[x].Stock =
                    InventoryDetailOverviews[x + 1].Stock - InventoryDetailOverviews[x + 1].Amount;
            }
        }
    }
}
