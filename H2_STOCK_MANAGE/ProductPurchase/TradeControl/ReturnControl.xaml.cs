using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using His_Pos.Class.StoreOrder;
using His_Pos.Interface;
using His_Pos.ProductPurchase;
using His_Pos.Service;
using His_Pos.Struct.Manufactory;
using His_Pos.Struct.Product;
using His_Pos.Struct.StoreOrder;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl
{
    /// <summary>
    /// ReturnControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnControl : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Inner Class -----
        public class BatchNumOverview
        {
            public BatchNumOverview(DataRow row)
            {
                BatchNumber = row["PRO_BATCHNUM"].ToString();
                Amount = Double.Parse(row["PRO_AMOUNT"].ToString());
                SelectedAmount = 0;
            }

            public string BatchNumber { get; }
            public double Amount { get; }

            public double SelectedAmount { get; set; }
        }
        #endregion

        #region ----- Define Variables -----
        public Collection<PurchaseProduct> ProductCollection { get; set; }

        private Collection<PurchasePrincipal> principalCollection;
        public Collection<PurchasePrincipal> PrincipalCollection
        {
            get
            {
                return principalCollection;
            }
            set
            {
                principalCollection = value;
                NotifyPropertyChanged("PrincipalCollection");
            }
        }
        private StoreOrder storeOrderData;
        public StoreOrder StoreOrderData
        {
            get
            {
                return storeOrderData;
            }
            set
            {
                storeOrderData = value;
                NotifyPropertyChanged("StoreOrderData");
            }
        }

        public DataGrid CurrentDataGrid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public ReturnControl()
        {
            InitializeComponent();

            DataContext = this;

            CurrentDataGrid = PStoreOrderDetail;
        }

        internal void SetDataContext(StoreOrder storeOrder)
        {
            StoreOrderData = storeOrder;

            InitPrincipal();

            UpdateOrderDetailUi();

            StoreOrderData.IsDataChanged = false;
        }

        private void InitPrincipal()
        {
            PrincipalCollection = ManufactoryDb.GetPrincipal(StoreOrderData.Manufactory.Id);

            if (StoreOrderData.Principal.Id == "")
                PrincipalCombo.SelectedIndex = 0;
        }

        #region ----- DataGrid Functions -----
        private void UpdateOrderDetailUi()
        {
            CurrentDataGrid.ItemsSource = null;

            switch (StoreOrderData.Type)
            {
                case OrderType.PROCESSING:
                    MainGrid.RowDefinitions[3].Height = new GridLength(0);
                    MainGrid.RowDefinitions[4].Height = new GridLength(1, GridUnitType.Star);
                    MainGrid.RowDefinitions[5].Height = new GridLength(0);
                    MainGrid.RowDefinitions[6].Height = new GridLength(50);

                    CurrentDataGrid = GStoreOrderDetail;
                    break;
                case OrderType.UNPROCESSING:
                    MainGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
                    MainGrid.RowDefinitions[4].Height = new GridLength(0);
                    MainGrid.RowDefinitions[5].Height = new GridLength(50);
                    MainGrid.RowDefinitions[6].Height = new GridLength(0);

                    CurrentDataGrid = PStoreOrderDetail;
                    break;
            }

            CurrentDataGrid.ItemsSource = StoreOrderData.Products;

            UpdatePricipalStackUi();
        }
        private void UpdatePricipalStackUi()
        {
            if (StoreOrderData.Principal.Name.Equals("新增負責人"))
            {
                HasPrincipalStack.Visibility = Visibility.Collapsed;
                DontHasPrincipalStack.Visibility = Visibility.Visible;
            }
            else
            {
                HasPrincipalStack.Visibility = Visibility.Visible;
                DontHasPrincipalStack.Visibility = Visibility.Collapsed;
            }
        }
        
        internal void ClearControl()
        {
            StoreOrderData = null;
            CurrentDataGrid.ItemsSource = null;
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is IDeletable)
            {
                if (StoreOrderData.Products.Contains(selectedItem))
                {
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";
                }

                CurrentDataGrid.SelectedItem = selectedItem;
                return;
            }

            CurrentDataGrid.SelectedIndex = StoreOrderData.Products.Count;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;

            if (leaveItem is IDeletable)
            {
                (leaveItem as IDeletable).Source = string.Empty;
            }
        }
        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StoreOrderData.Products.Remove((Product)CurrentDataGrid.SelectedItem);
            StoreOrderData.CalculateTotalPrice();

            StoreOrderData.IsDataChanged = true;

        }
        private void CheckAndAddProductByBatchOverview(TextBox textBox, PurchaseProduct selectedItem, bool isFirst = true)
        {
            Collection<BatchNumOverview> batchNumOverviews = StoreOrderDb.GetBatchNumOverview(selectedItem.Id, StoreOrderData.Warehouse.Id);

            if (batchNumOverviews.Count > 1)
            {
                BatchNumberDialog batchNumberDialog = new BatchNumberDialog(batchNumOverviews);
                batchNumberDialog.ShowDialog();

                if (!batchNumberDialog.IsConfirmClicked) return;

                foreach (var batch in batchNumberDialog.BatchNumOverviews)
                {
                    if (batch.SelectedAmount <= 0) continue;

                    AddProductByBatchOverview(textBox, selectedItem, batch, isFirst);
                    isFirst = false;
                }
            }
            else
            {
                AddProductByBatchOverview(textBox, selectedItem, batchNumOverviews[0], isFirst);
            }
        }

        private void AddProductByBatchOverview(TextBox textBox, PurchaseProduct purchaseProduct, BatchNumOverview batchNumOverview, bool isFirst)
        {
            if (StoreOrderData.Products.Count(p => p.Id.Equals(purchaseProduct.Id) && ((IProductReturn)p).BatchNumber.Equals(batchNumOverview.BatchNumber)) > 0)
            {
                MessageWindow messageWindow = new MessageWindow($"處理單內已經有{purchaseProduct.Id}(批號 {batchNumOverview.BatchNumber})!", MessageType.WARNING);
                messageWindow.ShowDialog();
                return;
            }

            Product newProduct;

            if (purchaseProduct.Type.Equals("M"))
                newProduct = new ProductReturnMedicine(purchaseProduct);
            else
                newProduct = new ProductReturnOTC(purchaseProduct);

            ((IProductReturn)newProduct).BatchNumber = batchNumOverview.BatchNumber;
            ((IProductReturn)newProduct).BatchLimit = batchNumOverview.Amount;
            ((ITrade)newProduct).Amount = batchNumOverview.SelectedAmount;

            int rowIndex = GetCurrentRowIndex(textBox);

            if(isFirst)
            {
                if (rowIndex == CurrentDataGrid.Items.Count - 1)
                {
                    StoreOrderData.Products.Add(newProduct);

                    textBox.Text = "";
                }
                else
                {
                    ((IProductReturn)newProduct).CopyFilledData(StoreOrderData.Products[rowIndex]);

                    StoreOrderData.Products[rowIndex] = newProduct;
                }
            }
            else
            {
                StoreOrderData.Products.Insert(rowIndex, newProduct);
            }

            StoreOrderData.IsDataChanged = true;
        }
        #endregion

        #region ----- P StoreOrderDetail Functions -----
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            if (textBox.Text == String.Empty)
                textBox.Text = "0";

            if (!textBox.Name.Equals("FreeAmount"))
                storeOrderData.CalculateTotalPrice();

            StoreOrderData.IsDataChanged = true;
        }

        private void Id_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is null) return;

            TextBox textBox = sender as TextBox;

            if (e.Key == Key.Enter)
            {
                NewItemDialog newItemDialog = new NewItemDialog(StoreOrderCategory.RETURN, ProductCollection, StoreOrderData.Manufactory.Id, StoreOrderData.Warehouse.Id, textBox.Text);

                if (newItemDialog.ConfirmButtonClicked)
                {
                    CheckAndAddProductByBatchOverview( textBox, newItemDialog.SelectedItem);
                }
            }
        }

        private void Id_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;

            TextBox textBox = sender as TextBox;

            textBox.SelectAll();
        }

        private void Id_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is null) return;

            TextBox textBox = sender as TextBox;

            e.Handled = true;

            textBox.Focus();
        }

        private void Id_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;

            TextBox textBox = sender as TextBox;

            var currentRowIndex = GetCurrentRowIndex(sender);

            if (currentRowIndex == -1 || currentRowIndex == CurrentDataGrid.Items.Count - 1) return;

            if (!textBox.Text.Equals(storeOrderData.Products[currentRowIndex].Id))
                textBox.Text = storeOrderData.Products[currentRowIndex].Id;
        }
        #endregion

        #region ----- Service Functions -----
        private int GetCurrentRowIndex(object sender)
        {
            if (sender is Button)
            {
                Button btn = sender as Button;

                List<Button> temp = new List<Button>();
                NewFunction.FindChildGroup<Button>(CurrentDataGrid, btn.Name, ref temp);
                for (int x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(btn))
                    {
                        return x;
                    }
                }
            }
            else if (sender is TextBox)
            {
                TextBox tb = sender as TextBox;

                List<TextBox> temp = new List<TextBox>();
                NewFunction.FindChildGroup<TextBox>(CurrentDataGrid, tb.Name, ref temp);
                for (int x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(tb))
                    {
                        return x;
                    }
                }
            }

            return -1;
        }

        private bool IsNumbers(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9) return true;
            if (key >= Key.NumPad0 && key <= Key.NumPad9) return true;

            return false;
        }

        private bool IsKeyAvailable(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9) return true;
            if (key >= Key.NumPad0 && key <= Key.NumPad9) return true;
            if (key == Key.Back || key == Key.Delete || key == Key.Left || key == Key.Right || key == Key.OemPeriod || key == Key.Decimal) return true;

            return false;
        }
        #endregion

        private void NewProduct(object sender, RoutedEventArgs e)
        {
            NewItemDialog newItemDialog = new NewItemDialog(StoreOrderCategory.RETURN, ProductCollection, StoreOrderData.Manufactory.Id, StoreOrderData.Warehouse.Id);

            newItemDialog.ShowDialog();

            if (newItemDialog.ConfirmButtonClicked)
            {
                List<TextBox> temp = new List<TextBox>();
                NewFunction.FindChildGroup<TextBox>(CurrentDataGrid, "Id", ref temp);

                CheckAndAddProductByBatchOverview(temp.Last(), newItemDialog.SelectedItem, false);
            }
        }

        private void Principal_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePricipalStackUi();

            StoreOrderData.IsDataChanged = true;
        }
        
    }
}
