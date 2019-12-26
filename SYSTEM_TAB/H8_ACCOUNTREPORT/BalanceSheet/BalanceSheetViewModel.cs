using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;
using His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet
{
    public class BalanceSheetViewModel: TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define ViewModels -----
        public MedPointViewModel MedPointViewModel { get; set; }
        public TransferViewModel TransferViewModel { get; set; }
        public PayableViewModel PayableViewModel { get; set; }
        public PayViewModel PayViewModel { get; set; }
        #endregion

        #region ----- Define Commands -----
        public RelayCommand ChangeDetailCommand { get; set; }
        public RelayCommand ReloadCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private BalanceSheetTypeEnum balanceSheetType = BalanceSheetTypeEnum.NoDetail;
        private BalanceSheetData selectedData;
        private BalanceSheetDatas leftBalanceSheetDatas;
        private BalanceSheetDatas rightBalanceSheetDatas;
        private double rightTotal;
        private double leftTotal;

        public BalanceSheetTypeEnum BalanceSheetType
        {
            get { return balanceSheetType; }
            set
            {
                balanceSheetType = value;
                RaisePropertyChanged(nameof(BalanceSheetType));
            }
        }
        public BalanceSheetData SelectedData
        {
            get { return selectedData; }
            set
            {
                selectedData = value;
                RaisePropertyChanged(nameof(SelectedData));
            }
        }
        public BalanceSheetDatas LeftBalanceSheetDatas
        {
            get { return leftBalanceSheetDatas; }
            set
            {
                leftBalanceSheetDatas = value;
                RaisePropertyChanged(nameof(LeftBalanceSheetDatas));
            }
        }
        public BalanceSheetDatas RightBalanceSheetDatas
        {
            get { return rightBalanceSheetDatas; }
            set
            {
                rightBalanceSheetDatas = value;
                RaisePropertyChanged(nameof(RightBalanceSheetDatas));
            }
        }
        public double RightTotal
        {
            get { return rightTotal; }
            set
            {
                rightTotal = value;
                RaisePropertyChanged(nameof(RightTotal));
            }
        }
        public double LeftTotal
        {
            get { return leftTotal; }
            set
            {
                leftTotal = value;
                RaisePropertyChanged(nameof(LeftTotal));
            }
        }
        #endregion

        public BalanceSheetViewModel()
        {
            MedPointViewModel = new MedPointViewModel();
            TransferViewModel = new TransferViewModel();
            PayableViewModel = new PayableViewModel();
            PayViewModel = new PayViewModel();

            ChangeDetailCommand = new RelayCommand(ChangeDetailAction);
            ReloadCommand = new RelayCommand(ReloadAction);

            ReloadAction();
        }

        #region ----- Define Actions -----
        private void ChangeDetailAction()
        {
            switch (BalanceSheetType)
            {
                case BalanceSheetTypeEnum.MedPoint:
                    break;
                case BalanceSheetTypeEnum.Pay:
                    break;
                case BalanceSheetTypeEnum.Payable:
                    break;
                case BalanceSheetTypeEnum.Transfer:
                    break;
            }
        }
        private void ReloadAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataSet dataSet = CashReportDb.GetBalanceSheet();

            if (dataSet.Tables.Count != 4)
            {
                MessageWindow.ShowMessage("連線錯誤 請稍後再試!", MessageType.ERROR);
                return;
            }
            
            LeftBalanceSheetDatas = new BalanceSheetDatas(dataSet.Tables[0]);
            RightBalanceSheetDatas = new BalanceSheetDatas(dataSet.Tables[1]);
            LeftTotal = (double)dataSet.Tables[2].Rows[0].Field<decimal>("LEFT_TOTAL");
            RightTotal = (double)dataSet.Tables[3].Rows[0].Field<decimal>("RIGHT_TOTAL");

            MainWindow.ServerConnection.CloseConnection();

            ChangeDetailAction();
        }
        #endregion

        #region ----- Define Functions -----
        #endregion
    }
}
