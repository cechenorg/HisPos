using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductManageDetail;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.SetPrices;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.PriceControl
{
    public class SingdePriceControlViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand ViewHistoryPriceCommand { get; set; }
        public RelayCommand PrintMedicineLabelCommand { get; set; }
        public RelayCommand SetPricesCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private string productID;
        private string wareHouseID;

        private ProductManageDetail medicineDetail;

        public ProductManageDetail MedicineDetail
        {
            get { return medicineDetail; }
            set { Set(() => MedicineDetail, ref medicineDetail, value); }
        }

        #endregion ----- Define Variables -----

        public SingdePriceControlViewModel()
        {
            ViewHistoryPriceCommand = new RelayCommand(ViewHistoryPriceAction);
            PrintMedicineLabelCommand = new RelayCommand(PrintMedicineLabelAction);
            SetPricesCommand = new RelayCommand(SetPricesAction);
        }

        #region ----- Define Actions -----

        private void ViewHistoryPriceAction()
        {
            NHIMedicineHistoryPriceWindow medicineHistoryPriceWindow = new NHIMedicineHistoryPriceWindow(productID);
            medicineHistoryPriceWindow.ShowDialog();
        }

        private void PrintMedicineLabelAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認列印藥品標籤?", "");

            if (!(bool)confirmWindow.DialogResult) return;

            MedicineTagStruct medicineTagStruct = MedicineTagStruct.GetDataByID(productID);

            if (medicineTagStruct is null)
                MessageWindow.ShowMessage("列印失敗 請稍後再試", MessageType.ERROR);
            else
                PrintMedBagSingleMode(medicineTagStruct);
        }

        private void SetPricesAction()
        {
            SetPricesWindow setPricesWindow = new SetPricesWindow(productID, MedicineDetail.RetailPrice, MedicineDetail.MemberPrice, MedicineDetail.EmployeePrice, MedicineDetail.SpecialPrice);
            setPricesWindow.ShowDialog();

            if ((bool)setPricesWindow.DialogResult)
            {
                ReloadData(productID, wareHouseID, ProductTypeEnum.OTCMedicine);
            }
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        public void PrintMedBagSingleMode(MedicineTagStruct medicineTagStruct)
        {
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            SetSingleModeMedTagReportViewer(rptViewer, medicineTagStruct);
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                ((ViewModelMainWindow)MainWindow.Instance.DataContext).StartPrintMedicineTag(rptViewer);
            });
        }

        private void SetSingleModeMedTagReportViewer(ReportViewer rptViewer, MedicineTagStruct medicineTagStruct)
        {
            var medicineList = new Collection<MedicineTagStruct>();
            medicineList.Add(medicineTagStruct);
            var json = JsonConvert.SerializeObject(medicineList);
            var dataTable = JsonConvert.DeserializeObject<DataTable>(json);
            rptViewer.LocalReport.ReportPath = @"RDLC\MedicineTag.rdlc";
            rptViewer.ProcessingMode = ProcessingMode.Local;
            rptViewer.LocalReport.DataSources.Clear();
            var rd = new ReportDataSource("MedicineTagDataSet", dataTable);
            rptViewer.LocalReport.DataSources.Add(rd);
            rptViewer.LocalReport.Refresh();
        }

        public void ReloadData(string proID, string wareID, ProductTypeEnum type)
        {
            productID = proID;
            wareHouseID = wareID;

            DataTable manageMedicineDetailDataTable = null;

            switch (type)
            {
                case ProductTypeEnum.OTCMedicine:
                    manageMedicineDetailDataTable = ProductDetailDB.GetProductManageOTCMedicineDetailByID(proID);
                    break;

                case ProductTypeEnum.NHIMedicine:
                    manageMedicineDetailDataTable = ProductDetailDB.GetProductManageNHIMedicineDetailByID(proID);
                    break;

                case ProductTypeEnum.SpecialMedicine:
                    manageMedicineDetailDataTable = ProductDetailDB.GetProductManageSpecialMedicineDetailByID(proID);
                    break;
            }

            if (manageMedicineDetailDataTable is null || manageMedicineDetailDataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            switch (type)
            {
                case ProductTypeEnum.OTCMedicine:
                    MedicineDetail = new ProductManageDetail(manageMedicineDetailDataTable.Rows[0]);
                    break;

                case ProductTypeEnum.NHIMedicine:
                    MedicineDetail = new ProductNHIDetail(manageMedicineDetailDataTable.Rows[0]);
                    break;

                case ProductTypeEnum.SpecialMedicine:
                    MedicineDetail = new ProductNHISpecialDetail(manageMedicineDetailDataTable.Rows[0]);
                    break;
            }
        }

        #endregion ----- Define Functions -----
    }
}