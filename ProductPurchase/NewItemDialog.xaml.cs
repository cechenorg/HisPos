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

namespace His_Pos.ProductPurchase
{
    /// <summary>
    /// NewItemDialog.xaml 的互動邏輯
    /// </summary>
    public partial class NewItemDialog : Window
    {
        ObservableCollection<object> DataCollection;
        Collection<ColumnDetail> ColumnDetails;
        private ItemType ItemType;
        private string Para;

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


        public NewItemDialog(ItemType type, string para)
        {
            InitializeComponent();
            ItemType = type;
            Para = para;

            InitColumnDetails();

            InitCollection();
        }

        private void InitCollection()
        {
            switch (ItemType)
            {
                case ItemType.Product:
                    DataCollection = ProductDb.GetItemDialogProduct(Para);
                    SearchResult.ItemsSource = DataCollection;
                    break;
            }
        }

        private void InitColumnDetails()
        {
            switch(ItemType)
            {
                case ItemType.Product:

                    Style textInMidStyle = this.FindResource("TextInMiddleCellStyle") as Style;

                    ColumnDetails = new Collection<ColumnDetail>() {
                                    new ColumnDetail("商品編號", "Id", 90, textInMidStyle),
                                    new ColumnDetail("商品名稱", "Name", 200),
                                    new ColumnDetail("庫存", "Inventory", 50, textInMidStyle),
                                    new ColumnDetail("安全量", "SafeAmount", 60, textInMidStyle),
                                    new ColumnDetail("基準量", "BasicAmount", 60, textInMidStyle)};
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
                    if ((item as Product).Id.Contains(SearchText.Text) || (item as Product).Name.Contains(SearchText.Text))
                        return true;
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

            SelectedItem = SearchResult.SelectedItem;
            ConfirmButtonClicked = true;
            Close();
        }
    }
}
