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
        public NormalViewModel NormalViewModel { get; set; }
        #endregion

        #region ----- Define Commands -----
        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand ShowHistoryCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private BalanceSheetTypeEnum balanceSheetType = BalanceSheetTypeEnum.NoDetail;
        private BalanceSheetData leftSelectedData;
        private BalanceSheetData rightSelectedData;
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
        public BalanceSheetData LeftSelectedData
        {
            get { return leftSelectedData; }
            set
            {
                leftSelectedData = value;
                rightSelectedData = null;
                RaisePropertyChanged(nameof(LeftSelectedData));
                RaisePropertyChanged(nameof(RightSelectedData));
                ChangeDetail();
            }
        }
        public BalanceSheetData RightSelectedData
        {
            get { return rightSelectedData; }
            set
            {
                rightSelectedData = value;
                leftSelectedData = null;
                RaisePropertyChanged(nameof(LeftSelectedData));
                RaisePropertyChanged(nameof(RightSelectedData));
                ChangeDetail();
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
            NormalViewModel = new NormalViewModel();

            ReloadCommand = new RelayCommand(ReloadAction);
            ShowHistoryCommand = new RelayCommand(ShowHistoryAction);

            ReloadAction();
        }

        #region ----- Define Actions -----
        private void ReloadAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataSet dataSet = CashReportDb.GetBalanceSheet();

            if (dataSet.Tables.Count != 7)
            {
                MessageWindow.ShowMessage("連線錯誤 請稍後再試!", MessageType.ERROR);
                return;
            }
            
            LeftBalanceSheetDatas = new BalanceSheetDatas(dataSet.Tables[0]);
            RightBalanceSheetDatas = new BalanceSheetDatas(dataSet.Tables[1]);
            LeftTotal = (double)dataSet.Tables[2].Rows[0].Field<decimal>("LEFT_TOTAL");
            RightTotal = (double)dataSet.Tables[3].Rows[0].Field<decimal>("RIGHT_TOTAL");

            PayableViewModel.StrikeDatas = new StrikeDatas(dataSet.Tables[4]);
            PayViewModel.StrikeDatas = new StrikeDatas(dataSet.Tables[5]);
            MedPointViewModel.StrikeDatas = new StrikeDatas(dataSet.Tables[6]);

            MainWindow.ServerConnection.CloseConnection();

            BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
            ChangeDetail();
        }
        private void ShowHistoryAction()
        {
            var historyWindow = new StrikeHistoryWindow();
            historyWindow.ShowDialog();

            ReloadAction();
        }
        #endregion

        #region ----- Define Functions -----
        private void ChangeDetail()
        {
            if (LeftSelectedData != null)
            {
                if (LeftSelectedData.Name.Contains("現金"))
                {
                    BalanceSheetType = BalanceSheetTypeEnum.Transfer;
                    TransferViewModel.Target = "銀行";
                    TransferViewModel.MaxValue = LeftSelectedData.Value;
                }
                else if (LeftSelectedData.Name.Contains("銀行"))
                {
                    BalanceSheetType = BalanceSheetTypeEnum.Transfer;
                    TransferViewModel.Target = "現金";
                    TransferViewModel.MaxValue = LeftSelectedData.Value;
                }
                else if (LeftSelectedData.Name.Contains("申報應收帳款"))
                    BalanceSheetType = BalanceSheetTypeEnum.MedPoint;
                else
                {
                    if (LeftSelectedData.ID.Length == 3)
                    {
                        NormalViewModel = new NormalViewModel(LeftSelectedData.ID);
                        BalanceSheetType = BalanceSheetTypeEnum.Normal;
                    }
                    else {
                        BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    }
                }
            }
            else if (RightSelectedData != null)
            {
                if (RightSelectedData.Name.Contains("應付帳款"))
                    BalanceSheetType = BalanceSheetTypeEnum.Payable;
                else if (RightSelectedData.Name.Contains("代付"))
                {
                    var ins = ViewModelMainWindow.Institutions.Single(i => i.ID.Equals(RightSelectedData.ID));

                    BalanceSheetType = BalanceSheetTypeEnum.Pay;
                    PayViewModel.Target = ins;
                }
                else
                {
                    if (RightSelectedData.ID.Length==3) { 
                    NormalViewModel = new NormalViewModel(RightSelectedData.ID);
                    BalanceSheetType = BalanceSheetTypeEnum.Normal;
                }
                    else
                {
                    BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                }
            }
            }
        }
        #endregion
    }
}
