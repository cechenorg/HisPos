using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.IndexReserve;
using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.CommonProduct;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Data;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.SYSTEM_TAB.INDEX
{
    public class Index : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region Var

        private List<string> medPrepareStatusCollection;

        public List<string> MedPrepareStatusCollection
        {
            get => medPrepareStatusCollection;
            set
            {
                Set(() => MedPrepareStatusCollection, ref medPrepareStatusCollection, value);
            }
        }

        private List<string> productTypeCollection;

        public List<string> ProductTypeCollection
        {
            get => productTypeCollection;
            set
            {
                Set(() => ProductTypeCollection, ref productTypeCollection, value);
            }
        }

        private string medPrepareStatusSelectedItem = "未處理";

        public string MedPrepareStatusSelectedItem
        {
            get => medPrepareStatusSelectedItem;
            set
            {
                Set(() => MedPrepareStatusSelectedItem, ref medPrepareStatusSelectedItem, value);
                ReserveCollectionViewSource.Filter += Filter;
                SetPhoneCount();
            }
        }

        private string productTypeStatusSelectedItem = "藥品";

        public string ProductTypeStatusSelectedItem
        {
            get => productTypeStatusSelectedItem;
            set
            {
                Set(() => ProductTypeStatusSelectedItem, ref productTypeStatusSelectedItem, value);
                ProductCollectionViewSource.Filter += ProductFilter;
            }
        }

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

        private CollectionViewSource productCollectionViewSource;

        private CollectionViewSource ProductCollectionViewSource
        {
            get => productCollectionViewSource;
            set
            {
                Set(() => ProductCollectionViewSource, ref productCollectionViewSource, value);
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

        private ICollectionView productCollectionView;

        public ICollectionView ProductCollectionView
        {
            get => productCollectionView;
            private set
            {
                Set(() => ProductCollectionView, ref productCollectionView, value);
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

        private int productCount;

        public int ProductCount
        {
            get => productCount;
            set
            {
                Set(() => ProductCount, ref productCount, value);
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

        private IndexReserveDetail indexReserveDetailSelectedItem;

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

        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            private set
            {
                Set(() => IsBusy, ref isBusy, value);
            }
        }

        private string busyContent;

        public string BusyContent
        {
            get => busyContent;
            set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }

        private CommonProducts commonProductsCollection;

        public CommonProducts CommonProductsCollection
        {
            get => commonProductsCollection;
            set
            {
                Set(() => CommonProductsCollection, ref commonProductsCollection, value);
            }
        }

        private CommonProduct commonProductSelectedItem;

        public CommonProduct CommonProductSelectedItem
        {
            get => commonProductSelectedItem;
            set
            {
                Set(() => CommonProductSelectedItem, ref commonProductSelectedItem, value);
            }
        }

        #endregion Var

        #region Command

        public RelayCommand ReserveSearchCommand { get; set; }
        public RelayCommand ReserveMedicineSendCommand { get; set; }
        public RelayCommand ReserveMedicineBackCommand { get; set; }
        public RelayCommand ControlMedicineUsageCommand { get; set; }
        public RelayCommand IndexReserveSelectionChangedCommand { get; set; }
        public RelayCommand CommonMedStoreOrderCommand { get; set; }
        public RelayCommand StatusChangedCommand { get; set; }
        public RelayCommand MedPrepareChangedCommand { get; set; }
        public RelayCommand ShowCustomerDetailWindowCommand { get; set; }
        public RelayCommand CustomerDataSaveCommand { get; set; }
        public RelayCommand ShowCustomerPrescriptionChangedCommand { get; set; }
        public RelayCommand DataChangeCommand { get; set; }
        public RelayCommand ShowMedicineDetailCommand { get; set; }
        public RelayCommand PrintIndexReserveMedbagCommand { get; set; }
        public RelayCommand ShowReserveDetailCommand { get; set; }
        public RelayCommand ShowCommonProductDetailCommand { get; set; }
        public RelayCommand CommonProductGetDataCommand { get; set; }
        public RelayCommand ProductActionCommand { get; set; }

        #endregion Command

        public Index()
        {
            InitStatusstring();
            Messenger.Default.Register<NotificationMessage>("ReloadIndexReserves", ReloadIndexReserve);
            ReserveSearchCommand = new RelayCommand(ReserveSearchAction);
            IndexReserveSelectionChangedCommand = new RelayCommand(IndexReserveSelectionChangedAction);
            ReserveMedicineSendCommand = new RelayCommand(ReserveSendAction);
            CommonMedStoreOrderCommand = new RelayCommand(CommonMedStoreOrderAction);
            StatusChangedCommand = new RelayCommand(StatusChangedAction);
            MedPrepareChangedCommand = new RelayCommand(MedPrepareChangedAction);
            ShowCustomerDetailWindowCommand = new RelayCommand(ShowCustomerDetailWindowAction);
            CustomerDataSaveCommand = new RelayCommand(CustomerDataSaveAction);
            ShowCustomerPrescriptionChangedCommand = new RelayCommand(ShowCustomerPrescriptionChangedAction);
            ReserveMedicineBackCommand = new RelayCommand(ReserveMedicineBackAction);
            ControlMedicineUsageCommand = new RelayCommand(ControlMedicineUsageAction);
            DataChangeCommand = new RelayCommand(DataChangeAction);
            ShowMedicineDetailCommand = new RelayCommand(ShowMedicineDetailAction);
            ShowCommonProductDetailCommand = new RelayCommand(ShowCommonProductDetailAction);
            PrintIndexReserveMedbagCommand = new RelayCommand(PrintPackageAction);
            ShowReserveDetailCommand = new RelayCommand(ShowReserveDetailAction);
            CommonProductGetDataCommand = new RelayCommand(CommonProductGetDataAcion);
            ProductActionCommand = new RelayCommand(ProductAction);
            ReserveSearchAction();
            CommonProductGetDataAcion();
            ProductAction();
        }

        #region Action

        private void CommonProductGetDataAcion()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "取得低於安全量商品...";
                CommonProductsCollection = CommonProducts.GetData();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                ProductAction();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void ProductAction()
        {
            ProductCollectionViewSource = new CollectionViewSource { Source = CommonProductsCollection };
            ProductCollectionView = ProductCollectionViewSource.View;
            ProductCollectionViewSource.Filter += ProductFilter;
        }

        private void DataChangeAction()
        {
            IsDataChanged = true;
        }

        private void ReserveMedicineBackAction()
        {
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

        private void ControlMedicineUsageAction()
        {
            var controlMedUsageWindow = new ControlMedicineUsageWindow.ControlMedicineUsageWindow();
            controlMedUsageWindow.Show();
        }

        private void PrintPackageAction()
        {
            if (IndexReserveSelectedItem is null) return;

            ConfirmWindow confirmWindow = new ConfirmWindow("是否列印封包明細?", "封包明細列印");
            if ((bool)confirmWindow.DialogResult)
            {
                ReportViewer rptViewer = new ReportViewer();
                IndexReserveSelectedItem.GetIndexDetail();
                IndexReserveSelectedItem.SetReserveMedicinesSheetReportViewer(rptViewer);
                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    ((VM)MainWindow.Instance.DataContext).StartPrintReserve(rptViewer);
                });
            }
        }

        private void ShowCustomerPrescriptionChangedAction()
        {
            if (IndexReserveSelectedItem is null) return;
            CustomerPrescriptionChangedWindow.CustomerPrescriptionChangedWindow customerPrescriptionChangedWindow = new CustomerPrescriptionChangedWindow.CustomerPrescriptionChangedWindow(IndexReserveSelectedItem.CusId);
            ReserveSearchAction();
        }

        private void CustomerDataSaveAction()
        {
            CustomerData.Save();
        }

        private void SetPhoneCount()
        {
            int filteredCount = 0;
            foreach (var item in ReserveCollectionViewSource.View)
            {
                filteredCount++;
            }
            IndexReserveCount = filteredCount;
        }

        private void ShowCustomerDetailWindowAction()
        {
            if (IndexReserveSelectedItem is null) return;
            CustomerDetailWindow.CustomerDetailWindow customerDetailWindow = new CustomerDetailWindow.CustomerDetailWindow(IndexReserveSelectedItem.CusId);
        }

        private void InitStatusstring()
        {
            PhoneCallStatusString = new List<string>() { "未處理", "已聯絡", "電話未接" };
            MedPrepareStatusCollection = new List<string>() { "未處理", "已備藥", "不備藥" };
            ProductTypeCollection = new List<string>() { "藥品", "OTC" };
        }

        private void StatusChangedAction()
        {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveSelectedItem.SaveStatus();
            CustomerData = Customer.GetCustomerByCusId(IndexReserveSelectedItem.CusId);
        }

        private void MedPrepareChangedAction()
        {
            if (IndexReserveSelectedItem is null) return;
            if (IndexReserveSelectedItem.PrepareMedStatus == IndexPrepareMedType.UnPrepare)
                IndexReserveSelectedItem.SaveStatus();
        }

        private void CommonMedStoreOrderAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否將低於安全量之藥品傳送訂單至杏德?", "常備藥傳送");
            if ((bool)confirmWindow.DialogResult)
            {
                if (ProductTypeStatusSelectedItem == "藥品")
                {
                    DataTable table = StoreOrderDB.StoreOrderCommonMedicine();

                    if (table.Rows.Count > 0)
                    {
                        StoreOrder storeOrder = new PurchaseOrder(table.Rows[0]);
                        storeOrder.GetOrderProducts();

                        table = StoreOrderDB.SendStoreOrderToSingde(storeOrder);

                        if (table.Rows.Count > 0 && table.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                        {
                            StoreOrderDB.StoreOrderToWaiting(storeOrder.ID);
                            MessageWindow.ShowMessage("傳送成功!", MessageType.SUCCESS);
                        }
                        else
                        {
                            StoreOrderDB.RemoveStoreOrderByID(storeOrder.ID);
                            MessageWindow.ShowMessage("傳送失敗!", MessageType.ERROR);
                        }

                        CommonProductGetDataAcion();
                    }
                }
                else if (ProductTypeStatusSelectedItem == "OTC")
                {
                    DataTable table = StoreOrderDB.StoreOrderOTCMedicine();

                    if (table.Rows.Count > 0)
                    {
                        StoreOrder storeOrder = new PurchaseOrder(table.Rows[0]);
                        storeOrder.GetOrderProducts();

                        table = StoreOrderDB.SendOTCStoreOrderToSingde(storeOrder);

                        if (table.Rows.Count > 0 && table.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                        {
                            StoreOrderDB.StoreOrderToWaiting(storeOrder.ID);
                            MessageWindow.ShowMessage("傳送成功!", MessageType.SUCCESS);
                        }
                        else
                        {
                            StoreOrderDB.RemoveStoreOrderByID(storeOrder.ID);
                            MessageWindow.ShowMessage("傳送失敗!", MessageType.ERROR);
                        }
                        CommonProductGetDataAcion();
                    }
                }
            }
        }

        private void ReserveSendAction()
        {
            IndexReserves indexReserves = new IndexReserves();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                BusyContent = "計算慢箋傳送量...";
                indexReserves = CaculateReserveSendAmount();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
                if (indexReserves.Count == 0)
                {
                    MessageWindow.ShowMessage("未有備藥傳送處方", MessageType.WARNING);
                    ReserveSearchAction();
                    return;
                }
                ReserveSendConfirmWindow.ReserveSendConfirmWindow reserveSendConfirmWindow = new ReserveSendConfirmWindow.ReserveSendConfirmWindow(indexReserves);
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void IndexReserveSelectionChangedAction()
        {
            if (IndexReserveSelectedItem is null) return;
        }

        private void ReserveSearchAction()
        {
            IndexReserveCollection.GetDataByDate(StartDate, EndDate);
            ReserveCollectionViewSource = new CollectionViewSource { Source = IndexReserveCollection };
            ReserveCollectionView = ReserveCollectionViewSource.View;
            ReserveCollectionViewSource.Filter += Filter;
            SetPhoneCount();
        }

        private void ReloadIndexReserve(NotificationMessage msg)
        {
            if (msg.Notification.Equals("ReloadIndexReserves"))
                ReserveSearchAction();
        }

        private void ProductFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is CommonProduct src))
                e.Accepted = false;
            e.Accepted = false;
            CommonProduct commonProducts = ((CommonProduct)e.Item);
            if (commonProducts.TypeID == 2 && ProductTypeStatusSelectedItem == "OTC")
                e.Accepted = true;
            else if (( commonProducts.TypeID == 1 || commonProducts.TypeID == 3) && ProductTypeStatusSelectedItem == "藥品")
                e.Accepted = true;
        }

        private void Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is IndexReserve src))
                e.Accepted = false;
            e.Accepted = false;
            IndexReserve indexitem = ((IndexReserve)e.Item);
            if (indexitem.PrepareMedStatus == IndexPrepareMedType.UnPrepare && MedPrepareStatusSelectedItem == "不備藥")
                e.Accepted = true;
            else if (indexitem.PrepareMedStatus == IndexPrepareMedType.Prepare && MedPrepareStatusSelectedItem == "已備藥")
                e.Accepted = true;
            else if (indexitem.PhoneCallStatus == "F" && IsShowUnPhoneCall)
                e.Accepted = true;
            else if (indexitem.PhoneCallStatus == "N" && IsShowUnPhoneProcess)
                e.Accepted = true;
            else if (indexitem.IsExpensive && IsExpensive)
                e.Accepted = true;
            else if (MedPrepareStatusSelectedItem != "已備藥" && MedPrepareStatusSelectedItem != "不備藥" && !IsShowUnPhoneCall && !IsShowUnPhoneProcess && !IsExpensive && indexitem.PrepareMedStatus == IndexPrepareMedType.Unprocess)
                e.Accepted = true;
        }

        private IndexReserves CaculateReserveSendAmount()
        {
            IndexReserves indexReserves = new IndexReserves();
            foreach (var r in ReserveCollectionView)
            {
                indexReserves.Add((IndexReserve)r);
            }
            for (int i = 0; i < indexReserves.Count; i++)
            {
                if (indexReserves[i].IsNoSend || indexReserves[i].PrepareMedStatus == IndexPrepareMedType.Prepare)
                {
                    if (indexReserves[i].PrepareMedStatus == IndexPrepareMedType.Unprocess)
                    {
                        indexReserves[i].PrepareMedStatus = IndexPrepareMedType.UnPrepare;
                        indexReserves[i].SaveStatus();
                    }
                    indexReserves.Remove(indexReserves[i]);
                    i--;
                }
            }
            MainWindow.ServerConnection.CloseConnection();
            return indexReserves;
        }

        private void ShowMedicineDetailAction()
        {
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { IndexReserveDetailSelectedItem.ID, "0" }, "ShowProductDetail"));
        }

        private void ShowCommonProductDetailAction()
        {
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { CommonProductSelectedItem.ID, "0" }, "ShowProductDetail"));
        }

        private void ShowReserveDetailAction()
        {
            if (IndexReserveSelectedItem is null)
                return;
            PrescriptionService.ShowPrescriptionEditWindow(IndexReserveSelectedItem.Id, PrescriptionType.ChronicReserve);
        }

        #endregion Action
    }
}