using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Collections.Specialized;
using His_Pos.Interface;
using His_Pos.ProductPurchaseRecord;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// MedicineDetail.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineDetail : Window
    {
        public InventoryMedicine InventoryMedicine;
        public SeriesCollection SalesCollection { get; set; }
        public string[] Months { get; set; }

        public ObservableCollection<CusOrderOverview> CusOrderOverviewCollection;
        public ObservableCollection<OTCStoreOrderOverview> StoreOrderOverviewCollection;
        public ObservableCollection<OTCStockOverview> MEDStockOverviewCollection;
        public ObservableCollection<string> MEDUnitChangdedCollection = new ObservableCollection<string>();
        public ObservableCollection<ProductUnit> MedUnitCollection;
        public ObservableCollection<ProductDetailManufactory> MEDManufactoryCollection;
        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection = new ObservableCollection<Manufactory>();
        public HashSet<ManufactoryChanged> MEDManufactoryChangedCollection = new HashSet<ManufactoryChanged>();
        public event MouseButtonEventHandler mouseButtonEventHandler;

        private bool IsChanged = false;
        private bool IsFirst = true;
        private string textBox_oldValue = "NotInit";
        private int LastSelectedIndex = -1;
        public MedicineDetail(InventoryMedicine inventoryMedicine)
        {
            InitializeComponent();
            InventoryMedicine = inventoryMedicine;

            UpdateUi();
           
            IsFirst = false;
            DataContext = this;
        }
        private void UpdateMed() {
            InventoryMedicine.Name = MedName.Content.ToString();
            InventoryMedicine.Id = MedId.Content.ToString();
            InventoryMedicine.Stock.SafeAmount = MedSaveAmount.Text;
            InventoryMedicine.Location = MedLocation.Text;
            InventoryMedicine.Stock.BasicAmount = MedBasicAmount.Text;
            TextRange textRange = new TextRange(MedNotes.Document.ContentStart, MedNotes.Document.ContentEnd);
            InventoryMedicine.Note = textRange.Text;
        }
        private void UpdateUi()
        {
                if (InventoryMedicine is null) return;
            MedName.Content = InventoryMedicine.Name;
            MedId.Content = InventoryMedicine.Id;
            MedSaveAmount.Text = InventoryMedicine.Stock.SafeAmount;
            MedLocation.Text = InventoryMedicine.Location;
            MedBasicAmount.Text = InventoryMedicine.Stock.BasicAmount;
            MedNotes.Document.Blocks.Clear();
            MedNotes.AppendText(InventoryMedicine.Note);
            MedStatus.Text = InventoryMedicine.Status == true ? "啟用" : "已停用";
            IsChangedLabel.Content = "未修改";
            
             CusOrderOverviewCollection = OTCDb.GetOtcCusOrderOverviewByID(InventoryMedicine.Id);
             MedCusOrder.ItemsSource = CusOrderOverviewCollection;

                StoreOrderOverviewCollection = OTCDb.GetOtcStoOrderByID(InventoryMedicine.Id);
                MedStoOrder.ItemsSource = StoreOrderOverviewCollection;

            MEDStockOverviewCollection = ProductDb.GetProductStockOverviewById(InventoryMedicine.Id);
                MedStock.ItemsSource = MEDStockOverviewCollection;
                UpdateStockOverviewInfo();
            MedUnitCollection = ProductDb.GetProductUnitById(InventoryMedicine.Id);
           
            MEDManufactoryCollection = ManufactoryDb.GetManufactoryCollection(InventoryMedicine.Id);
            MEDManufactoryCollection.Add(new ProductDetailManufactory());
            MedManufactory.ItemsSource = MEDManufactoryCollection;
            MEDManufactoryCollection.CollectionChanged += MedManufactoryCollectionOnCollectionChanged;

            foreach (DataRow row in MainWindow.ManufactoryTable.Rows)
            {
                bool keep = true;

                foreach (var detailManufactory in MEDManufactoryCollection)
                {
                    if (row["MAN_ID"].ToString() == detailManufactory.Id)
                        keep = false;
                }

                if (keep)
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
        private void SetUnitValue()
        {
            int count = 0;
            string index = "";
            foreach (var row in MedUnitCollection)
            {
                index = count.ToString();
                ((TextBox)DockUnit.FindName("MedUnitName" + index)).Text = row.Unit;
                ((TextBox)DockUnit.FindName("MedUnitAmount" + index)).Text = row.Amount;
                ((TextBox)DockUnit.FindName("MedUnitPrice" + index)).Text = row.Price;
                ((TextBox)DockUnit.FindName("MedUnitVipPrice" + index)).Text = row.VIPPrice;
                ((TextBox)DockUnit.FindName("MedUnitEmpPrice" + index)).Text = row.EmpPrice;
                count++;
            }
        }
        private void UpdateChart()
        {
            SalesCollection = new SeriesCollection();
            SalesCollection.Add(GetSalesLineSeries());
            AddMonths();
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
        private LineSeries GetSalesLineSeries()
        {
            ChartValues<double> chartValues = OTCDb.GetOtcSalesByID(InventoryMedicine.Id);

            return new LineSeries
            {
                Title = "銷售量",
                Values = chartValues,
                PointGeometrySize = 10,
                LineSmoothness = 0,
                DataLabels = true
            };
        }
        private void UpdateStockOverviewInfo()
        {
            int totalStock = 0;
            double totalPrice = 0;

            foreach (var Otc in MEDStockOverviewCollection)
            {
                totalStock += Int32.Parse(Otc.Amount);
                totalPrice += Double.Parse(Otc.Price) * Int32.Parse(Otc.Amount);
            }

            TotalStock.Content = InventoryMedicine.Stock.Inventory;
            StockTotalPrice.Content = "$" + InventoryMedicine.StockValue;
        }
        private void ChangedCancelButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUi();
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is CusOrderOverview)
                MedCusOrder.SelectedItem = selectedItem;
            else if (selectedItem is OTCStoreOrderOverview)
                MedStoOrder.SelectedItem = selectedItem;
            else if (selectedItem is OTCStockOverview)
                MedStock.SelectedItem = selectedItem;
            else if (selectedItem is IDeletable)
            {
                if (selectedItem != MEDManufactoryCollection.Last())
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";
                MedManufactory.SelectedItem = selectedItem;
                LastSelectedIndex = MedManufactory.SelectedIndex;
            }
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;
            if (leaveItem is CusOrderOverview)
                MedCusOrder.SelectedItem = null;
            else if (leaveItem is OTCStoreOrderOverview)
                MedStoOrder.SelectedItem = null;
            else if (leaveItem is OTCStockOverview)
                MedStock.SelectedItem = null;
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

        public static void FindChildGroup<T>(DependencyObject parent, string childName, ref List<T> list) where T : DependencyObject
        {
            //List<TextBox> textBoxList = new List<TextBox>();
            //FindChildGroup(PrescriptionSet, "MedicineDays", ref textBoxList);

            // Checks should be made, but preferably one time before calling.
            // And here it is assumed that the programmer has taken into
            // account all of these conditions and checks are not needed.
            //if ((parent == null) || (childName == null) || (<Type T is not inheritable from FrameworkElement>))
            //{
            //    return;
            //}
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                // Get the child
                var child = VisualTreeHelper.GetChild(parent, i);

                // Compare on conformity the type

                // Not compare - go next
                if (!(child is T childTest))
                {
                    // Go the deep
                    FindChildGroup(child, childName, ref list);
                }
                else
                {
                    // If match, then check the name of the item
                    FrameworkElement childElement = childTest as FrameworkElement;

                    Debug.Assert(childElement != null, nameof(childElement) + " != null");
                    if (childElement.Name == childName)
                    {
                        // Found
                        list.Add(childTest);
                    }

                    // We are looking for further, perhaps there are
                    // children with the same name
                    FindChildGroup(child, childName, ref list);
                }
            }
        }

        private void OtcUnitOnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

           
        }

        private void AddNewMedUnit(object tag, string text)
        {
            ProductUnit otcUnit = new ProductUnit();

            switch (tag)
            {
                case "Unit":
                    otcUnit.Unit = text;
                    break;
                case "Amount":
                    otcUnit.Amount = text;
                    break;
                case "Price":
                    otcUnit.Price = text;
                    break;
                case "VIPPrice":
                    otcUnit.VIPPrice = text;
                    break;
                case "EmpPrice":
                    otcUnit.EmpPrice = text;
                    break;
                default:
                    return;
            }

            MedUnitCollection.Add(otcUnit);
        }

        private void OtcMedGotFocus(object sender, RoutedEventArgs e)
        {
            textBox_oldValue = (sender as TextBox).Text;
        }

        private void MedUnit_OnLoaded(object sender, EventArgs e)
        {
            ChangeFirstAmountToReadOnly();
        }

        private void ChangeFirstAmountToReadOnly()
        {
        }
        private void SetMedTextBoxChangedCollection(string name)
        {
            if (ChangedFlagNotChanged())
                setChangedFlag();
            if (name.Contains("MedUnit"))
            {
                string index = name.Substring(name.Length - 1, 1);
                if (!MEDUnitChangdedCollection.Contains(index))
                    MEDUnitChangdedCollection.Add(index);
            }
        }
        private void MedData_TextChanged(object sender,EventArgs e)
        {
            if (IsChangedLabel is null || IsFirst)
                return;
            if (sender is TextBox) {
                TextBox txt = sender as TextBox;
                SetMedTextBoxChangedCollection(txt.Name);
            }
            if (sender is ComboBox)
            {
                ComboBox txt = sender as ComboBox;
                SetMedTextBoxChangedCollection(txt.Name);
            }
        }
        private void MedManufactoryCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            ProductDetailManufactory OldProductDetailManufactory = (e.OldItems is null) ? null : e.OldItems[0] as ProductDetailManufactory;
            ProductDetailManufactory NewProductDetailManufactory = (e.NewItems is null) ? null : e.NewItems[0] as ProductDetailManufactory;

            if (ChangedFlagNotChanged())
                setChangedFlag();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    return;
                case NotifyCollectionChangedAction.Replace:
                    if (NewProductDetailManufactory.OrderId is null && OldProductDetailManufactory.Id is null)
                        MEDManufactoryChangedCollection.Add(new ManufactoryChanged(NewProductDetailManufactory, ProcedureProcessType.INSERT));
                    else
                    {
                        MEDManufactoryChangedCollection.Add(new ManufactoryChanged(NewProductDetailManufactory, ProcedureProcessType.UPDATE));
                        ManufactoryAutoCompleteCollection.Add(OldProductDetailManufactory);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    MEDManufactoryChangedCollection.RemoveWhere(DeleteManufactoryChanged);
                    if (OldProductDetailManufactory.OrderId != null)
                        MEDManufactoryChangedCollection.Add(new ManufactoryChanged(OldProductDetailManufactory, ProcedureProcessType.DELETE));
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
        
       

        private void MedNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsChangedLabel is null || IsFirst)
                return;
            if (ChangedFlagNotChanged())
                setChangedFlag();
        }

        private void ButtonUpdateSubmmit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ChangedFlagNotChanged()) return;
            
            InventoryMedicine.Location = MedLocation.Text;
            InventoryMedicine.Stock.BasicAmount = MedBasicAmount.Text;
            InventoryMedicine.Stock.SafeAmount = MedSaveAmount.Text;
            InventoryMedicine.Note = new TextRange(MedNotes.Document.ContentStart, MedNotes.Document.ContentEnd).Text;
            InventoryMedicine.Status = MedSaveAmount.Text == "啟用" ? true : false;
            ProductDb.UpdateOtcDataDetail(InventoryMedicine, "InventoryMedicine");

            foreach (var manufactoryChanged in MEDManufactoryChangedCollection)
            {
                ManufactoryDb.UpdateProductManufactory(InventoryMedicine.Id, manufactoryChanged);
            }
            foreach (string index in MEDUnitChangdedCollection)
            {
                ProductUnit prounit = new ProductUnit(Convert.ToInt32(index), ((TextBox)DockUnit.FindName("MedUnitName" + index)).Text,
                                         ((TextBox)DockUnit.FindName("MedUnitAmount" + index)).Text, ((TextBox)DockUnit.FindName("MedUnitPrice" + index)).Text,
                                          ((TextBox)DockUnit.FindName("MedUnitVipPrice" + index)).Text, ((TextBox)DockUnit.FindName("MedUnitEmpPrice" + index)).Text);
                ProductDb.UpdateOtcUnit(prounit, InventoryMedicine.Id);
            }
            InitVariables();
            MouseButtonEventHandler handler = mouseButtonEventHandler;

            handler(this, e);
        }
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MEDManufactoryCollection.RemoveAt(MedManufactory.SelectedIndex);
        }

        private void MedManufactoryAuto_Populating(object sender, PopulatingEventArgs e)
        {
            var ManufactoryAuto = sender as AutoCompleteBox;

            if (ManufactoryAuto is null) return;

            if (ManufactoryAuto.ItemsSource is null)
                ManufactoryAuto.ItemsSource = ManufactoryAutoCompleteCollection;

            foreach (Manufactory manufactory in ManufactoryAutoCompleteCollection)
            {
                if (manufactory.Id == MEDManufactoryCollection[MEDManufactoryCollection.Count - 2].Id)
                {
                    ManufactoryAutoCompleteCollection.Remove(manufactory);
                    break;
                }
            }

            ManufactoryAuto.PopulateComplete();
        }

        private void MedManufactoryAuto_DropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            var ManufactoryAuto = sender as AutoCompleteBox;

            if (ManufactoryAuto is null) return;
            if (ManufactoryAuto.SelectedItem is null) return;

            if (MEDManufactoryCollection.Count == MedManufactory.SelectedIndex + 1)
            {
                MEDManufactoryCollection[MedManufactory.SelectedIndex] = new ProductDetailManufactory(ManufactoryAuto.SelectedItem as Manufactory);
                MEDManufactoryCollection.Add(new ProductDetailManufactory());
            }
            else
            {
                MEDManufactoryCollection[LastSelectedIndex] = new ProductDetailManufactory(ManufactoryAuto.SelectedItem as Manufactory);
            }
        }

        private void MedManufactoryAuto_LostFocus(object sender, RoutedEventArgs e)
        {
            var ManufactoryAuto = sender as AutoCompleteBox;
            if (ManufactoryAuto is null) return;

            if ((ManufactoryAuto.Text is null || ManufactoryAuto.Text == String.Empty) && LastSelectedIndex != MEDManufactoryCollection.Count - 1)
            {
                MEDManufactoryCollection.RemoveAt(LastSelectedIndex);
            }
        }

        private void DataGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectitem = MedStoOrder.SelectedItem;
            ProductPurchaseRecordView.Proid = ((OTCStoreOrderOverview)selectitem).StoreOrderId;
            MainWindow.Instance.AddNewTab("處理單紀錄");

        }
        
    }
}
