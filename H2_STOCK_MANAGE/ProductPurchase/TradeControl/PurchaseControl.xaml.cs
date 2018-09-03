using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private int totalPage;
        public int TotalPage
        {
            get
            {
                return totalPage;
            }
            set
            {
                totalPage = value;
                NotifyPropertyChanged("TotalPage");
            }
        }

        private int currentPage;
        public int CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                currentPage = value;
                NotifyPropertyChanged("CurrentPage");
            }
        }

        private const int PRODUCT_PER_PAGE = 12;

        enum PagingType
        {
            INIT,
            DEL,
            ADD,
            SPLIT
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
        
        public PurchaseControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        internal void SetDataContext(StoreOrder storeOrder)
        {
            StoreOrderData = storeOrder;
            
            InitPrincipal();

            UpdateOrderDetailUi();
            
            PreparePaging(PagingType.INIT);

            StoreOrderData.IsDataChanged = false;
        }

        private void InitPrincipal()
        {
            PrincipalCollection = ManufactoryDb.GetPrincipal(StoreOrderData.Manufactory.Id);

            if (StoreOrderData.Principal.Id == "")
                PrincipalCombo.SelectedIndex = 0;
        }

        #region ----- StoreOrderDetail Functions -----
        private void UpdateOrderDetailUi()
        {
            AddNewProduct.IsEnabled = true;
            DeleteOrder.IsEnabled = true;
            ConfirmToProcess.IsEnabled = true;
            Confirm.IsEnabled = true;

            switch (StoreOrderData.Type)
            {
                case OrderType.PROCESSING:
                    Confirm.Visibility = Visibility.Visible;
                    ConfirmToProcess.Visibility = Visibility.Collapsed;
                    DeleteOrder.Visibility = Visibility.Collapsed;
                    EmptySpace.Width = 400;
                    StoreOrderDetail.Columns[11].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[12].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[13].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[5].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[6].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[7].Visibility = Visibility.Collapsed;
                    break;
                case OrderType.UNPROCESSING:
                    Confirm.Visibility = Visibility.Collapsed;
                    ConfirmToProcess.Visibility = Visibility.Visible;
                    DeleteOrder.Visibility = Visibility.Visible;
                    EmptySpace.Width = 270;
                    StoreOrderDetail.Columns[11].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[12].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[13].Visibility = Visibility.Collapsed;
                    StoreOrderDetail.Columns[5].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[6].Visibility = Visibility.Visible;
                    StoreOrderDetail.Columns[7].Visibility = Visibility.Visible;
                    break;
            }

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

            if (dataGrid.Items.Count == e.Row.GetIndex() + 1) return;

            int rowNum = (e.Row.GetIndex() + 1) + (CurrentPage - 1) * PRODUCT_PER_PAGE;

            e.Row.Header = rowNum.ToString();
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

                StoreOrderDetail.SelectedItem = selectedItem;
                return;
            }

            StoreOrderDetail.SelectedIndex = StoreOrderData.Products.Count;
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

                var focusedCell = StoreOrderDetail.CurrentCell.Column.GetCellContent(StoreOrderDetail.CurrentCell.Item);

                while (true)
                {
                    if (focusedCell is ContentPresenter)
                    {
                        UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                        if (!(child is Image))
                            break;
                    }

                    focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                    focusedCell = StoreOrderDetail.CurrentCell.Column.GetCellContent(StoreOrderDetail.CurrentCell.Item);
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

            if (rowIndex == StoreOrderDetail.Items.Count - 1)
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
            StoreOrderData.Products.Remove((Product)StoreOrderDetail.SelectedItem);
            StoreOrderData.CalculateTotalPrice();

            PreparePaging(PagingType.DEL);

            StoreOrderData.IsDataChanged = true;
        }

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
            
            PreparePaging(PagingType.SPLIT);
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

                    PreparePaging(PagingType.ADD);
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

            if (currentRowIndex == -1 || currentRowIndex == StoreOrderDetail.Items.Count - 1) return;

            if (!textBox.Text.Equals(storeOrderData.Products[currentRowIndex].Id))
                textBox.Text = storeOrderData.Products[currentRowIndex].Id;
        }
        #endregion

        #region ----- Paging Functions -----
        private void PreparePaging(PagingType type)
        {
            if (storeOrderData.Products.Count == 0)
                TotalPage = 1;
            else
                TotalPage = (storeOrderData.Products.Count / PRODUCT_PER_PAGE) + ((storeOrderData.Products.Count % PRODUCT_PER_PAGE == 0) ? 0 : 1);
            
            switch (type)
            {
                case PagingType.INIT:
                    CurrentPage = 1;
                    break;
                case PagingType.DEL:
                    if (StoreOrderDetail.Items.Count == 1)
                    {
                        CurrentPage = TotalPage;
                    }
                    break;
                case PagingType.ADD:
                    CurrentPage = TotalPage;
                    break;
                case PagingType.SPLIT:
                    break;
            }

            SelectPage();
        }

        private void SelectPage()
        {
            StoreOrderDetail.ItemsSource = storeOrderData.Products.Skip(PRODUCT_PER_PAGE * (currentPage - 1)).Take(PRODUCT_PER_PAGE).ToList();
        }
        private void ChangePage(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;

            Button button = sender as Button;

            switch (button.Tag.ToString())
            {
                case "First":
                    CurrentPage = 1;
                    break;
                case "Minus":
                    if (CurrentPage - 1 >= 1)
                        CurrentPage--;
                    else
                        CurrentPage = 1;
                    break;
                case "Plus":
                    if (CurrentPage + 1 <= TotalPage)
                        CurrentPage++;
                    else
                        CurrentPage = TotalPage;
                    break;
                case "Last":
                    CurrentPage = TotalPage;
                    break;
            }

            SelectPage();
        }

        private void ChangeCurrentPage_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;

            TextBox textBox = sender as TextBox;
            CheckPageValid(textBox);

            SelectPage();
        }

        private void CheckPageValid(TextBox textBox)
        {
            int selectPage = Int32.Parse(textBox.Text.ToString());

            if (selectPage < 1)
                selectPage = 1;
            else if (selectPage > TotalPage)
                selectPage = TotalPage;

            CurrentPage = selectPage;
        }

        private void ChangeCurrentPage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is null) return;

            TextBox textBox = sender as TextBox;

            if (e.Key == Key.Enter)
            {
                CheckPageValid(textBox);

                SelectPage();
            }
            else if (!IsNumbers(e.Key))
                e.Handled = true;
        }

        #endregion

        #region ----- Service Functions -----
        private int GetCurrentRowIndex(object sender)
        {
            if (sender is AutoCompleteBox)
            {
                AutoCompleteBox productAuto = sender as AutoCompleteBox;

                List<AutoCompleteBox> temp = new List<AutoCompleteBox>();
                NewFunction.FindChildGroup<AutoCompleteBox>(StoreOrderDetail, productAuto.Name, ref temp);
                for (int x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(productAuto))
                    {
                        return x;
                    }
                }
            }
            else if (sender is Button)
            {
                Button btn = sender as Button;

                List<Button> temp = new List<Button>();
                NewFunction.FindChildGroup<Button>(StoreOrderDetail, btn.Name, ref temp);
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
                NewFunction.FindChildGroup<TextBox>(StoreOrderDetail, tb.Name, ref temp);
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

                PreparePaging(PagingType.ADD);
            }
        }

        private void Principal_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePricipalStackUi();

            StoreOrderData.IsDataChanged = true;
        }
    }
}
