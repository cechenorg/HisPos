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

namespace His_Pos.ProductPurchase
{
    /// <summary>
    /// NewItemDialog.xaml 的互動邏輯
    /// </summary>
    public partial class NewItemDialog : Window
    {
        private ObservableCollection<object> DataCollection;
        Collection<ColumnDetail> ColumnDetails;
        private ItemType ItemType;
        public string Para;

        public object SelectedItem;
        public bool ConfirmButtonClicked = false;

        struct ColumnDetail
        {
            public string Header;
            public string Binding;
            public int Width;
            public Style Style;

            public ColumnDetail(string header, string binding, int width, Style style = null)
            {
                Header = header;
                Binding = binding;
                Width = width;
                Style = style;
            }
        }
        public NewItemDialog(ItemType type, ObservableCollection<object> collection)
        {
            InitializeComponent();
            Title = "新增";

            ItemType = type;
            DataCollection = collection;

            InitUi();

            InitColumnDetails();

            InitCollection();
        }

        private void InitUi()
        {
            switch (ItemType)
            {
                case ItemType.Product:
                    OnlyManufactory.Visibility = Visibility.Visible;
                    AllProducts.Visibility = Visibility.Visible;
                    break;
            }

        }

        private void InitCollection()
        {
            SearchResult.ItemsSource = DataCollection;
            SearchResult.Items.Filter = SearchFilter;
        }

        private void InitColumnDetails()
        {
            switch(ItemType)
            {
                case ItemType.Product:

                    Style textInMidStyle = this.FindResource("TextInMiddleCellStyle") as Style;

                    ColumnDetails = new Collection<ColumnDetail>() {
                                    new ColumnDetail("商品編號", "Product.Id", 120, textInMidStyle),
                                    new ColumnDetail("商品名稱", "Product.Name", 350),
                                    new ColumnDetail("庫存", "Product.Stock.Inventory", 50, textInMidStyle),
                                    new ColumnDetail("安全量", "Product.Stock.SafeAmount", 70, textInMidStyle),
                                    new ColumnDetail("基準量", "Product.Stock.BasicAmount", 70, textInMidStyle)};
                    break;
            }

            updateColumns();
        }

        private void updateColumns()
        {
            foreach(ColumnDetail columnDetail in ColumnDetails)
            { 
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = columnDetail.Header;
                column.Binding = new Binding(columnDetail.Binding);
                column.Width = columnDetail.Width;
                column.ElementStyle = columnDetail.Style;
                column.IsReadOnly = true;
                SearchResult.Columns.Add(column);
            }
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SearchResult.Items.Filter = SearchFilter;
        }

        private bool SearchFilter(object item)
        {
            switch (ItemType)
            {
                case ItemType.Product:
                    if (!((bool) IsStatusTrue.IsChecked ||
                          ((IProductPurchase) (item as ProductPurchaseView.NewItemProduct).Product).Status))
                        return false;

                    if ((bool) OnlyManufactory.IsChecked)
                    {
                        if (String.IsNullOrEmpty(SearchText.Text))
                            return (item as ProductPurchaseView.NewItemProduct).IsThisMan;

                        if (((item as ProductPurchaseView.NewItemProduct).Product.Id.Contains(SearchText.Text) || (item as ProductPurchaseView.NewItemProduct).Product.Name.Contains(SearchText.Text)) && (item as ProductPurchaseView.NewItemProduct).IsThisMan)
                            return true;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(SearchText.Text))
                            return true;

                        if ((item as ProductPurchaseView.NewItemProduct).Product.Id.Contains(SearchText.Text) || (item as ProductPurchaseView.NewItemProduct).Product.Name.Contains(SearchText.Text))
                            return true;
                    }
                    break;
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

            SelectedItem = (SearchResult.SelectedItem as ProductPurchaseView.NewItemProduct).Product;
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
