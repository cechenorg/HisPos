using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ExportProductRecord;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.Service.ExportService;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.RecordControl
{
    public class ProductRecordDetailControlViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand SearchProductRecordCommand { get; set; }
        public RelayCommand ExportRecordCommand { get; set; }
        public RelayCommand<string> FilterRecordCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private DateTime? startDate = DateTime.Today.AddMonths(-3);
        private DateTime? endDate = DateTime.Today;

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
        #endregion

        public ProductRecordDetailControlViewModel()
        {
            RegisterCommand();
        }

        #region ----- Define Actions -----
        private void SearchProductRecordAction()
        {
            if (StartDate is null || EndDate is null)
            {
                MessageWindow.ShowMessage("日期格式錯誤", MessageType.ERROR);
                return;
            }

            MainWindow.ServerConnection.OpenConnection();
            InventoryRecordCollection = ProductInventoryRecords.GetInventoryRecordsByID(Medicine.ID, SelectedWareHouse.ID, (DateTime)StartDate, (DateTime)EndDate);

            InventoryRecordCollectionView = CollectionViewSource.GetDefaultView(InventoryRecordCollection);
            InventoryRecordCollectionView.Filter += RecordFilter;

            if (!InventoryRecordCollectionView.IsEmpty)
            {
                InventoryRecordCollectionView.MoveCurrentToLast();
                CurrentInventoryRecord = (ProductInventoryRecord)InventoryRecordCollectionView.CurrentItem;
            }

            ReloadStockDetail();
            ReloadProductGroupAndPrescription();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void ExportRecordAction()
        {
            Collection<object> tempCollection = new Collection<object>() { new List<object> { Medicine.ID, StartDate, EndDate, SelectedWareHouse.ID } };

            MainWindow.ServerConnection.OpenConnection();
            ExportExcelService service = new ExportExcelService(tempCollection, new ExportProductRecordTemplate());
            bool isSuccess = service.Export($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{Medicine.ID}商品歷程{DateTime.Now:yyyyMMdd-hhmmss}.xlsx");
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
        #endregion

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
        #endregion
    }
}
