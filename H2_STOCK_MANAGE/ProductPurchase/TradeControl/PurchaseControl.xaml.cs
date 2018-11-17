using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using His_Pos.Class.StoreOrder;
using His_Pos.Interface;
using His_Pos.ProductPurchase;
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

        private Product CurrentProduct { get; set; }

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

            StoreOrderData.CalculateTotalPrice();
            
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
                    MainGrid.RowDefinitions[4].Height = new GridLength(0);
                    MainGrid.RowDefinitions[7].Height = new GridLength(0);
                    MainGrid.RowDefinitions[8].Height = new GridLength(50);
                    MainGrid.RowDefinitions[9].Height = new GridLength(0);
                    
                    if (StoreOrderData.Manufactory.Id.Equals("0"))
                    {
                        CurrentDataGrid = WStoreOrderDetail;

                        MainGrid.RowDefinitions[5].Height = new GridLength(0);
                        MainGrid.RowDefinitions[6].Height = new GridLength(1, GridUnitType.Star);
                        
                        CurrentDataGrid.Columns[4].Visibility = Visibility.Collapsed;
                        CurrentDataGrid.Columns[5].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        CurrentDataGrid = GStoreOrderDetail;

                        MainGrid.RowDefinitions[5].Height = new GridLength(1, GridUnitType.Star);
                        MainGrid.RowDefinitions[6].Height = new GridLength(0);
                    }
                    break;
                case OrderType.UNPROCESSING:
                    
                    MainGrid.RowDefinitions[5].Height = new GridLength(0);
                    MainGrid.RowDefinitions[6].Height = new GridLength(0);
                    MainGrid.RowDefinitions[7].Height = new GridLength(50);
                    MainGrid.RowDefinitions[8].Height = new GridLength(0);
                    MainGrid.RowDefinitions[9].Height = new GridLength(0);


                    if (StoreOrderData.Manufactory.Id.Equals("0"))
                    {
                        CurrentDataGrid = PSStoreOrderDetail;

                        MainGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
                        MainGrid.RowDefinitions[4].Height = new GridLength(0);

                        SetCurrentPrice();
                    }
                    else
                    {
                        CurrentDataGrid = PStoreOrderDetail;

                        MainGrid.RowDefinitions[3].Height = new GridLength(0);
                        MainGrid.RowDefinitions[4].Height = new GridLength(1, GridUnitType.Star);

                        CurrentDataGrid.Columns[10].Visibility = Visibility.Visible;
                    }
                    break;
                case OrderType.WAITING:
                    MainGrid.RowDefinitions[3].Height = new GridLength(0);
                    MainGrid.RowDefinitions[4].Height = new GridLength(0);
                    MainGrid.RowDefinitions[5].Height = new GridLength(0);
                    MainGrid.RowDefinitions[6].Height = new GridLength(1, GridUnitType.Star);
                    MainGrid.RowDefinitions[7].Height = new GridLength(0);
                    MainGrid.RowDefinitions[8].Height = new GridLength(0);
                    MainGrid.RowDefinitions[9].Height = new GridLength(50);

                    CurrentDataGrid = WStoreOrderDetail;

                    CurrentDataGrid.Columns[4].Visibility = Visibility.Visible;
                    CurrentDataGrid.Columns[5].Visibility = Visibility.Collapsed;
                    break;
            }

            CurrentDataGrid.ItemsSource = StoreOrderData.Products;

            UpdatePricipalStackUi();
        }

        private void SetCurrentPrice()
        {
            foreach (var product in StoreOrderData.Products)
            {
                if (((ITrade) product).Price == 0 && ((IProductPurchase) product).OrderAmount != 0)
                {
                    if(((IProductPurchase)product).OrderAmount >= ((IProductPurchase)product).PackageAmount)
                        ((ITrade)product).Price = ((IProductPurchase)product).PackagePrice;
                    else
                        ((ITrade)product).Price = ((IProductPurchase)product).SingdePrice;
                }
            }
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

        //private void StoreOrderDetail_OnLoadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    if (sender is null) return;

        //    DataGrid dataGrid = sender as DataGrid;

        //    if (dataGrid.Items.Count == e.Row.GetIndex() + 1 && storeOrderData.type == OrderType.UNPROCESSING) return;

        //    int rowNum = (e.Row.GetIndex() + 1);

        //    if (e.Row.Header is null)
        //        e.Row.Header = rowNum.ToString();

        //    e.Row.Header = e.Row.GetIndex();
        //}

        internal void ClearControl()
        {
            StoreOrderData = null;
            CurrentDataGrid.ItemsSource = null;
        }
        private void DataGridRow_MouseEnter(object sender, EventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;
            
            CurrentProduct = selectedItem as Product;

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

        private void DataGridRow_MouseLeave(object sender, EventArgs e)
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
                    TextBox textBox = sender as TextBox;

                    if (textBox.Name.Equals("Id"))
                    {
                        if (CurrentProduct == null || !textBox.Text.Equals(CurrentProduct.Id))
                        {
                            Product currentProduct = ((ICloneable) CurrentProduct)?.Clone() as Product;

                            NewItemDialog newItemDialog = new NewItemDialog(StoreOrderCategory.PURCHASE, ProductCollection, StoreOrderData.Manufactory.Id, StoreOrderData.Warehouse.Id, textBox.Text);

                            if (newItemDialog.ConfirmButtonClicked)
                            {
                                if (string.IsNullOrEmpty(newItemDialog.SelectedItem.Id))
                                {
                                    textBox.Text = "";
                                    textBox.Focus();
                                    return;
                                }

                                if (StoreOrderData.Products.Count(p => p.Id.Equals(newItemDialog.SelectedItem.Id)) > 0)
                                {
                                    MessageWindow messageWindow = new MessageWindow("處理單內已經有此品項!", MessageType.WARNING, true);
                                    messageWindow.ShowDialog();
                                    textBox.Text = "";
                                    textBox.Focus();
                                    return;
                                }

                                AddProduct(textBox, newItemDialog.SelectedItem, currentProduct);
                                return;
                            }
                        }
                    }

                    MoveFocusNext(sender);
                }
            }
            else if ((sender as TextBox).Tag != null && (sender as TextBox).Tag.Equals("CheckInputOnlyNum"))
            {
                if (!IsKeyAvailable(e.Key))
                    e.Handled = true;
            }

        }

        private void MoveFocusNext(object sender)
        {
            (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (Keyboard.FocusedElement is Button)
                (Keyboard.FocusedElement as Button).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if(CurrentDataGrid.CurrentCell.Column is null) return;

            var focusedCell = CurrentDataGrid.CurrentCell.Column.GetCellContent(CurrentDataGrid.CurrentCell.Item);

            if (focusedCell is null) return;

            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                    if (child is StackPanel)
                    {
                        StackPanel stackPanel = child as StackPanel;
                        if (stackPanel.Tag != null && stackPanel.Tag.ToString().Equals("NotSkip"))
                            break;
                    }
                    else if (!(child is Image))
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

                secondChild.Focus();
            }
        }

        private void AddProduct(TextBox textBox, PurchaseProduct product, Product currentProduct)
        {
            Product newProduct;

            if (product.Type.Equals("M"))
                newProduct = new ProductPurchaseMedicine(product, StoreOrderData.Manufactory.Id.Equals("0"));
            else
                newProduct = new ProductPurchaseOtc(product, StoreOrderData.Manufactory.Id.Equals("0"));
            
            if (currentProduct is null)
            {
                StoreOrderData.Products.Add(newProduct);

                textBox.Text = "";

                CurrentDataGrid.CurrentCell = (CurrentDataGrid.Name.Equals("PSStoreOrderDetail"))? new DataGridCellInfo(CurrentDataGrid.Items[StoreOrderData.Products.Count - 1], OrderAmount)
                                                                                                 : new DataGridCellInfo(CurrentDataGrid.Items[StoreOrderData.Products.Count - 1], Price);

                var focusedCell = CurrentDataGrid.CurrentCell.Column.GetCellContent(CurrentDataGrid.CurrentCell.Item);
                var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell ?? throw new InvalidOperationException(), 0);
                while (firstChild is ContentPresenter)
                {
                    firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                }
                firstChild.Focus();
            }
            else
            {
                ((IProductPurchase)newProduct).CopyFilledData(currentProduct);

                Product tempP = StoreOrderData.Products.Single(p => p.Id.Equals(currentProduct.Id));

                int index = StoreOrderData.Products.IndexOf(tempP);

                StoreOrderData.Products.RemoveAt(index);
                StoreOrderData.Products.Insert(index, newProduct);

                CurrentProduct = newProduct;
                textBox.Focus();
            }

            StoreOrderData.IsDataChanged = true;
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StoreOrderData.Products.Remove((Product)CurrentDataGrid.SelectedItem);
            StoreOrderData.CalculateTotalPrice();

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
        

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;

            TextBox textBox = sender as TextBox;

            textBox.SelectAll();
        }

        private void TextBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

            if (CurrentProduct is null) return;

            if (!textBox.Text.Equals(CurrentProduct.Id))
                textBox.Text = CurrentProduct.Id;
        }
        #endregion

        #region ----- G StoreOrderDetail Functions -----
        private void SplitBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (GStoreOrderDetail.SelectedItem is null) return;

            double left = ((ITrade)GStoreOrderDetail.SelectedItem).Amount % 2;

            ((ITrade)GStoreOrderDetail.SelectedItem).Amount = ((int)((ITrade)GStoreOrderDetail.SelectedItem).Amount / 2);

            StoreOrderData.Products.Insert(GStoreOrderDetail.SelectedIndex + 1, ((ICloneable)GStoreOrderDetail.SelectedItem).Clone() as Product);

            if (left != 0)
                ((ITrade)GStoreOrderDetail.SelectedItem).Amount += left;

            StoreOrderData.IsDataChanged = true;
        }
        private void MergeBatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (GStoreOrderDetail.SelectedItem is null) return;

            Product product = GStoreOrderDetail.SelectedItem as Product;

            if (!((IProductPurchase) product).BatchNumber.Equals(""))
            {
                MessageWindow messageWindow = new MessageWindow("此商品有批號無法合批!", MessageType.ERROR, true);
                messageWindow.ShowDialog();

                return;
            }

            ((ITrade)StoreOrderData.Products.Single(p => p.Id == product.Id && ((IProductPurchase)p).IsFirstBatch)).Amount += ((ITrade)product).Amount;

            StoreOrderData.Products.Remove(product);
        }
        private void Amount_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(CurrentProduct is null) return;

            ((ITrade)CurrentProduct).Amount = ((IProductPurchase) CurrentProduct).OrderAmount;
        }
        #endregion

        #region ----- Service Functions -----

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
            NewItemDialog newItemDialog = new NewItemDialog(StoreOrderCategory.PURCHASE, ProductCollection, StoreOrderData.Manufactory.Id, StoreOrderData.Warehouse.Id);

            newItemDialog.ShowDialog();

            if (newItemDialog.ConfirmButtonClicked)
            {
                if(StoreOrderData.Products.Count(p => p.Id.Equals(newItemDialog.SelectedItem.Id)) > 0)
                {
                    MessageWindow messageWindow = new MessageWindow("處理單內已經有此品項!", MessageType.WARNING);
                    messageWindow.ShowDialog();
                    return;
                }

                //SetChanged();
                if (newItemDialog.SelectedItem.Type.Equals("M"))
                    StoreOrderData.Products.Add(new ProductPurchaseMedicine(newItemDialog.SelectedItem, StoreOrderData.Manufactory.Id.Equals("0")));
                else
                    StoreOrderData.Products.Add(new ProductPurchaseOtc(newItemDialog.SelectedItem, StoreOrderData.Manufactory.Id.Equals("0")));
            }
        }

        private void Principal_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StoreOrderData is null) return;

            UpdatePricipalStackUi();

            StoreOrderData.IsDataChanged = true;
        }

        private void ShowDeclareDataOverview(object sender, MouseButtonEventArgs e)
        {
            DeclareDataDetailOverview declareDataDetailOverview = new DeclareDataDetailOverview(StoreOrderData.Id);
            declareDataDetailOverview.ShowDialog();
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
