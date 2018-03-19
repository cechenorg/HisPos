using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<OTCUnit> OTCUnitCollection;

        private Otc otc;
        private bool IsChanged = false;
        private string textBox_oldValue = "NotInit";

        public OtcDetail(Otc o)
        {
            InitializeComponent();

            otc = o;
            
            UpdateUi();
            CheckAuth();

            DataContext = this;
        }

        private void ChangedCancelButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeComponent();

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
            
            OtcSaveAmount.Text = otc.SafeAmount;
            OtcManufactory.Text = otc.ManufactoryName;
            
            CusOrderOverviewCollection = OTCDb.GetOtcCusOrderOverviewByID(otc.Id);
            OtcCusOrder.ItemsSource = CusOrderOverviewCollection;

            StoreOrderOverviewCollection = OTCDb.GetOtcStoOrderByID(otc.Id);
            OtcStoOrder.ItemsSource = StoreOrderOverviewCollection;

            OTCStockOverviewCollection = OTCDb.GetOtcStockOverviewById(otc.Id);
            OtcStock.ItemsSource = OTCStockOverviewCollection;
            UpdateStockOverviewInfo();

            OTCUnitCollection = OTCDb.GetOtcUnitById(otc.Id);
            OtcUnit.ItemsSource = OTCUnitCollection;

            UpdateChart();

            IsChangedLabel.Content = "未修改";
            IsChangedLabel.Foreground = (Brush)FindResource("ForeGround");

            textBox_oldValue = "NotInit";
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
            else if (selectedItem is OTCUnit)
                OtcUnit.SelectedItem = selectedItem;
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
            else if (leaveItem is OTCUnit)
                OtcUnit.SelectedItem = null;
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ( textBox_oldValue == "NotInit") return;

            TextBox textBox = sender as TextBox;

            if (ChangedFlagNotChanged() && textBox.Text != textBox_oldValue)
                setChangedFlag();
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

            if (textBox.Text != string.Empty && OTCUnitCollection.Count == OtcUnit.SelectedIndex)
            {
                AddNewOTCUnit(textBox.Tag, textBox.Text);
                textBox.Text = "";
            }
        }

        private void AddNewOTCUnit(object tag, string text)
        {
            OTCUnit otcUnit = new OTCUnit();

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

            OTCUnitCollection.Add(otcUnit);
        }

        private void OTCUnit_KeyDown(object sender, KeyEventArgs e)
        {
            setChangedFlag();
        }

        private void OtcUnitGotFocus(object sender, RoutedEventArgs e)
        {
            textBox_oldValue = (sender as TextBox).Text;
        }
    }
}
