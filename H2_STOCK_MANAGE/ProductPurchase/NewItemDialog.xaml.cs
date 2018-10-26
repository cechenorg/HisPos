using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Product;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using His_Pos.Interface;
using His_Pos.Struct.Product;
using His_Pos.Class.StoreOrder;

namespace His_Pos.ProductPurchase
{
    /// <summary>
    /// NewItemDialog.xaml 的互動邏輯
    /// </summary>
    public partial class NewItemDialog : Window
    {
        private Collection<PurchaseProduct> DataCollection;

        public PurchaseProduct SelectedItem;
        public bool ConfirmButtonClicked = false;
        public string WareId;
        public string ManufactoryID;

        private bool IsZeroShow = true;
        
        public NewItemDialog(StoreOrderCategory category, Collection<PurchaseProduct> collection, string manId, string warId, string searchText = "")
        {
            InitializeComponent();
            Title = "新增";

            DataCollection = collection;
            ManufactoryID = manId;
            WareId = warId;
            SearchText.Text = searchText;

            if (category == StoreOrderCategory.RETURN)
                IsZeroShow = false;

            InitCollection();

            if( !SearchText.Text.Equals("") )
            {
                AllProducts.IsChecked = true;

                if (SearchResult.Items.Count == 0)
                {
                    MessageWindow messageWindow = new MessageWindow("查無商品!", MessageType.ERROR, true);
                    messageWindow.ShowDialog();
                    ConfirmButtonClicked = true;
                    Close();
                }
                else if (SearchResult.Items.Count == 1)
                {
                    SelectedItem = (PurchaseProduct)SearchResult.Items[0];
                    ConfirmButtonClicked = true;
                    Close();
                }
                else
                {
                    SearchResult.SelectedIndex = 0;
                    ShowDialog();
                }
            }
        }
        
        public NewItemDialog(Collection<PurchaseProduct> collection, string manId, string searchText,string warId)
        {
            InitializeComponent();
            Title = "新增";

            DataCollection = collection;
            ManufactoryID = manId;
            WareId = warId;
            SearchText.Text = searchText;
            AllProducts.IsChecked = true;

            InitCollection();

            if(SearchResult.Items.Count == 0)
            {
                MessageWindow messageWindow = new MessageWindow("查無商品!", MessageType.ERROR, true);
                messageWindow.ShowDialog();
                Close();
            }
            else if (SearchResult.Items.Count == 1)
            {
                SelectedItem = (PurchaseProduct)SearchResult.Items[0];
                ConfirmButtonClicked = true;
                Close();
            }
            else
                ShowDialog();
        }
        private void InitCollection()
        {
            SearchResult.ItemsSource = DataCollection;
            SearchResult.Items.Filter = SearchFilter;
        }
        
        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SearchResult.Items.Filter = SearchFilter;
        }

        private bool SearchFilter(object item)
        {
            if (!((bool) IsStatusTrue.IsChecked ||
                  ((PurchaseProduct)item).Status))
                return false;

            if(!IsZeroShow)
            {
                if ((((PurchaseProduct)item).Inventory <= 0))
                    return false;
            }

            if ((bool) OnlyManufactory.IsChecked)
            {
                if (String.IsNullOrEmpty(SearchText.Text) && (((PurchaseProduct)item).WarId == WareId || WareId == null) )
                    return ((PurchaseProduct)item).Mans.Contains(ManufactoryID);

                if ((((PurchaseProduct)item).Id.Contains(SearchText.Text) || ((PurchaseProduct)item).Name.Contains(SearchText.Text)) && ((PurchaseProduct)item).Mans.Contains(ManufactoryID)
                      && (((PurchaseProduct)item).WarId == WareId || WareId == null)
                    )
                    return true;
            }
            else
            {
                if (String.IsNullOrEmpty(SearchText.Text)
                       && (((PurchaseProduct)item).WarId == WareId || WareId == null)
                    )
                    return true;

                if ((((PurchaseProduct)item).Id.Contains(SearchText.Text) || ((PurchaseProduct)item).Name.Contains(SearchText.Text))
                    && (((PurchaseProduct)item).WarId == WareId || WareId == null)
                    )
                    return true;
            }

            return false;
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            ConfirmSelectResult();
        }

        private void ConfirmSelectResult()
        {
            if (SearchResult.SelectedItem is null)
            {
                MessageWindow messageWindow = new MessageWindow("請選擇一個項目!", MessageType.ERROR, true);
                messageWindow.ShowDialog();
                return;
            }

            SelectedItem = (PurchaseProduct)SearchResult.SelectedItem;
            ConfirmButtonClicked = true;
            Close();
        }

        private void Radio_OnChecked(object sender, RoutedEventArgs e)
        {
            if (SearchResult is null) return;

            SearchResult.Items.Filter = SearchFilter;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is null) return;

            ItemGrid.RowDefinitions[1].Height = new GridLength((sender as NewItemDialog).Height - 140);
        }

        private void NewItemDialog_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            int currentIndex = -1;

            if (e.Key == Key.Up)
            {
                currentIndex = SearchResult.SelectedIndex;

                if (currentIndex == -1) return;
                else if (currentIndex - 1 > -1)
                    SearchResult.SelectedIndex = currentIndex - 1;

                SearchResult.ScrollIntoView(SearchResult.SelectedItem);
            }
            else if (e.Key == Key.Down)
            {
                currentIndex = SearchResult.SelectedIndex;

                if (currentIndex == -1) return;
                else if (currentIndex + 1 <= SearchResult.Items.Count - 1)
                    SearchResult.SelectedIndex = currentIndex + 1;

                SearchResult.ScrollIntoView(SearchResult.SelectedItem);
            }
            else if (e.Key == Key.Enter)
            {
                ConfirmSelectResult();
            }
        }
    }
}
