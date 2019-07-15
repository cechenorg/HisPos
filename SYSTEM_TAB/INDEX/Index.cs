using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.IndexReserve;
using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using His_Pos.NewClass.StoreOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;
using System.Data;
using His_Pos.NewClass.Product;
using System.Linq;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.INDEX.ReserveSendConfirmWindow;

namespace His_Pos.SYSTEM_TAB.INDEX
{
    class Index : TabBase {
        public override TabBase getTab() {
            return this;
        }
        #region Var
        private CollectionViewSource reserveCollectionViewSource;
        private CollectionViewSource ReserveCollectionViewSource
        {
            get => reserveCollectionViewSource;
            set
            {
                Set(() => ReserveCollectionViewSource, ref reserveCollectionViewSource, value);
                SetPhoneCount();
            }
        }

        private ICollectionView reserveCollectionView;
        public ICollectionView ReserveCollectionView
        {
            get => reserveCollectionView;
            private set
            {
                Set(() => ReserveCollectionView, ref reserveCollectionView, value); 
            }
        }
        private Inventorys inventoryCollection;
        public Inventorys InventoryCollection
        {
            get => inventoryCollection;
            set
            {
                Set(() => InventoryCollection, ref inventoryCollection, value);
            }
        }
        private bool isDataChanged;
        public bool IsDataChanged
        {
            get => isDataChanged;
            set
            {
                Set(() => IsDataChanged, ref isDataChanged, value);
            }
        }
        private int indexReserveCount;  
        public int IndexReserveCount
        {
            get => indexReserveCount;
            set
            {
                Set(() => IndexReserveCount, ref indexReserveCount, value); 
            }
        }
        private bool isShowUnPrepareReserve = false;
        public bool IsShowUnPrepareReserve
        {
            get => isShowUnPrepareReserve;
            set
            {
                Set(() => IsShowUnPrepareReserve, ref isShowUnPrepareReserve, value);
                ReserveCollectionViewSource.Filter += Filter;
                SetPhoneCount();
            }
        }
        private bool isShowPrepareReserve = false;
        public bool IsShowPrepareReserve
        {
            get => isShowPrepareReserve;
            set
            {
                Set(() => IsShowPrepareReserve, ref isShowPrepareReserve, value);
                ReserveCollectionViewSource.Filter += Filter;
                SetPhoneCount();
            }
        }
        
        private bool isShowUnPhoneCall = false;
        public bool IsShowUnPhoneCall
        {
            get => isShowUnPhoneCall;
            set
            { 
                Set(() => IsShowUnPhoneCall, ref isShowUnPhoneCall, value);
                ReserveCollectionViewSource.Filter += Filter;
                SetPhoneCount();
            }
        }
        private bool isShowUnPhoneProcess = false;
        public bool IsShowUnPhoneProcess
        {
            get => isShowUnPhoneProcess;
            set
            {
                Set(() => IsShowUnPhoneProcess, ref isShowUnPhoneProcess, value);
                ReserveCollectionViewSource.Filter += Filter;
                SetPhoneCount();
            }
        }
        private bool isExpensive = false;
        public bool IsExpensive
        {
            get => isExpensive;
            set
            {
                Set(() => IsExpensive, ref isExpensive, value);
                ReserveCollectionViewSource.Filter += Filter;
                SetPhoneCount();
            }
        }
        
        private DateTime startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private DateTime endDate = DateTime.Today.AddDays(5);
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }
        private IndexReserves indexReserveCollection = new IndexReserves();
        public IndexReserves IndexReserveCollection
        {
            get => indexReserveCollection;
            set
            {
                Set(() => IndexReserveCollection, ref indexReserveCollection, value);
            }
        }
        private IndexReserve indexReserveSelectedItem;
        public IndexReserve IndexReserveSelectedItem
        {
            get => indexReserveSelectedItem;
            set
            {
                Set(() => IndexReserveSelectedItem, ref indexReserveSelectedItem, value);
                if (IndexReserveSelectedItem is null) return;
                IndexReserveDetailCollection.GetDataById(IndexReserveSelectedItem.Id);
                CustomerData = Customer.GetCustomerByCusId(IndexReserveSelectedItem.CusId);
            }
        }
        private IndexReserveDetails indexReserveDetailCollection = new IndexReserveDetails();
        public IndexReserveDetails IndexReserveDetailCollection
        {
            get => indexReserveDetailCollection;
            set
            {
                Set(() => IndexReserveDetailCollection, ref indexReserveDetailCollection, value);
            }
        }
        private IndexReserveDetail indexReserveDetailSelectedItem ;
        public IndexReserveDetail IndexReserveDetailSelectedItem
        {
            get => indexReserveDetailSelectedItem;
            set
            {
                Set(() => IndexReserveDetailSelectedItem, ref indexReserveDetailSelectedItem, value);
            }
        }
        private List<string> phoneCallStatusString;
        public List<string> PhoneCallStatusString
        {
            get => phoneCallStatusString;
            set
            {
                Set(() => PhoneCallStatusString, ref phoneCallStatusString, value);
            }
        }
        private List<string> prepareStatusString;
        public List<string> PrepareStatusString
        {
            get => prepareStatusString;
            set
            {
                Set(() => PrepareStatusString, ref prepareStatusString, value);
            }
        }
         
