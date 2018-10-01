using His_Pos.Class;
using His_Pos.Class.Product;
using His_Pos.Class.StoreOrder;
using His_Pos.H1_DECLARE.PrescriptionDec2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace His_Pos.IndexView
{
    /// <summary>
    /// IndexView.xaml 的互動邏輯
    /// </summary>
    public partial class IndexView : UserControl, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged; 
        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class DailyTakeChronicList {
            public DailyTakeChronicList(DataRow row) {
                DecMasId = row["HISDECMAS_ID"].ToString();
                TakeDays = row["DAYS"].ToString();
                CusName = row["CUS_NAME"].ToString();
                CusTel = row["CUS_TEL"].ToString();
                DivInsName = row["DIVINS_NAME"].ToString();
            }
            public string DecMasId { get; set; }
            public string TakeDays{ get; set; }
            public string CusName { get; set; }
            public string CusTel { get; set; }
            public string DivInsName { get; set; }
        }
        public class ProductPurchaseList {
            public ProductPurchaseList(DataRow row) {
                proId = row["HISMED_ID"].ToString();
                proName = row["PRO_NAME"].ToString();
                Stock = row["PRO_INVENTORY"].ToString();
                DemandAmount = row["TOTALAMOUNT"].ToString();
                PurchaseAmount = Convert.ToInt32(row["NEEDAMOUNT"].ToString()) > 0 ? row["NEEDAMOUNT"].ToString()  : "0";
                ReturnAmount = Convert.ToInt32(row["NEEDAMOUNT"].ToString()) < 0 ? (Convert.ToInt32(row["NEEDAMOUNT"].ToString()) * -1).ToString() : "0";
            }

            public string proId { get; set; }
            public string proName { get; set; }
            public string Stock { get; set; }
            public string DemandAmount { get; set; }
            public string PurchaseAmount { get; set; }
            public string ReturnAmount { get; set; }
        } 
        private ObservableCollection<ProductPurchaseList> productListCollection;
        public ObservableCollection<ProductPurchaseList> ProductListCollection
        {
            get => productListCollection;
            set
            {
                productListCollection = value;
                NotifyPropertyChanged("ProductListCollection");
            }
        }
        private ObservableCollection<DailyTakeChronicList> dailyTakeChronicListCollection;
        public ObservableCollection<DailyTakeChronicList> DailyTakeChronicListCollection
        {
            get => dailyTakeChronicListCollection;
            set
            {
                dailyTakeChronicListCollection = value;
                NotifyPropertyChanged("DailyTakeChronicListCollection");
            }
        }
        
        public IndexView()
        {
            InitializeComponent();
            Date.Content = DateTime.Today.ToString("yyyy/MM/dd");
            ChronicDb.CaculateChironic();
            InitData();
            DataContext = this;
        }

        public void InitData() { 
            ProductListCollection = ProductDb.DailyPurchaseReturn();
            DailyTakeChronicListCollection = ChronicDb.DailyTakeChronic();
        }
       

        private void TransferToStoord_Click(object sender, RoutedEventArgs e) {
            if(ProductListCollection.Where(pro => pro.PurchaseAmount != "0").ToList().Count != 0)
                StoreOrderDb.AddDailyOrder(StoreOrderCategory.PURCHASE, ProductListCollection.Where(pro => pro.PurchaseAmount != "0").ToList());
            if(ProductListCollection.Where(pro => pro.ReturnAmount != "0").ToList().Count != 0)
                StoreOrderDb.AddDailyOrder(StoreOrderCategory.RETURN, ProductListCollection.Where(pro => pro.ReturnAmount != "0").ToList());
            ChronicDb.UpdateDailyChronic();
            MessageWindow messageWindow = new MessageWindow("已轉出進退貨單",MessageType.SUCCESS);
            messageWindow.ShowDialog();
            MainWindow.Instance.AddNewTab("處理單管理");
            InitData();
        }

     

        private void ShowPrescription_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var selectedItem = (sender as DataGridRow).Item;
            MainWindow.Instance.AddNewTab("處方登錄");
            PrescriptionDec2View.IndexViewDecMasId = ((DailyTakeChronicList)selectedItem).DecMasId;
        }
    }
}
