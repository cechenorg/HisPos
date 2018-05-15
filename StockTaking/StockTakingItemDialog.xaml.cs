using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Interface;
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
using System.Windows.Shapes;

namespace His_Pos.StockTaking
{
    /// <summary>
    /// StockTakingItemDialog.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingItemDialog : Window,INotifyPropertyChanged
    {
        public Product SelectedItem;
        public bool ConfirmButtonClicked = false;
        public ObservableCollection<Product> productsTakingCollection;
        public ObservableCollection<Product> productsCollection;
        public ObservableCollection<Product> ProductsCollection {
            get {
                return productsCollection;
            }
            set{
                productsCollection = value;
                NotifyPropertyChanged("ProductsCollection");
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
        public StockTakingItemDialog(ObservableCollection<Product> products, ObservableCollection<Product> takingCollection)
        {
            InitializeComponent();
            DataContext = this;
            Title = "新增";
            ProductsCollection = products;
            productsTakingCollection = takingCollection;
            SearchResult.Items.Filter = SearchFilter;
        }

        
        
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (SearchResult.SelectedItem is null)
            {
                MessageWindow messageWindow = new MessageWindow("請選擇一個項目!", MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }
            SelectedItem = (SearchResult.SelectedItem as Product);
            ConfirmButtonClicked = true;
            Close();
        }

        private bool SearchFilter(object item) {
            if( (((Product)item).Id.Contains(SearchText.Text) || SearchText.Text == string.Empty)
                && ( (!((IStockTaking)item).Status && (bool)IsStatusTrue.IsChecked) || !(bool)IsStatusTrue.IsChecked)
                && (!productsTakingCollection.Contains(item))
              ) return true;
            return false;
        }
        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchResult.Items.Filter = SearchFilter;
        }

        private void IsStatusTrue_Click(object sender, RoutedEventArgs e)
        {
            SearchResult.Items.Filter = SearchFilter;
        }
    }
}
