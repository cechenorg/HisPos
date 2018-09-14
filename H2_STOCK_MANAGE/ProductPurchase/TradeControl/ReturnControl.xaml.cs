using System;
using System.Collections.Generic;
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
using His_Pos.Class;
using His_Pos.Class.StoreOrder;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl
{
    /// <summary>
    /// ReturnControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnControl : UserControl, INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public ReturnControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        internal void SetDataContext(StoreOrder storeOrderData)
        {
            
        }

        internal void ClearControl()
        {

        }

        #region ----- Paging Functions -----
        private void PreparePaging(PagingType type)
        {
            if (storeOrderData.Products.Count == 0)
                TotalPage = 1;
            else if (StoreOrderData.Type == OrderType.UNPROCESSING)
                TotalPage = (storeOrderData.Products.Count / PRODUCT_PER_PAGE) + ((storeOrderData.Products.Count % PRODUCT_PER_PAGE == 0) ? 0 : 1);
            else if (StoreOrderData.Type == OrderType.PROCESSING)
                TotalPage = (storeOrderData.Products.Count / (PRODUCT_PER_PAGE + 1)) + ((storeOrderData.Products.Count % (PRODUCT_PER_PAGE + 1) == 0) ? 0 : 1);

            switch (type)
            {
                case PagingType.INIT:
                    CurrentPage = 1;
                    break;
                case PagingType.DEL:
                    if (CurrentDataGrid.Items.Count == 1)
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
            if (StoreOrderData.type == OrderType.PROCESSING)
                CurrentDataGrid.ItemsSource = storeOrderData.Products.Skip((PRODUCT_PER_PAGE + 1) * (currentPage - 1)).Take(PRODUCT_PER_PAGE + 1).ToList();
            else
                CurrentDataGrid.ItemsSource = storeOrderData.Products.Skip(PRODUCT_PER_PAGE * (currentPage - 1)).Take(PRODUCT_PER_PAGE).ToList();
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
        }

        #endregion
    }
}
