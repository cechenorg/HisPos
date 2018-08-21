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

        public string ManufactoryID;

        public NewItemDialog(Collection<PurchaseProduct> collection, string id)
        {
            InitializeComponent();
            Title = "新增";
            
            DataCollection = collection;
            ManufactoryID = id;

            InitCollection();
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

            if ((bool) OnlyManufactory.IsChecked)
            {
                if (String.IsNullOrEmpty(SearchText.Text))
                    return ((PurchaseProduct)item).Mans.Contains(ManufactoryID);

                if ((((PurchaseProduct)item).Id.Contains(SearchText.Text) || ((PurchaseProduct)item).Name.Contains(SearchText.Text)) && ((PurchaseProduct)item).Mans.Contains(ManufactoryID))
                    return true;
            }
            else
            {
                if (String.IsNullOrEmpty(SearchText.Text))
                    return true;

                if (((PurchaseProduct)item).Id.Contains(SearchText.Text) || ((PurchaseProduct)item).Name.Contains(SearchText.Text))
                    return true;
            }

            return false;
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            if (SearchResult.SelectedItem is null)
            {
                MessageWindow messageWindow = new MessageWindow("請選擇一個項目!", MessageType.ERROR);
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
    }
}
