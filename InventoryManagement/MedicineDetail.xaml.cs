using His_Pos.Class.Product;
using LiveCharts;
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

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// MedicineDetail.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineDetail : Window
    {
        private Medicine medicine;
        public SeriesCollection SalesCollection { get; set; }
        public string[] Months { get; set; }

        public ObservableCollection<CusOrderOverview> CusOrderOverviewCollection;
        public ObservableCollection<OTCStoreOrderOverview> StoreOrderOverviewCollection;
        public ObservableCollection<OTCStockOverview> OTCStockOverviewCollection;
        public ObservableCollection<ProductUnit> MedUnitCollection;
        
        private bool IsChanged = false;
        private string textBox_oldValue = "NotInit";

        public MedicineDetail(Medicine med)
        {
            InitializeComponent();

            medicine = med;

            UpdateUi();

        }

        private void UpdateUi()
        {
            MedName.Content = medicine.Name;
            MedId.Content = medicine.Id;
            MedSaveAmount.Text = medicine.SafeAmount;
            MedManufactory.Text = medicine.ManufactoryName;

            IsChangedLabel.Content = "未修改";

            MedUnitCollection = ProductDb.GetProductUnitById(medicine.Id);
            MedUnit.ItemsSource = MedUnitCollection;
        }

        private void ChangedCancelButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is CusOrderOverview)
                OtcCusOrder.SelectedItem = selectedItem;
            else if (selectedItem is OTCStoreOrderOverview)
                OtcStoOrder.SelectedItem = selectedItem;
            else if (selectedItem is OTCStockOverview)
                OtcStock.SelectedItem = selectedItem;
            else if (selectedItem is ProductUnit)
                MedUnit.SelectedItem = selectedItem;
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
            else if (leaveItem is ProductUnit)
                MedUnit.SelectedItem = null;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox_oldValue == "NotInit") return;

            TextBox textBox = sender as TextBox;

            if (ChangedFlagNotChanged() && textBox.Text != textBox_oldValue)
                setChangedFlag();
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

            if (textBox.Text != string.Empty && MedUnitCollection.Count == MedUnit.SelectedIndex)
            {
                AddNewMedUnit(textBox.Tag, textBox.Text);
                textBox.Text = "";
            }
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
            List<TextBox> textBoxs = new List<TextBox>();
            FindChildGroup(MedUnit, "AmountCell", ref textBoxs);

            if (textBoxs.Count == 0) return;

            textBoxs[0].IsReadOnly = true;
            textBoxs[0].BorderBrush = Brushes.Transparent;
        }
    }
}
