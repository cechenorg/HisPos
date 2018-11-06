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
        
        private InventoryMedicine inventoryMedicineBackup { get; }

        public InventoryMedicineDetail MedicineDetails { get; set; }


        private InventoryMedicine inventoryMedicine;
        public InventoryMedicine InventoryMedicine
        {
            get { return inventoryMedicine; }
            set
            {
                inventoryMedicine = value;
                NotifyPropertyChanged("InventoryMedicine");
            }
        }

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

        private ObservableCollection<ProductUnit> productUnitCollection;
        public ObservableCollection<ProductUnit> ProductUnitCollection
        {
            get { return productUnitCollection; }
            set
            {
                productUnitCollection = value;
                NotifyPropertyChanged("ProductUnitCollection");
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

            InventoryMedicine = ((ICloneable)ProductDetail.NewProduct).Clone() as InventoryMedicine;
            inventoryMedicineBackup = (InventoryMedicine)ProductDetail.NewProduct;
            ProductDetail.NewProduct = null;
            InitMedicineDatas();
        }

        #region ----- Init Data -----
        private void InitMedicineDatas()
        {
            ProductGroupCollection = ProductDb.GetProductGroup(InventoryMedicine.Id, InventoryMedicine.WareHouseId);
            MedicineDetails = ProductDb.GetInventoryMedicineDetail(InventoryMedicine.Id);
            InventoryDetailOverviews = ProductDb.GetInventoryDetailOverviews(InventoryMedicine.Id);
            ProductUnitCollection = ProductDb.GetProductUnitById(InventoryMedicine.Id);

            CalculateStock();

            SideEffectBox.AppendText(InventoryMedicine.SideEffect);
            IndicationBox.AppendText(InventoryMedicine.Indication);
            NoteBox.AppendText(InventoryMedicine.Note);

            if (InventoryDetailOverviews.Count > 0)
            {
                InventoryDetailOverviewDataGrid.SelectedItem = InventoryDetailOverviews.Last();
                InventoryDetailOverviewDataGrid.ScrollIntoView(InventoryDetailOverviews.Last());
            }

            if (!InventoryMedicine.Control.Equals("0"))
            {
                ControlStack.Visibility = Visibility.Visible;

            }
        }

        private void CalculateStock()
        {
            if(InventoryDetailOverviews.Count == 0) return;

            InventoryDetailOverviews[InventoryDetailOverviews.Count - 1].Stock = InventoryMedicine.Stock.Inventory;

            for (int x = InventoryDetailOverviews.Count - 2; x >= 0; x--)
            {
                InventoryDetailOverviews[x].Stock =
                    InventoryDetailOverviews[x + 1].Stock - InventoryDetailOverviews[x + 1].Amount;
            }
        }
        #endregion

        #region ----- Filter -----
        private bool InventoryDetailOverviewFilter(object obj)
        {
            InventoryDetailOverview inventoryDetailOverview = obj as InventoryDetailOverview;

            switch (inventoryDetailOverview.Type)
            {
                case InventoryDetailOverviewType.PurchaseReturn:
                    return (bool)PurchaseCheckBox.IsChecked;
                case InventoryDetailOverviewType.StockTaking:
                    return (bool)StockTakingCheckBox.IsChecked;
                case InventoryDetailOverviewType.Treatment:
                    return (bool)TreatmentCheckBox.IsChecked;
            }

            return false;
        }
        #endregion

        #region ----- Data Changed -----
        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            SideEffectBox.Document.Blocks.Clear();
            IndicationBox.Document.Blocks.Clear();
            NoteBox.Document.Blocks.Clear();

            InventoryMedicine = ((ICloneable)inventoryMedicineBackup).Clone() as InventoryMedicine;
            ProductUnitCollection = ProductDb.GetProductUnitById(InventoryMedicine.Id);

            SideEffectBox.AppendText(InventoryMedicine.SideEffect);
            IndicationBox.AppendText(InventoryMedicine.Indication);
            NoteBox.AppendText(InventoryMedicine.Note);

            InitMedicineDataChanged();
        }

        private void MedicineData_Changed(object sender, EventArgs e)
        {
            MedicineDataChanged();
        }

        private void MedicineDataChanged()
        {
            CancelBtn.IsEnabled = true;
            ConfirmBtn.IsEnabled = true;

            ChangedLabel.Foreground = Brushes.Red;
            ChangedLabel.Content = "已修改";
        }

        private void InitMedicineDataChanged()
        {
            CancelBtn.IsEnabled = false;
            ConfirmBtn.IsEnabled = false;

            ChangedLabel.Foreground = Brushes.DimGray;
            ChangedLabel.Content = "未修改";
        }
        #endregion
        
        private void InventoryFilter_OnClick(object sender, RoutedEventArgs e)
        {
            InventoryDetailOverviewDataGrid.Items.Filter = InventoryDetailOverviewFilter;
        }

        private void CommonMed_OnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox is null) return;

            MedicineDataChanged();

            if ((bool)checkBox.IsChecked)
            {
                SafeAmountStack.IsEnabled = false;
                BasicAmountStack.IsEnabled = false;
            }
            else
            {
                SafeAmountStack.IsEnabled = true;
                BasicAmountStack.IsEnabled = true;
            }
        }

        private void ButtonStockCheck_Click(object sender, RoutedEventArgs e) {
            StockTakingOrderDb.StockCheckById(InventoryMedicine.Id, TextBoxTakingValue.Text);
            MessageWindow messageWindow = new MessageWindow("單品盤點成功!",MessageType.SUCCESS);
            messageWindow.ShowDialog();
            InitMedicineDatas();
        }
    }
}
