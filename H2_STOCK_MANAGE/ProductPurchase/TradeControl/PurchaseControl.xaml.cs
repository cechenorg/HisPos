using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using MahApps.Metro.Controls;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl
{
    /// <summary>
    /// PurchaseControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseControl : UserControl, INotifyPropertyChanged
    {
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
        
        public PurchaseControl()
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
                    MainGrid.RowDefinitions[2].Height = new GridLength(0);
                    MainGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
                    MainGrid.RowDefinitions[4].Height = new GridLength(0);
                    MainGrid.RowDefinitions[5].Height = new GridLength(50);

                    CurrentDataGrid = GStoreOrderDetail;
                    break;
                case OrderType.UNPROCESSING:
                    MainGrid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
                    MainGrid.RowDefinitions[3].Height = new GridLength(0);
                    MainGrid.RowDefinitions[4].Height = new GridLength(50);
                    MainGrid.RowDefinitions[5].Height = new GridLength(0);
                    
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

        private void StoreOrderDetail_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (sender is null) return;

            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid.Items.Count == e.Row.GetIndex() + 1 && storeOrderData.type == OrderType.UNPROCESSING) return;

            int rowNum = (e.Row.GetIndex() + 1);

            if (e.Row.Header is null)
                e.Row.Header = rowNum.ToString();
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

        private void StoreOrderDetail_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            StoreOrderData.IsDataChanged = true;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (sender is TextBox)
                {
                    (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                    if (Keyboard.FocusedElement is Button)
                        (Keyboard.FocusedElement as Button).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                else
                {
                    UIElement child = (sender as AutoCompleteBox).FindChild<TextBox>("Text");

                    child.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }

                var focusedCell = CurrentDataGrid.CurrentCell.Column.GetCellContent(CurrentDataGrid.CurrentCell.Item);

                while (true)
                {
                    if (focusedCell is ContentPresenter)
                    {
                        UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                        if (!(child is Image))
                            break;
                    }

                    focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                    focusedCell = CurrentDataGrid.CurrentCell.Column.GetCellContent(CurrentDataGrid.CurrentCell.Item);
                }

                UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                if (firstChild is TextBox)
                    firstChild.Focus();
                else
                {
                    UIElement secondChild = (UIElement)VisualTreeHelper.GetChild(firstChild, 0);

                    if (secondChild is AutoCompleteBox)
                    {
                        secondChild.FindChild<TextBox>("Text").Focus();
                    }
                    else
                    {
                        secondChild.Focus();
                    }
                }
            }
        }
        private void AddProduct(TextBox textBox, PurchaseProduct product)
        {
            Product newProduct;

            if (product.Type.Equals("M"))
                newProduct = new ProductPurchaseMedicine(product);
            else
                newProduct = new ProductPurchaseOtc(product);

            int rowIndex = GetCurrentRowIndex(textBox);

            if (rowIndex == CurrentDataGrid.Items.Count - 1)
            {
                StoreOrderData.Products.Add(newProduct);

                textBox.Text = "";
            }
            else
            {
                ((IProductPurchase)newProduct).CopyFilledData(StoreOrderData.Products[rowIndex]);

                StoreOrderData.Products[rowIndex] = newProduct;
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

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StoreOrderData.Products.Remove((Product)CurrentDataGrid.SelectedItem);
            StoreOrderData.CalculateTotalPrice();
            
            StoreOrderData.IsDataChanged = true;
        }

        

        private void Id_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is null) return;

            TextBox textBox = sender as TextBox;

            if (e.Key == Key.Enter)
            {
                NewItemDialog newItemDialog = new NewItemDialog(ProductCollection, StoreOrderData.Manufactory.Id, textBox.Text, StoreOrderData.Warehouse.Id);

                if (newItemDialog.ConfirmButtonClicked)
                {
                    AddProduct(textBox, newItemDialog.SelectedItem);
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
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

        #region ----- G StoreOrderDetail Functions -----
        private void SplitBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;

            var currentRowIndex = GetCurrentRowIndex(sender);

            double left = ((ITrade)StoreOrderData.Products[currentRowIndex]).Amount % 2;

            ((ITrade)StoreOrderData.Products[currentRowIndex]).Amount = ((int)((ITrade)StoreOrderData.Products[currentRowIndex]).Amount / 2);

            StoreOrderData.Products.Insert(currentRowIndex + 1, ((ICloneable)StoreOrderData.Products[currentRowIndex]).Clone() as Product);

            if (left != 0)
                ((ITrade)StoreOrderData.Products[currentRowIndex]).Amount += left;

            StoreOrderData.IsDataChanged = true;

            CurrentDataGrid.Items.Refresh();
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
        #endregion
        
        private void NewProduct(object sender, RoutedEventArgs e)
        {
            NewItemDialog newItemDialog = new NewItemDialog(ProductCollection, StoreOrderData.Manufactory.Id, StoreOrderData.Warehouse.Id);

            newItemDialog.ShowDialog();

            if (newItemDialog.ConfirmButtonClicked)
            {
                //SetChanged();
                if (newItemDialog.SelectedItem.Type.Equals("M"))
                    StoreOrderData.Products.Add(new ProductPurchaseMedicine(newItemDialog.SelectedItem));
                else
                    StoreOrderData.Products.Add(new ProductPurchaseOtc(newItemDialog.SelectedItem));
            }
        }

        private void Principal_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePricipalStackUi();

            StoreOrderData.IsDataChanged = true;
        }

        private void ShowDeclareDataOverview(object sender, MouseButtonEventArgs e)
        {
            DeclareDataDetailOverview declareDataDetailOverview = new DeclareDataDetailOverview();
            declareDataDetailOverview.Show();
        }
    }

    public class HasDeclareDataToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || (int)value == 0) return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }

}
