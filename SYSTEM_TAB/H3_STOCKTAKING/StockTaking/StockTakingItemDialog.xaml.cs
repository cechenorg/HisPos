using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Person;
using His_Pos.FunctionWindow;
using His_Pos.Interface;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking
{
    /// <summary>
    /// StockTakingItemDialog.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingItemDialog : Window,INotifyPropertyChanged
    {
        public Product SelectedItem;
        public string SelectedUser;
        public bool ConfirmButtonClicked = false;
        public ObservableCollection<Product> productsTakingCollection;
        private ObservableCollection<Person> UserAutoCompleteCollection;
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
            ///UserAutoCompleteCollection = PersonDb.GetUserCollection();
            TakingEmp.ItemsSource = UserAutoCompleteCollection;
            TakingEmp.ItemFilter = UserFilter;
        }

        public AutoCompleteFilterPredicate<object> UserFilter
        {
            get
            {
                return (searchText, obj) =>
                    ((obj as Person).Id is null) ? true : (obj as Person).Id.Contains(searchText)
                    || (obj as Person).Name.Contains(searchText);
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (SearchResult.SelectedItem is null)
            {
                MessageWindow.ShowMessage("請選擇一個項目!", MessageType.ERROR, true);
                
                return;
            }
            SelectedItem = (SearchResult.SelectedItem as Product);
            SelectedUser = (TakingEmp.SelectedItem != null)? (TakingEmp.SelectedItem as Person).Id : "";
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
