using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;
using His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet
{
    public class BalanceSheetViewModel : TabBase
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
        public NormalNoEditViewModel NormalNoEditViewModel { get; set; }
        public BankViewModel BankViewModel { get; set; }

        #endregion ----- Define ViewModels -----

        #region ----- Define Commands -----

        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand ShowHistoryCommand { get; set; }
        public RelayCommand FirstCommand { get; set; }
        public RelayCommand AccountManageCommand { get; set; }

        public RelayCommand ExportCSVCommand { get; set; }

        #endregion ----- Define Commands -----

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

        public double firstValue;

        public double FirstValue
        {
            get { return firstValue; }
            set
            {
                firstValue = value;
                RaisePropertyChanged(nameof(FirstValue));
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

        #endregion ----- Define Variables -----

        public BalanceSheetViewModel()
        {
            MedPointViewModel = new MedPointViewModel();
            TransferViewModel = new TransferViewModel();
            PayableViewModel = new PayableViewModel();
            PayViewModel = new PayViewModel();
            NormalViewModel = new NormalViewModel();

            ReloadCommand = new RelayCommand(ReloadAction);
            ShowHistoryCommand = new RelayCommand(ShowHistoryAction);
            FirstCommand = new RelayCommand(FirstAction);
            AccountManageCommand = new RelayCommand(AccountManageAction);
            ExportCSVCommand = new RelayCommand(ExportCSVAction);
            ReloadAction();
        }

        private void ExportCSVAction()
        {
            System.Windows.Forms.SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog();
            diag.FileName =   "資產負債表.csv";
            diag.Filter = "csv (*.csv)|*.csv";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                string filePath = diag.FileName;

                string result = "科別\t項目\t金額\t\t\t科別\t項目\t金額\r\n";

                int maxSize = LeftBalanceSheetDatas.Count > RightBalanceSheetDatas.Count
                    ? LeftBalanceSheetDatas.Count
                    : RightBalanceSheetDatas.Count;


                for (int i = 0; i < maxSize; i++)
                {
                    if (LeftBalanceSheetDatas.Count > i)
                    {
                        LeftBalanceSheetDatas[i].Name = LeftBalanceSheetDatas[i].Name.Replace("+", "");
                        LeftBalanceSheetDatas[i].Name = LeftBalanceSheetDatas[i].Name.Replace("-", "");
                        result += $"{LeftBalanceSheetDatas[i].Name}\t{LeftBalanceSheetDatas[i].Type}\t{LeftBalanceSheetDatas[i].Value}";
                    }
                    else
                    {
                        result += $" \t\t";
                    }

                    result += "\t\t\t";
                    if (RightBalanceSheetDatas.Count > i)
                    {
                        RightBalanceSheetDatas[i].Name = RightBalanceSheetDatas[i].Name.Replace("+", "");
                        RightBalanceSheetDatas[i].Name = RightBalanceSheetDatas[i].Name.Replace("-", "");
                        result += $"{RightBalanceSheetDatas[i].Name}\t{RightBalanceSheetDatas[i].Type}\t{RightBalanceSheetDatas[i].Value}";
                    }
                    result += "\r\n";
                }
                 
                File.WriteAllText(filePath, result, Encoding.Unicode);
            }
        }

        private void FirstAction()
        {
            if (LeftSelectedData != null)
            {
                try
                {
                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("Name", LeftSelectedData.ID));
                    parameters.Add(new SqlParameter("CurrentUserId ", ViewModelMainWindow.CurrentUser.ID));
                    parameters.Add(new SqlParameter("VALUE", FirstValue));
                    parameters.Add(new SqlParameter("NOTE", "first"));
                    parameters.Add(new SqlParameter("SourceId", LeftSelectedData.ID));
                    DataTable dataTable = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertAccountsRecordFirst]", parameters);
                    MainWindow.ServerConnection.CloseConnection();
                }
                catch
                {
                    MessageWindow.ShowMessage("發生錯誤請再試一次", MessageType.ERROR);
                    return;
                }
                MessageWindow.ShowMessage("設定成功", MessageType.SUCCESS);
            }
            else if (RightSelectedData != null)
            {
                try
                {
                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("Name", RightSelectedData.ID));
                    parameters.Add(new SqlParameter("CurrentUserId ", ViewModelMainWindow.CurrentUser.ID));
                    parameters.Add(new SqlParameter("VALUE", FirstValue));
                    parameters.Add(new SqlParameter("NOTE", "first"));
                    parameters.Add(new SqlParameter("SourceId", RightSelectedData.ID));
                    DataTable dataTable = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertAccountsRecordFirst]", parameters);
                    MainWindow.ServerConnection.CloseConnection();
                }
                catch
                {
                    MessageWindow.ShowMessage("發生錯誤請再試一次", MessageType.SUCCESS);
                    return;
                }
                MessageWindow.ShowMessage("設定成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("請選擇正確項目", MessageType.ERROR);
            }
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

            //PayableViewModel.StrikeDatas = new StrikeDatas(dataSet.Tables[4]);
            //PayViewModel.StrikeDatas = new StrikeDatas(dataSet.Tables[5]);
            MedPointViewModel.StrikeDatas = new StrikeDatas(dataSet.Tables[6]);

            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("載入完成。", MessageType.SUCCESS);
        }

        private void ShowHistoryAction()
        {
            var historyWindow = new StrikeHistoryWindow();
            historyWindow.ShowDialog();

            ReloadAction();
        }

        private void AccountManageAction()
        {
            var accManageWindow = new AccountManage();
            accManageWindow.ShowDialog();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void ChangeDetail()
        {
            if (LeftSelectedData != null)
            {
                if (LeftSelectedData.Name.Contains("現金"))
                {
                    NormalViewModel = new NormalViewModel("001");
                    BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    BalanceSheetType = BalanceSheetTypeEnum.Normal;
                }
                else if (LeftSelectedData.Name.Contains("銀行"))
                {
                    BalanceSheetType = BalanceSheetTypeEnum.Transfer;
                    TransferViewModel.Target = "現金";
                    TransferViewModel.MaxValue = LeftSelectedData.Value;
                    BankViewModel = new BankViewModel(LeftSelectedData.ID);
                    BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    BalanceSheetType = BalanceSheetTypeEnum.Bank;
                }
                else if (LeftSelectedData.Name.Contains("申報應收帳款"))
                    BalanceSheetType = BalanceSheetTypeEnum.MedPoint;
                else
                {
                    if (LeftSelectedData.ID.Length == 3)
                    {
                        if (LeftSelectedData.ID == "006")
                        {
                            //BankViewModel = new BankViewModel(LeftSelectedData.ID);
                            BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                        }
                        else if (LeftSelectedData.ID == "007")
                        {
                            BankViewModel = new BankViewModel(LeftSelectedData.ID);
                            BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                            BalanceSheetType = BalanceSheetTypeEnum.Bank;
                        }
                        else if (LeftSelectedData.ID == "105")
                        {
                            NormalNoEditViewModel = new NormalNoEditViewModel(LeftSelectedData.ID);
                            BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                            BalanceSheetType = BalanceSheetTypeEnum.NormalNoEdit;
                        }
                        else
                        {
                            NormalViewModel = new NormalViewModel(LeftSelectedData.ID);
                            BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                            BalanceSheetType = BalanceSheetTypeEnum.Normal;
                        }
                    }
                    else
                    {
                        BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    }
                }
            }
            else if (RightSelectedData != null)
            {
                if (RightSelectedData.Name.Contains("應付帳款"))
                {
                    NormalViewModel = new NormalViewModel(RightSelectedData.ID);
                    BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    BalanceSheetType = BalanceSheetTypeEnum.Normal;
                }
                else if (RightSelectedData.Name.Contains("代付"))
                {
                    //var ins = ViewModelMainWindow.Institutions.Single(i => i.ID.Equals(RightSelectedData.ID));

                    //BalanceSheetType = BalanceSheetTypeEnum.Pay;
                    //PayViewModel.Target = ins;
                    NormalViewModel = new NormalViewModel(RightSelectedData.ID);
                    BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    BalanceSheetType = BalanceSheetTypeEnum.Normal;
                }
                else
                {
                    if (RightSelectedData.ID == "105")
                    {
                        NormalViewModel = new NormalViewModel(RightSelectedData.ID);
                        BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                        BalanceSheetType = BalanceSheetTypeEnum.Normal;
                    }
                    else if (RightSelectedData.ID == "201" || RightSelectedData.ID == "202" || RightSelectedData.ID == "204")
                    {
                        BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    }
                    else if (RightSelectedData.ID.Length == 3)
                    {
                        NormalViewModel = new NormalViewModel(RightSelectedData.ID);
                        BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                        BalanceSheetType = BalanceSheetTypeEnum.Normal;
                    }
                    else
                    {
                        BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    }
                }
            }
        }

        #endregion ----- Define Functions -----
    }
}