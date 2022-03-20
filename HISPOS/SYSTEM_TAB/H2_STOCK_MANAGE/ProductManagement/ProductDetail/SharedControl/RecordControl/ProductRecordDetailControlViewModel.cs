using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ExportProductRecord;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.Service.ExportService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.RecordControl
{
    public class ProductRecordDetailControlViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand SearchProductRecordCommand { get; set; }
        public RelayCommand ExportRecordCommand { get; set; }
        public RelayCommand<string> FilterRecordCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private string productID;
        private string wareHouseID;
        private DateTime? startDate = DateTime.Today.AddMonths(-12);
        private DateTime? endDate = DateTime.Today;
        private ProductInventoryRecords inventoryRecordCollection;
        private ProductInventoryRecord currentInventoryRecord;
        private ICollectionView inventoryRecordCollectionView;
        private ProductInventoryRecordType filterType = ProductInventoryRecordType.All;

        public DateTime? StartDate
        {
            get { return startDate; }
            set { Set(() => StartDate, ref startDate, value); }
        }

        public DateTime? EndDate
        {
            get { return endDate; }
            set { Set(() => EndDate, ref endDate, value); }
        }

        public ProductInventoryRecords InventoryRecordCollection
        {
            get { return inventoryRecordCollection; }
            set { Set(() => InventoryRecordCollection, ref inventoryRecordCollection, value); }
        }

        public ProductInventoryRecord CurrentInventoryRecord
        {
            get { return currentInventoryRecord; }
            set { Set(() => CurrentInventoryRecord, ref currentInventoryRecord, value); }
        }

        public ICollectionView InventoryRecordCollectionView
        {
            get => inventoryRecordCollectionView;
            set { Set(() => InventoryRecordCollectionView, ref inventoryRecordCollectionView, value); }
        }

        #endregion ----- Define Variables -----

        public ProductRecordDetailControlViewModel()
        {
            RegisterCommand();
        }

        #region ----- Define Actions -----

        private void SearchProductRecordAction()
        {
            Messenger.Default.Send(new NotificationMessage<string>(this, productID, "RELOAD"));
        }

        private void ExportRecordAction()
        {
            Collection<object> tempCollection = new Collection<object>() { new List<object> { productID, StartDate, EndDate, wareHouseID } };

            MainWindow.ServerConnection.OpenConnection();
            ExportExcelService service = new ExportExcelService(tempCollection, new ExportProductRecordTemplate());
            bool isSuccess = service.Export($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{productID}商品歷程{DateTime.Now:yyyyMMdd-hhmmss}.xlsx");
            MainWindow.ServerConnection.CloseConnection();

            if (isSuccess)
                MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
            else
                MessageWindow.ShowMessage("匯出失敗 請稍後再試", MessageType.ERROR);
        }

        private void FilterRecordAction(string filterCondition)
        {
            if (filterCondition != null)
                filterType = (ProductInventoryRecordType)int.Parse(filterCondition);

            InventoryRecordCollectionView.Filter += RecordFilter;

            if (!InventoryRecordCollectionView.IsEmpty)
            {
                InventoryRecordCollectionView.MoveCurrentToLast();
                CurrentInventoryRecord = (ProductInventoryRecord)InventoryRecordCollectionView.CurrentItem;
            }
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommand()
        {
            SearchProductRecordCommand = new RelayCommand(SearchProductRecordAction);
            FilterRecordCommand = new RelayCommand<string>(FilterRecordAction);
            ExportRecordCommand = new RelayCommand(ExportRecordAction);
        }

        private bool RecordFilter(object record)
        {
            if (ProductInventoryRecordType.All == filterType)
                return true;
            else
                return filterType == ((ProductInventoryRecord)record).Type;
        }

        private void SearchRecord()
        {
            if (StartDate is null || EndDate is null)
            {
                MessageWindow.ShowMessage("日期格式錯誤", MessageType.ERROR);
                return;
            }

            MainWindow.ServerConnection.OpenConnection();
            InventoryRecordCollection = ProductInventoryRecords.GetInventoryRecordsByID(productID, wareHouseID, (DateTime)StartDate, (DateTime)EndDate);

            InventoryRecordCollectionView = CollectionViewSource.GetDefaultView(InventoryRecordCollection);
            InventoryRecordCollectionView.Filter += RecordFilter;

            if (!InventoryRecordCollectionView.IsEmpty)
            {
                InventoryRecordCollectionView.MoveCurrentToLast();
                CurrentInventoryRecord = (ProductInventoryRecord)InventoryRecordCollectionView.CurrentItem;
            }

            MainWindow.ServerConnection.CloseConnection();
        }

        internal void ReloadData(string proID, string wareID)
        {
            productID = proID;
            wareHouseID = wareID;

            SearchRecord();
        }

        #endregion ----- Define Functions -----
    }
}