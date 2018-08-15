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
using His_Pos.Class.Product;
using His_Pos.Class.StoreOrder;
using His_Pos.Interface;
using His_Pos.ProductPurchase;
using His_Pos.Service;
using His_Pos.Struct.Product;
using MahApps.Metro.Controls;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl
{
    /// <summary>
    /// PurchaseControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseControl : UserControl, INotifyPropertyChanged
    {

        public Collection<PurchaseProduct> ProductAutoCompleteCollection { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public PurchaseControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        internal void SetDataContext(StoreOrder storeOrder)
        {
            StoreOrderData = storeOrder;

            UpdateOrderDetailUi();
        }

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
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (sender is TextBox)
                {
                    (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                    if(Keyboard.FocusedElement is Button)
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

        private void ProductID_Populating(object sender, PopulatingEventArgs e)
        {
            var productAuto = sender as AutoCompleteBox;

            if (String.IsNullOrEmpty(storeOrderData.Manufactory.Id) || productAuto is null || ProductAutoCompleteCollection is null) return;

            var result = ProductAutoCompleteCollection.Where(x => (x.Id.ToLower().Contains(productAuto.Text.ToLower()) || x.ChiName.ToLower().Contains(productAuto.Text.ToLower()) || x.EngName.ToLower().Contains(productAuto.Text.ToLower()))).Take(50);
            
            productAuto.ItemsSource = new ObservableCollection<PurchaseProduct>(result.ToList());
            productAuto.ItemFilter = ProductFilter;
            productAuto.PopulateComplete();
        }

        private void ProductID_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var productAuto = sender as AutoCompleteBox;

            if (productAuto is null) return;
            if (productAuto.SelectedItem is null)
            {
                if (productAuto.Text != string.Empty && (productAuto.ItemsSource as ObservableCollection<PurchaseProduct>).Count != 0 && productAuto.Text.Length >= 4)
                    productAuto.SelectedItem = (productAuto.ItemsSource as ObservableCollection<PurchaseProduct>)[0];
                else
                    return;
            }

            Product newProduct;

            if (((PurchaseProduct)productAuto.SelectedItem).Type.Equals("M"))
                newProduct = new ProductPurchaseMedicine((PurchaseProduct)productAuto.SelectedItem);
            else
                newProduct = new ProductPurchaseOtc((PurchaseProduct)productAuto.SelectedItem);

            int rowIndex = GetCurrentRowIndex(productAuto);

            if (rowIndex == StoreOrderData.Products.Count)
            {
                StoreOrderData.Products.Add(newProduct);
                
                productAuto.Text = "";
            }
            else
            {
                ((IProductPurchase)newProduct).CopyFilledData(StoreOrderData.Products[rowIndex]);

                StoreOrderData.Products[rowIndex] = newProduct;
            }
        }

        private int GetCurrentRowIndex(AutoCompleteBox productAuto)
        {
            List<AutoCompleteBox> temp = new List<AutoCompleteBox>();
            NewFunction.FindChildGroup<AutoCompleteBox>(StoreOrderDetail, productAuto.Name, ref temp);
            for (int x = 0; x < temp.Count; x++)
            {
                if (temp[x].Equals(productAuto))
                {
                    return x;
                }
            }

            return -1;
        }

        public AutoCompleteFilterPredicate<object> ProductFilter
        {
            get
            {
                return (searchText, obj) =>
                    ((PurchaseProduct)obj).Id.ToLower().Contains(searchText.ToLower())
                    || ((PurchaseProduct)obj).ChiName.ToLower().Contains(searchText.ToLower())
                    || ((PurchaseProduct)obj).EngName.ToLower().Contains(searchText.ToLower());
            }
        }

        private void NewProduct(object sender, RoutedEventArgs e)
        {
            NewItemDialog newItemDialog = new NewItemDialog(ProductAutoCompleteCollection, StoreOrderData.Manufactory.Id);

            newItemDialog.ShowDialog();

            if (newItemDialog.ConfirmButtonClicked)
            {
                //SetChanged();
                StoreOrderData.Products.Add(newItemDialog.SelectedItem as Product);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            if (textBox.Text == String.Empty)
                textBox.Text = "0";

            if (!textBox.Name.Equals("FreeAmount"))
                storeOrderData.CalculateTotalPrice();
        }
        
    }
}