        private Customer customerData = new Customer();
        public Customer CustomerData
        {
            get => customerData;
            set
            {
                Set(() => CustomerData, ref customerData, value);
                IsDataChanged = false;
            }
        }
        #endregion
        #region Command
        public RelayCommand ReserveSearchCommand { get; set; }
        public RelayCommand ReserveMedicineSendCommand { get; set; }
        public RelayCommand ReserveMedicineBackCommand { get; set; }
        public RelayCommand IndexReserveSelectionChangedCommand { get; set; }
        public RelayCommand CommonMedStoreOrderCommand { get; set; }
        public RelayCommand StatusChangedCommand { get; set; }
        public RelayCommand ShowCustomerDetailWindowCommand { get; set; }
        public RelayCommand CustomerDataSaveCommand { get; set; }
        public RelayCommand ShowCustomerPrescriptionChangedCommand { get; set; }
        public RelayCommand DataChangeCommand { get; set; }
        public RelayCommand ShowMedicineDetailCommand { get; set; }

        #endregion
        public Index() {
            InitStatusstring();
            ReserveSearchCommand = new RelayCommand(ReserveSearchAction);
            IndexReserveSelectionChangedCommand = new RelayCommand(IndexReserveSelectionChangedAction);
            ReserveMedicineSendCommand = new RelayCommand(ReserveSendAction);
            CommonMedStoreOrderCommand = new RelayCommand(CommonMedStoreOrderAction);
            StatusChangedCommand = new RelayCommand(StatusChangedAction);
            ShowCustomerDetailWindowCommand = new RelayCommand(ShowCustomerDetailWindowAction);
            CustomerDataSaveCommand = new RelayCommand(CustomerDataSaveAction);
            ShowCustomerPrescriptionChangedCommand = new RelayCommand(ShowCustomerPrescriptionChangedAction);
            ReserveMedicineBackCommand = new RelayCommand(ReserveMedicineBackAction);
            DataChangeCommand = new RelayCommand(DataChangeAction);
            ShowMedicineDetailCommand = new RelayCommand(ShowMedicineDetailAction);
        }
        #region Action
        private void DataChangeAction() {
            IsDataChanged = true;
        }
        private void ReserveMedicineBackAction() {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否將未設定為常備藥且90天內慢箋未使用之藥品退貨?", "慢箋退貨");
            if ((bool)confirmWindow.DialogResult)
            {
                DataTable table = StoreOrderDB.StoreOrderReturnReserve();
                if (table.Rows.Count > 0)
                {
                    MessageWindow.ShowMessage("已轉出退貨單 將跳轉至進退貨管理", MessageType.SUCCESS);
                    ProductPurchaseReturnViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).ProductPurchaseReturn;
                    Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, table.Rows[0].Field<string>("StoOrdID"), ""));
                }
            }
           
        }
        private void ShowCustomerPrescriptionChangedAction() {
            if (IndexReserveSelectedItem is null) return;
            CustomerPrescriptionChangedWindow.CustomerPrescriptionChangedWindow customerPrescriptionChangedWindow = new CustomerPrescriptionChangedWindow.CustomerPrescriptionChangedWindow(IndexReserveSelectedItem.CusId); 
        }
        private void CustomerDataSaveAction() {
            CustomerData.Save();
        }
        private void SetPhoneCount() {
            int filteredCount = 0;
            foreach (var item in ReserveCollectionViewSource.View)
            {
                filteredCount++;
            }
            IndexReserveCount = filteredCount;
        }
        private void ShowCustomerDetailWindowAction() {
            if (IndexReserveSelectedItem is null) return; 
            CustomerDetailWindow.CustomerDetailWindow customerDetailWindow = new CustomerDetailWindow.CustomerDetailWindow(IndexReserveSelectedItem.CusId); 
        }
        private void InitStatusstring() {
            PhoneCallStatusString = new List<string>() { "未處理", "已聯絡", "電話未接" };
            PrepareStatusString = new List<string>() { "未處理", "備藥", "不備藥" };
        }
        private void StatusChangedAction() {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveSelectedItem.SaveStatus();
            CustomerData = Customer.GetCustomerByCusId(IndexReserveSelectedItem.CusId);
        }
        private void CommonMedStoreOrderAction() {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否將已設定為常備藥且低於安全量之藥品產生採購製表至基準量?","常備藥轉採購");
            if ((bool)confirmWindow.DialogResult)
            {
                StoreOrderDB.StoreOrderCommonMedicine();
                MessageWindow.ShowMessage("已轉出採購單 請至進退貨管理確認", MessageType.SUCCESS);
            }
        }
      
        private void ReserveSendAction() { 
            IndexReserves indexReserves = CaculateReserveSendAmount();
            if (indexReserves.Count == 0) {
                MessageWindow.ShowMessage("未有備藥傳送處方",MessageType.WARNING);
                ReserveSearchAction();
                return;
            } 
            ReserveSendConfirmWindow.ReserveSendConfirmWindow reserveSendConfirmWindow = new ReserveSendConfirmWindow.ReserveSendConfirmWindow(indexReserves);
           //ProductPurchaseReturnViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).ProductPurchaseReturn;
           //Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, "", ""));
           //ReserveSearchAction();
        }
          
        private void IndexReserveSelectionChangedAction() {
            if (IndexReserveSelectedItem is null) return;
            
        }
        private void ReserveSearchAction() {
            IndexReserveCollection.GetDataByDate(StartDate, EndDate);
            ReserveCollectionViewSource = new CollectionViewSource { Source = IndexReserveCollection };
            ReserveCollectionView = ReserveCollectionViewSource.View;
            ReserveCollectionViewSource.Filter += Filter;
            SetPhoneCount();
        }
        private void Filter(object sender, FilterEventArgs e) {
            if (e.Item is null) return;
            if (!(e.Item is IndexReserve src))
                e.Accepted = false; 
            e.Accepted = false; 
            IndexReserve indexitem = ((IndexReserve)e.Item);
            if (indexitem.PrepareMedStatus == "不備藥" && IsShowUnPrepareReserve)
                e.Accepted = true;
            else if(indexitem.PrepareMedStatus == "備藥" && IsShowPrepareReserve)
                e.Accepted = true; 
            else if (indexitem.PhoneCallStatus == "F" && IsShowUnPhoneCall)
                e.Accepted = true;
            else if (indexitem.PhoneCallStatus == "N" && IsShowUnPhoneProcess)
                e.Accepted = true;
            else if(indexitem.IsExpensive && IsExpensive)
                e.Accepted = true;
            else if(!IsShowPrepareReserve && !IsShowUnPrepareReserve && !IsShowUnPhoneCall && !IsShowUnPhoneProcess && !IsExpensive && indexitem.PrepareMedStatus == "未處理")
                e.Accepted = true;
            
        }
        private IndexReserves CaculateReserveSendAmount() {
            IndexReserves indexReserves = new IndexReserves(); 
            foreach (var r in ReserveCollectionView) {
                indexReserves.Add((IndexReserve)r);
            }
            List<string> MedicineIds = new List<string>();
            foreach (var indexReserve in indexReserves) {
                foreach (var med in indexReserve.IndexReserveDetailCollection) {
                    MedicineIds.Add(med.ID);
                }
            }
            MainWindow.ServerConnection.OpenConnection();
            InventoryCollection = Inventorys.GetAllInventoryByProIDs(MedicineIds);

            for (int i = 0; i < indexReserves.Count; i++) {
                if (indexReserves[i].PrepareMedStatus == "備藥" && !indexReserves[i].IsSend)
                {
                    indexReserves[i].GetIndexDetail();
                    for (int j = 0; j < indexReserves[i].IndexReserveDetailCollection.Count; j++)
                    {
                        var pro = indexReserves[i].IndexReserveDetailCollection[j];
                        if (InventoryCollection.Count(inv => inv.InvID.ToString() == pro.InvID) == 0)
                            pro.SendAmount = Convert.ToInt32(pro.Amount);
                        else
                        {
                            var target = InventoryCollection.Single(inv => inv.InvID.ToString() == pro.InvID);
                            pro.SendAmount = target.OnTheFrame - pro.Amount > 0 ? 0 : pro.Amount - target.OnTheFrame;
                        } 
                    }
                }
                else {
                    indexReserves.Remove(indexReserves[i]);
                    i--;
                } 
            }
            MainWindow.ServerConnection.CloseConnection();
            for (int i = 0; i < indexReserves.Count; i++) {
                int sameCount = 0;
                int zeroSendCount = 0;
                foreach (var s in indexReserves[i].IndexReserveDetailCollection) {
                    if (s.SendAmount == s.Amount)
                        sameCount++;
                    else if (s.SendAmount == 0)
                        zeroSendCount++; 
                }
                if (sameCount == 0 && zeroSendCount == indexReserves[i].IndexReserveDetailCollection.Count)
                    indexReserves[i].PrepareMedType = ReserveSendType.AllPrepare;
                else if(sameCount == indexReserves[i].IndexReserveDetailCollection.Count)
                    indexReserves[i].PrepareMedType = ReserveSendType.AllSend;
                else
                    indexReserves[i].PrepareMedType = ReserveSendType.CoPrepare;
            }

            return indexReserves;
        }
        private void ShowMedicineDetailAction() {
          ProductDetailWindow.ShowProductDetailWindow();
          Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { IndexReserveDetailSelectedItem.ID, "0" }, "ShowProductDetail"));
        }
        #endregion
    }
}
