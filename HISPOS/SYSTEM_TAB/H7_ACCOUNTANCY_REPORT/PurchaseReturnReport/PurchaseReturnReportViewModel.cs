﻿using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.StoreOrder.Report;
using His_Pos.NewClass.WareHouse;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.PurchaseReturnReport
{
    public class PurchaseReturnReportViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ExportCSVCommand { get; set; }
        public RelayCommand ExportCSVTotalCommand { get; set; }
        public RelayCommand ExportCSVDetailTotalCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        #region ///// Search Variables /////

        private DateTime? startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private DateTime? endDate = DateTime.Today;
        //private string manufactoryName = "";
        private WareHouse selectedWareHouse;
        public Manufactories ManufacturerCollection { get; set; }
        public Manufactory Manufacturer { get; set; }
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

        //public string ManufactoryName
        //{
        //    get { return manufactoryName; }
        //    set { Set(() => ManufactoryName, ref manufactoryName, value); }
        //}

        public WareHouse SelectedWareHouse
        {
            get { return selectedWareHouse; }
            set { Set(() => SelectedWareHouse, ref selectedWareHouse, value); }
        }

        #endregion ///// Search Variables /////

        private ManufactoryOrders manufactoryOrderCollection;
        private ManufactoryOrder currentManufactoryOrder;

        private DateTime SearchStartDate { get; set; }
        private DateTime SearchEndDate { get; set; }

        public ManufactoryOrders ManufactoryOrderCollection
        {
            get { return manufactoryOrderCollection; }
            set
            {
                Set(() => ManufactoryOrderCollection, ref manufactoryOrderCollection, value);
                RaisePropertyChanged(nameof(ManufactoryOrdersPurchaseTotal));
                RaisePropertyChanged(nameof(ManufactoryOrdersReturnTotal));
                RaisePropertyChanged(nameof(ManufactoryOrdersPurchaseCount));
                RaisePropertyChanged(nameof(ManufactoryOrdersReturnCount));
            }
        }

        public ManufactoryOrder CurrentManufactoryOrder
        {
            get { return currentManufactoryOrder; }
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                value?.GetOrderDetails(SearchStartDate, SearchEndDate, SelectedWareHouse.ID);
                MainWindow.ServerConnection.CloseConnection();
                Set(() => CurrentManufactoryOrder, ref currentManufactoryOrder, value);

                RaisePropertyChanged("HasManufactory");
            }
        }

        public WareHouses WareHouseCollection { get; set; }
        public int HasManufactory 
        { get 
            { return IsSingdeManufactory(); } 
        }
        private int IsSingdeManufactory()
        {
            if(CurrentManufactoryOrder == null)
            {
                return 0;
            }
            else if(CurrentManufactoryOrder != null && CurrentManufactoryOrder.ManufactoryID != 0)
            {
                return 1;
            }
            else if (CurrentManufactoryOrder != null && CurrentManufactoryOrder.ManufactoryID == 0)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
        public double ManufactoryOrdersPurchaseCount { get { return (ManufactoryOrderCollection is null) ? 0 : ManufactoryOrderCollection.Sum(m => m.PurchaseCount); } }
        public double ManufactoryOrdersPurchaseTotal { get { return (ManufactoryOrderCollection is null) ? 0 : ManufactoryOrderCollection.Sum(m => m.PurchasePrice); } }
        public double ManufactoryOrdersReturnCount { get { return (ManufactoryOrderCollection is null) ? 0 : ManufactoryOrderCollection.Sum(m => m.ReturnCount); } }
        public double ManufactoryOrdersReturnTotal { get { return (ManufactoryOrderCollection is null) ? 0 : ManufactoryOrderCollection.Sum(m => m.ReturnPrice); } }

        #endregion ----- Define Variables -----

        public PurchaseReturnReportViewModel()
        {
            RegisterCommands();

            WareHouseCollection = new WareHouses(WareHouseDb.Init());
            ManufacturerCollection = new Manufactories(ManufactoryDB.GetAllManufactories());
            SelectedWareHouse = WareHouseCollection[0];
        }

        #region ----- Define Actions -----

        private void SearchAction()
        {
            if (!IsSearchConditionValid()) return;

            MainWindow.ServerConnection.OpenConnection();
            ManufactoryOrderCollection = ManufactoryOrders.GetManufactoryOrdersBySearchCondition(StartDate, EndDate, Manufacturer is null ? string.Empty : Manufacturer.Name, SelectedWareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();

            SearchStartDate = (DateTime)StartDate;
            SearchEndDate = (DateTime)EndDate;
        }

        private void ExportCSVAction()
        {
            if (CurrentManufactoryOrder != null)
                CurrentManufactoryOrder.ExportToCSV(SearchStartDate, SearchEndDate);
        }

        private void ExportCSVDetailTotalAction()
        {
            if (CurrentManufactoryOrder != null && SelectedWareHouse != null)
            {
                CurrentManufactoryOrder.ExportToCSVTotalDetail(SearchStartDate, SearchEndDate, SelectedWareHouse.ID);
            }
        }

        private void ExportCSVTotalAction()
        {
            if (ManufactoryOrderCollection is null || ManufactoryOrderCollection.Count == 0)
                return;

            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "進退貨報表存檔";
            fdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            fdlg.Filter = "報表格式|*.csv";
            fdlg.FileName = $"{(Manufacturer is null ? string.Empty : Manufacturer.Name)}進退貨報表_{SearchStartDate.ToString("yyyyMMdd")}-{SearchEndDate.ToString("yyyyMMdd")}";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("廠商,進貨單數,進貨金額,退貨單數,退貨金額");
                        foreach (var order in ManufactoryOrderCollection)
                        {
                            file.WriteLine($"{order.ManufactoryName},{order.PurchaseCount},{order.PurchasePrice},{order.ReturnCount},{order.ReturnPrice}");
                        }
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommands()
        {
            SearchCommand = new RelayCommand(SearchAction);
            ExportCSVCommand = new RelayCommand(ExportCSVAction);
            ExportCSVTotalCommand = new RelayCommand(ExportCSVTotalAction);
            ExportCSVDetailTotalCommand = new RelayCommand(ExportCSVDetailTotalAction);
        }

        private bool IsSearchConditionValid()
        {
            if (StartDate is null || EndDate is null)
            {
                MessageWindow.ShowMessage("查詢時間為必填欄位!", MessageType.ERROR);
                return false;
            }

            if (EndDate < StartDate)
            {
                MessageWindow.ShowMessage("起始日期大於終結日期!", MessageType.ERROR);
                return false;
            }

            return true;
        }

        #endregion ----- Define Functions -----
    }
}