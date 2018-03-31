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

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// MedicineDetail.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineDetail : Window
    {
        public Medicine medicine;
        public SeriesCollection SalesCollection { get; set; }
        public string[] Months { get; set; }

        public ObservableCollection<CusOrderOverview> CusOrderOverviewCollection;
        public ObservableCollection<OTCStoreOrderOverview> StoreOrderOverviewCollection;
        public ObservableCollection<OTCStockOverview> MEDStockOverviewCollection;
        public ObservableCollection<string> MEDUnitChangdedCollection = new ObservableCollection<string>();
        public ObservableCollection<ProductUnit> MedUnitCollection;
        public ObservableCollection<Manufactory> MEDManufactoryCollection;
        public ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection = new ObservableCollection<Manufactory>();

        public event MouseButtonEventHandler mouseButtonEventHandler;

        private bool IsChanged = false;
        private bool IsFirst = true;
        private string textBox_oldValue = "NotInit";

        public MedicineDetail(string proId)
        {
            InitializeComponent();

            medicine = MedicineDb.GetMedDetail(proId);

            UpdateUi();
            IsFirst = false;
        }

        private void UpdateUi()
        {
                if (medicine is null) return;
            MedName.Content = medicine.Name;
            MedId.Content = medicine.Id;
            MedSaveAmount.Text = medicine.SafeAmount;
            MedLocation.Text = medicine.Location;
            MedBasicAmount.Text = medicine.BasicAmount;
            MedNotes.Document.Blocks.Clear();
            MedNotes.AppendText(medicine.Note);

            IsChangedLabel.Content = "未修改";
            
             CusOrderOverviewCollection = OTCDb.GetOtcCusOrderOverviewByID(medicine.Id);
             MedCusOrder.ItemsSource = CusOrderOverviewCollection;

                StoreOrderOverviewCollection = OTCDb.GetOtcStoOrderByID(medicine.Id);
                MedStoOrder.ItemsSource = StoreOrderOverviewCollection;

            MEDStockOverviewCollection = ProductDb.GetProductStockOverviewById(medicine.Id);
                MedStock.ItemsSource = MEDStockOverviewCollection;
                UpdateStockOverviewInfo();
            MedUnitCollection = ProductDb.GetProductUnitById(medicine.Id);
            MEDManufactoryCollection = GetManufactoryCollection();
            MedManufactory.ItemsSource = MEDManufactoryCollection;
            foreach (DataRow row in MainWindow.ManufactoryTable.Rows)
            {
                ManufactoryAutoCompleteCollection.Add(new Manufactory(row, DataSource.MANUFACTORY));
            }

            UpdateChart();
                InitVariables();
                SetUnitValue();
            
        }
        private ObservableCollection<Manufactory> GetManufactoryCollection()
        {
            ObservableCollection<Manufactory> manufactories = new ObservableCollection<Manufactory>();

            var man = MainWindow.ProManTable.Select("PRO_ID = '" + medicine.Id + "'");

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
            ChartValues<double> chartValues = OTCDb.GetOtcSalesByID(medicine.Id);

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

            TotalStock.Content = totalStock.ToString();
            StockTotalPrice.Content = "$" + totalPrice.ToString("0.00");
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
                MedCusOrder.SelectedItem = selectedItem;
            else if (selectedItem is OTCStockOverview)
                MedStock.SelectedItem = selectedItem;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;

            if (leaveItem is CusOrderOverview)
                MedCusOrder.SelectedItem = null;
            else if (leaveItem is OTCStoreOrderOverview)
                MedCusOrder.SelectedItem = null;
            else if (leaveItem is OTCStockOverview)
                MedStock.SelectedItem = null;
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
        private void MedData_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsChangedLabel is null || IsFirst)
                return;
            TextBox txt = sender as TextBox;
            SetMedTextBoxChangedCollection(txt.Name);
        }
        private void ButtonUpdateSubmmit_Click(object sender, RoutedEventArgs e)
        {
            //OTCDb.UpdateOtcDataDetail(medicine.Id, MedSaveAmount.Text,MedBasicAmount.Text, MedLocation.Text, new TextRange(MedNotes.Document.ContentStart, MedNotes.Document.ContentEnd).Text);
            //foreach (string index in MEDUnitChangdedCollection){
            //    ProductUnit prounit = new ProductUnit(Convert.ToInt32(index), ((TextBox)DockUnit.FindName("MedUnitName" + index)).Text,
            //                             ((TextBox)DockUnit.FindName("MedUnitAmount" + index)).Text, ((TextBox)DockUnit.FindName("MedUnitPrice" + index)).Text,
            //                              ((TextBox)DockUnit.FindName("MedUnitVipPrice" + index)).Text, ((TextBox)DockUnit.FindName("MedUnitEmpPrice" + index)).Text);
            //    OTCDb.UpdateOtcUnit(prounit,medicine.Id);
            //}
            //InitVariables();
        }

        private void MedManufactoryAuto_OnDropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            var ManufactoryAuto = sender as AutoCompleteBox;

            if (ManufactoryAuto is null) return;
            if (ManufactoryAuto.SelectedItem is null) return;

            if (MEDManufactoryCollection.Count <= MedManufactory.SelectedIndex)
            {
                MEDManufactoryCollection[MedManufactory.SelectedIndex] = (Manufactory)ManufactoryAuto.SelectedItem;
                MEDManufactoryCollection.Add(new Manufactory());
            }
            else
            {
                MEDManufactoryCollection[MedManufactory.SelectedIndex] = (Manufactory)ManufactoryAuto.SelectedItem;
                return;
            }
            ManufactoryAuto.Text = "";
        }

        private void MedNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsChangedLabel is null || IsFirst)
                return;
            if (ChangedFlagNotChanged())
                setChangedFlag();
        }
    }
}
