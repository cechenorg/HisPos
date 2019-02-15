using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchase;

namespace His_Pos.SYSTEM_TAB.INDEX
{
    /// <summary>
    /// IndexView.xaml 的互動邏輯
    /// </summary>
    public partial class IndexView : UserControl, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged; 
        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class DailtChronicPhoneCall {

            public DailtChronicPhoneCall(DataRow row) {
                DecMasId = row["HISDECMAS_ID"].ToString();
                CusId = row["CUS_ID"].ToString();
                CusName = row["CUS_NAME"].ToString();
                Phone = row["CUS_TEL"].ToString();
                TreatDate = Convert.ToDateTime(row["HISDECMAS_TREATDATE"]).AddYears(-1911).ToString("yyy-MM-dd");
                PrescriptDate = Convert.ToDateTime(row["HISDECMAS_PRESCRIPTIONDATE"]).AddYears(-1911).ToString("yyy-MM-dd");  
                DivInsName = row["DIVINS_NAME"].ToString();
                Status = row["STATUS"].ToString() ;
            }
            public string DecMasId { get; set; }
            public string CusId { get; set; }
            public string CusName { get; set; }
            public string Phone { get; set; }
            public string DivInsName { get; set; }
            public string TreatDate { get; set; }
            public string PrescriptDate { get; set; }
            public string Status { get; set; }
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
                DemandAmount = row["NEEDAMOUNT"].ToString();
                PurchaseAmount = Convert.ToInt32(row["ORDERAMOUNT"].ToString()) > 0 ? row["ORDERAMOUNT"].ToString()  : "0";
                ReturnAmount = Convert.ToInt32(row["ORDERAMOUNT"].ToString()) < 0 ? (Convert.ToInt32(row["ORDERAMOUNT"].ToString()) * -1).ToString() : "0";
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
        private ObservableCollection<DailtChronicPhoneCall> dailtChronicPhoneCallCollection;
        public ObservableCollection<DailtChronicPhoneCall> DailtChronicPhoneCallCollection
        {
            get => dailtChronicPhoneCallCollection;
            set
            {
                dailtChronicPhoneCallCollection = value;
                NotifyPropertyChanged("DailtChronicPhoneCallCollection");
            }
        }

        private bool isUnPhone = true;
        public bool IsUnPhone {
            get => isUnPhone;
            set
            {
                isUnPhone = value;
                NotifyPropertyChanged("IsUnPhone");
            } 
        }
        private bool isGetMed = true;
        public bool IsGetMed
        {
            get => isGetMed;
            set
            {
                isGetMed = value;
                NotifyPropertyChanged("IsGetMed");
            }
        }
        private bool isNoGetMed = true;
        public bool IsNoGetMed
        {
            get => isNoGetMed;
            set
            {
                isNoGetMed = value;
                NotifyPropertyChanged("IsNoGetMed");
            }
        }
        private bool isNoGetPhone = true;
        public bool IsNoGetPhone
        {
            get => isNoGetPhone;
            set
            {
                isNoGetPhone = value;
                NotifyPropertyChanged("IsNoGetPhone");
            }
        }
        public static IndexView Instance;
        public IndexView()
        {
            InitializeComponent();
           // Date.Content = DateTime.Today.ToString("yyyy/MM/dd");
            ///ChronicDb.DailyPredictChronic();
            InitData();
            DataContext = this;
            Instance = this;
        }

        public void InitData() {
            try
            {
                IsGetMed = false;
                IsNoGetMed = false;
               ///ProductListCollection = ProductDb.DailyPurchaseReturn();
               ///DailyTakeChronicListCollection = ChronicDb.DailyTakeChronic();
               ///DailtChronicPhoneCallCollection = ChronicDb.DailyChronicPhoneCall();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage("InitData()", MessageType.ERROR);
                
                return;
            }
        }

        private void TransferToStoord_Click(object sender, RoutedEventArgs e) {
            if (ProductListCollection.Where(pro => pro.PurchaseAmount != "0").ToList().Count != 0)
                ;/// StoreOrderDb.AddDailyOrder(StoreOrderCategory.PURCHASE, ProductListCollection.Where(pro => pro.PurchaseAmount != "0").ToList());
            if (ProductListCollection.Where(pro => pro.ReturnAmount != "0").ToList().Count != 0)
                ;/// StoreOrderDb.AddDailyOrder(StoreOrderCategory.RETURN, ProductListCollection.Where(pro => pro.ReturnAmount != "0").ToList());
            ///ChronicDb.UpdateDailyChronic();
            MessageWindow.ShowMessage("已轉出進退貨單",MessageType.SUCCESS);
             
            MainWindow.Instance.AddNewTab("處理單管理");
            if(ProductPurchaseView.Instance != null)
                ProductPurchaseView.Instance.InitData();
            InitData();
        }
         
        private void ShowPrescription_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            //var selectedItem = (sender as DataGridRow).Item;
            //MainWindow.Instance.AddNewTab("處方登錄");
            //PrescriptionDec2View.IndexViewDecMasId = ((DailyTakeChronicList)selectedItem).DecMasId;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ChironicPhoneCall.SelectedItem is null) return;
            DailtChronicPhoneCall a =  (DailtChronicPhoneCall)ChironicPhoneCall.SelectedItem;
            ///ChronicDb.UpdateChronicPhoneCall(a.DecMasId,a.Status);
        }

        private void PhoneCall_Checked(object sender, RoutedEventArgs e) {
            if (DailtChronicPhoneCallCollection is null) return;
            ChironicPhoneCall.Items.Filter = ((o) => {
                string status = ((DailtChronicPhoneCall)o).Status;
                switch (status)
                {
                    case "未聯絡":
                        if (IsUnPhone)
                            return true;
                        else
                            return false;
                    case "確定領藥":
                        if (IsGetMed)
                            return true;
                        else
                            return false;
                    case "確定不領":
                        if (IsNoGetMed)
                            return true;
                        else
                            return false;
                    case "電話未接":
                        if (IsNoGetPhone)
                            return true;
                        else
                            return false;
                    default:
                        return true;
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
