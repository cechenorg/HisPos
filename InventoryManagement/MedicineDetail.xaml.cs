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
using System.ComponentModel;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// MedicineDetail.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineDetail : UserControl
    {
        public InventoryMedicine InventoryMedicine;
        public SeriesCollection SalesCollection { get; set; }
        public string[] Months { get; set; }

        public ObservableCollection<CusOrderOverview> CusOrderOverviewCollection;
        public ObservableCollection<OTCStoreOrderOverview> StoreOrderOverviewCollection;
        public ObservableCollection<OTCStockOverview> MEDStockOverviewCollection;
        public ObservableCollection<string> MEDUnitChangdedCollection = new ObservableCollection<string>();
        public ObservableCollection<string> MedIngredientCollection = new ObservableCollection<string>();
       
        public ObservableCollection<ProductUnit> MedUnitCollection;
        public ObservableCollection<ProductDetailManufactory> MEDManufactoryCollection;

        private bool IsChanged = false;
        private bool IsFirst = true;
        public MedicineDetail()
        {
            InitializeComponent();

            InventoryMedicine = (InventoryMedicine)ProductDetail.NewProduct;
            ProductDetail.NewProduct = null;

            UpdateUi();
            
            IsFirst = false;
            DataContext = this;
        }
   
        private void UpdateUi()
        {
            if (InventoryMedicine is null) return;

            MedId.Content = InventoryMedicine.Id;
            MedChiName.Text = InventoryMedicine.ChiName.Trim();
            MedEngName.Text = InventoryMedicine.EngName.Trim();
            IsControl.IsChecked = InventoryMedicine.Control;
            IsFrozen.IsChecked = InventoryMedicine.Frozen;
            MedSafeAmount.Text = InventoryMedicine.Stock.SafeAmount;
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

            MedIngredientCollection.Clear();
            string [] split = InventoryMedicine.Ingredient.Split('+');
            foreach (string row in split) {
                MedIngredientCollection.Add(row.Trim());
            }
            MedIngredient.ItemsSource = MedIngredientCollection;

            MEDManufactoryCollection = ManufactoryDb.GetManufactoryCollection(InventoryMedicine.Id);
            MedManufactory.ItemsSource = MEDManufactoryCollection;
      
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
            else if (selectedItem is string) 
                MedIngredient.SelectedItem = selectedItem; 
            else if (selectedItem is ProductDetailManufactory)
                MedManufactory.SelectedItem = selectedItem;


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
            else if (leaveItem is string)
                MedIngredient.SelectedItem = null;
            else if (leaveItem is ProductDetailManufactory)
                MedManufactory.SelectedItem = null;
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
            if (sender is CheckBox) {
                CheckBox txt = sender as CheckBox;
                SetMedTextBoxChangedCollection(txt.Name);
            }
        }
      
        private void MedNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsChangedLabel is null || IsFirst)
                return;
            if (ChangedFlagNotChanged())
                setChangedFlag();
        }
        private bool CheckValue()
        {
            List<string> _errorList = new List<string>();
            bool check = true;

            if (Convert.ToInt32(MedBasicAmount.Text) < Convert.ToInt32(MedSafeAmount.Text))
            {
                _errorList.Add("安全量不可高於基準量");
                check = false;
            }
            if (!check)
            {
                var errors = _errorList.Aggregate(string.Empty, (current, error) => current + (error + "\n"));
                MessageWindow messageWindow = new MessageWindow(errors, MessageType.ERROR);
                messageWindow.ShowDialog();
            }
            return check;
        }
        private void ButtonUpdateSubmmit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ChangedFlagNotChanged()) return;
            if (!CheckValue()) return;

            InventoryMedicine.Control = (bool)IsControl.IsChecked;
            InventoryMedicine.Frozen = (bool)IsFrozen.IsChecked;
            InventoryMedicine.ChiName = MedChiName.Text;
            InventoryMedicine.EngName = MedEngName.Text;
            InventoryMedicine.Name = InventoryMedicine.EngName.Contains(" ") ? InventoryMedicine.EngName.Split(' ')[0] + " " + InventoryMedicine.EngName.Split(' ')[1] + "... " + InventoryMedicine.ChiName : InventoryMedicine.ChiName;
            InventoryMedicine.Location = MedLocation.Text;
            InventoryMedicine.Stock.BasicAmount = MedBasicAmount.Text;
            InventoryMedicine.Stock.SafeAmount = MedSafeAmount.Text;
            InventoryMedicine.Note = new TextRange(MedNotes.Document.ContentStart, MedNotes.Document.ContentEnd).Text;
            InventoryMedicine.Status = MedStatus.Text == "啟用" ? true : false;
            ProductDb.UpdateOtcDataDetail(InventoryMedicine, "InventoryMedicine");
            
            foreach (string index in MEDUnitChangdedCollection)
            {
                ProductUnit prounit = new ProductUnit(Convert.ToInt32(index), ((TextBox)DockUnit.FindName("MedUnitName" + index)).Text,
                                         ((TextBox)DockUnit.FindName("MedUnitAmount" + index)).Text, ((TextBox)DockUnit.FindName("MedUnitPrice" + index)).Text,
                                          ((TextBox)DockUnit.FindName("MedUnitVipPrice" + index)).Text, ((TextBox)DockUnit.FindName("MedUnitEmpPrice" + index)).Text);
                ProductDb.UpdateOtcUnit(prounit, InventoryMedicine.Id);
            }
            MessageWindow messageWindow = new MessageWindow("商品修改成功!", MessageType.SUCCESS);
            messageWindow.Show();
            InitVariables();
        }
     
        private void DataGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectitem = MedStoOrder.SelectedItem;
            ProductPurchaseRecordView.Proid = ((OTCStoreOrderOverview)selectitem).StoreOrderId;
            MainWindow.Instance.AddNewTab("處理單紀錄");
        }

       
    }
}
