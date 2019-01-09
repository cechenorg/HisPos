using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.Class.Product;
using His_Pos.Class.StockTakingOrder;
using His_Pos.FunctionWindow;
using His_Pos.Interface;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.InventoryManagement.MedControl
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
                SingdeMinOrderAmount = dataRow["PROSIN_MINORDER"].ToString();
                PackageAmount = dataRow["PROSIN_PACKAGEQTY"].ToString();
                SingdePackagePrice = dataRow["PROSIN_PACKAGEPRICE"].ToString();
                SindePrice = dataRow["PROSIN_PRICE"].ToString();
                Form = dataRow["HISMED_FORM"].ToString();
                ATC = dataRow["HISMED_ATC"].ToString();
                Manufactory = dataRow["HISMED_MANUFACTORY"].ToString();
                SC = dataRow["HISMED_SC"].ToString();
                Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
            }

            public string NHIPrice { get; }
            public string PackageAmount { get; }
            public string SingdePackagePrice { get; }
            public string SingdeMinOrderAmount { get; }
            public string SindePrice { get; }
            public string Form { get; }
            public string ATC { get; }
            public string Manufactory { get; }
            public string SC { get; }
            public string Ingredient { get; }
        }

        public struct SyncFlag
        {

            public SyncFlag(DataRow dataRow)
            {
                ChiNameFlag = Boolean.Parse(dataRow["HISMEDSYNC_NAME"].ToString());
                EngNameFlag = Boolean.Parse(dataRow["HISMEDSYNC_ENGNAME"].ToString());
                NoteFlag = Boolean.Parse(dataRow["HISMEDSYNC_NOTE"].ToString());
                SideEffectFlag = Boolean.Parse(dataRow["HISMEDSYNC_SIDEFFECT"].ToString());
                IndicationFlag = Boolean.Parse(dataRow["HISMEDSYNC_INDICATION"].ToString());
            }

            public SyncFlag(bool defaultValue)
            {
                ChiNameFlag = defaultValue;
                EngNameFlag = defaultValue;
                NoteFlag = defaultValue;
                SideEffectFlag = defaultValue;
                IndicationFlag = defaultValue;
            }

            public bool ChiNameFlag { get; set; }
            public bool EngNameFlag { get; set; }
            public bool NoteFlag { get; set; }
            public bool SideEffectFlag { get; set; }
            public bool IndicationFlag { get; set; }

            public bool IsAllDataSync
            {
                get { return ChiNameFlag && EngNameFlag && NoteFlag && SideEffectFlag && IndicationFlag; }
            }
        }
        #endregion

        #region ----- Define Variables -----

        private bool IsFirst { get; set; } = true;

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

        public SyncFlag flags;
        public SyncFlag Flags
        {
            get { return flags; }
            set
            {
                flags = value;
                NotifyPropertyChanged("Flags");
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
            Flags = MedicineDb.GetMedicineSyncFlag(InventoryMedicine.Id);

            CalculateStock();

            SideEffectBox.AppendText(InventoryMedicine.SideEffect);
            IndicationBox.AppendText(InventoryMedicine.Indication);
            WarningBox.AppendText(InventoryMedicine.Warnings);
            NoteBox.AppendText(InventoryMedicine.Note);

            if (InventoryDetailOverviews.Count > 0)
            {
                InventoryDetailOverviewDataGrid.SelectedItem = InventoryDetailOverviews.Last();
                InventoryDetailOverviewDataGrid.ScrollIntoView(InventoryDetailOverviews.Last());
            }

            if (!(InventoryMedicine.Control.Equals("0") || InventoryMedicine.Control.Equals("")))
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
        private void MedicineData_Changed(object sender, EventArgs e)
        {
            MedicineDataChanged();
        }

        private void MedicineDataChanged()
        {
            if(IsFirst) return;

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

        #region ----- Unit DataGrid -----
        private void Unit_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is null) return;

            if (e.Key == Key.Enter)
            {
                MoveFocusNext(sender);

                TextBox textBox = sender as TextBox;

                if (textBox.Name.Equals("Unit") && !textBox.Text.Equals(""))
                {
                    if (!(UnitDataGrid.SelectedItem is ProductUnit))
                    {
                        ProductUnit productUnit = new ProductUnit(textBox.Text);
                        productUnitCollection.Add(productUnit);
                        textBox.Text = "";

                        UnitDataGrid.CurrentCell = new DataGridCellInfo(UnitDataGrid.Items[productUnitCollection.Count - 1], Amount);

                        var focusedCell = UnitDataGrid.CurrentCell.Column.GetCellContent(UnitDataGrid.CurrentCell.Item);
                        var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell ?? throw new InvalidOperationException(), 0);
                        while (firstChild is ContentPresenter)
                        {
                            firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                        }
                        firstChild.Focus();
                    }
                }

                e.Handled = true;
            }
        }

        private void MoveFocusNext(object sender)
        {
            (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (UnitDataGrid.CurrentCell.Column is null) return;

            var focusedCell = UnitDataGrid.CurrentCell.Column.GetCellContent(UnitDataGrid.CurrentCell.Item);

            if (focusedCell is null) return;

            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                    if (!(child is Image))
                        break;
                }

                focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                focusedCell = UnitDataGrid.CurrentCell.Column.GetCellContent(UnitDataGrid.CurrentCell.Item);
            }

            UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

            if (firstChild is TextBox)
                firstChild.Focus();
        }

        private void Unit_OnFocus(object sender, RoutedEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is IDeletable)
            {
                if ((selectedItem as ProductUnit).BaseType) return;

                (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";
            }
        }

        private void Unit_LostFocus(object sender, RoutedEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is IDeletable)
                (selectedItem as IDeletable).Source = "";
        }

        private void UnitDeleteDot_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (UnitDataGrid.SelectedItem is null) return;

            ProductUnitCollection.Remove(UnitDataGrid.SelectedItem as ProductUnit);

            MedicineDataChanged();
        }
        #endregion

        #region ----- StockTaking -----
        private void ButtonStockCheck_Click(object sender, RoutedEventArgs e)
        {
            if (!IsStockTakingValid()) return;

            if (!ConfirmStockTaking()) return;

            string stockValue = StockTakingOrderDb.StockCheckById(InventoryMedicine.Id, TextBoxTakingValue.Text);
            MessageWindow messageWindow = new MessageWindow("單品盤點成功!", MessageType.SUCCESS);
            messageWindow.ShowDialog();
            
            InventoryMedicine.Stock.Inventory = Double.Parse(TextBoxTakingValue.Text);
            inventoryMedicineBackup.Stock.Inventory = Double.Parse(TextBoxTakingValue.Text);

            InventoryMedicine.StockValue = stockValue;
            inventoryMedicineBackup.StockValue = stockValue;

            InventoryDetailOverviews = ProductDb.GetInventoryDetailOverviews(InventoryMedicine.Id);
            CalculateStock();
            TextBoxTakingValue.Text = string.Empty;
        }

        private bool ConfirmStockTaking()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否確認盤點?", MessageType.ONLYMESSAGE);
            confirmWindow.ShowDialog();

            return confirmWindow.Confirm;
        }

        private bool IsStockTakingValid()
        {
            if (TextBoxTakingValue.Text.Equals(""))
            {
                MessageWindow messageWindow = new MessageWindow("請輸入數量!", MessageType.WARNING, true);
                messageWindow.ShowDialog();

                return false;
            }

            return true;
        }
        #endregion

        #region ----- Confirm Change -----
        private void ConfirmBtn_OnClick(object sender, RoutedEventArgs e)
        {
            InventoryMedicine.SideEffect = new TextRange(SideEffectBox.Document.ContentStart, SideEffectBox.Document.ContentEnd).Text.Replace("\r\n","");
            InventoryMedicine.Indication = new TextRange(IndicationBox.Document.ContentStart, IndicationBox.Document.ContentEnd).Text.Replace("\r\n", "");
            InventoryMedicine.Warnings = new TextRange(WarningBox.Document.ContentStart, WarningBox.Document.ContentEnd).Text.Replace("\r\n", "");
            InventoryMedicine.Note = new TextRange(NoteBox.Document.ContentStart, NoteBox.Document.ContentEnd).Text.Replace("\r\n", "");

            MedicineDb.UpdateInventoryMedicineData(InventoryMedicine);
            ProductDb.UpdateInventoryProductUnit(InventoryMedicine.Id, ProductUnitCollection);

            UpdateNewDataToCurrentMed();

            InitMedicineDataChanged();

            MessageWindow messageWindow = new MessageWindow("更新成功!", MessageType.SUCCESS);
            messageWindow.ShowDialog();
        }

        private void UpdateNewDataToCurrentMed()
        {
            inventoryMedicineBackup.EngName = InventoryMedicine.EngName;
            inventoryMedicineBackup.ChiName = InventoryMedicine.ChiName;
            inventoryMedicineBackup.Status = InventoryMedicine.Status;
            inventoryMedicineBackup.Location = InventoryMedicine.Location;
            inventoryMedicineBackup.BarCode = InventoryMedicine.BarCode;
            inventoryMedicineBackup.Common = InventoryMedicine.Common;
            inventoryMedicineBackup.Stock.SafeAmount = InventoryMedicine.Stock.SafeAmount;
            inventoryMedicineBackup.Stock.BasicAmount = InventoryMedicine.Stock.BasicAmount;
            inventoryMedicineBackup.SideEffect = InventoryMedicine.SideEffect;
            inventoryMedicineBackup.Indication = InventoryMedicine.Indication;
            inventoryMedicineBackup.Warnings = InventoryMedicine.Warnings;
            inventoryMedicineBackup.Note = InventoryMedicine.Note;
        }
        #endregion

        #region ----- Cancel Change -----
        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            IsFirst = true;

            SideEffectBox.Document.Blocks.Clear();
            IndicationBox.Document.Blocks.Clear();
            WarningBox.Document.Blocks.Clear();
            NoteBox.Document.Blocks.Clear();

            InventoryMedicine = ((ICloneable)inventoryMedicineBackup).Clone() as InventoryMedicine;
            ProductUnitCollection = ProductDb.GetProductUnitById(InventoryMedicine.Id);

            SideEffectBox.AppendText(InventoryMedicine.SideEffect);
            IndicationBox.AppendText(InventoryMedicine.Indication);
            WarningBox.AppendText(InventoryMedicine.Warnings);
            NoteBox.AppendText(InventoryMedicine.Note);

            InitMedicineDataChanged();
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
                SafeAmountStack.IsEnabled = true;
                BasicAmountStack.IsEnabled = true;
                MinOrderStack.IsEnabled = true;
            }
            else
            {
                SafeAmountStack.IsEnabled = false;
                BasicAmountStack.IsEnabled = false;
                MinOrderStack.IsEnabled = false;
            }
        }

        private void MedicineDetail_OnGotFocus(object sender, RoutedEventArgs e)
        {
            IsFirst = false;
        }

        private void OpenMedHisPrice_OnClick(object sender, RoutedEventArgs e)
        {
            MedicineHistoryPriceWindow medicineHistoryPriceWindow = new MedicineHistoryPriceWindow(InventoryMedicine.Id);
            medicineHistoryPriceWindow.Show();
        }

        private void DataSync_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;
            
            InventoryMedicine = MedicineDb.ResetMedicineSyncFlag(InventoryMedicine.Id);
            UpdateNewDataToCurrentMed();

            SideEffectBox.Document.Blocks.Clear();
            IndicationBox.Document.Blocks.Clear();
            WarningBox.Document.Blocks.Clear();
            NoteBox.Document.Blocks.Clear();

            SideEffectBox.AppendText(InventoryMedicine.SideEffect);
            IndicationBox.AppendText(InventoryMedicine.Indication);
            WarningBox.AppendText(InventoryMedicine.Warnings);
            NoteBox.AppendText(InventoryMedicine.Note);

            InitMedicineDataChanged();

            Flags = new SyncFlag(true);

            MessageWindow messageWindow = new MessageWindow("資料已同步!", MessageType.SUCCESS);
            messageWindow.ShowDialog();
        }
    }
}
