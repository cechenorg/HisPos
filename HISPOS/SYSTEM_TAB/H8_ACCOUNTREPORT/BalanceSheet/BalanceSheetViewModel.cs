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
using System;
using System.Reflection;


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
        public ProductViewModel ProductViewModel { get; set; }
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

        public DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                RaisePropertyChanged(nameof(EndDate));
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
        DataSet _outputDataSet;
        private void ExportCSVAction()
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.FileName = "資產負債表.csv";
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
                result += DetailToExcel(filePath);//(20220419明細匯出)
                try
                {
                    File.WriteAllText(filePath, result, Encoding.Unicode);
                    ConfirmWindow cw = new ConfirmWindow("是否開啟檔案", "確認");//(20220419匯出完成詢問使用者是否開啟檔案)
                    if ((bool)cw.DialogResult)
                    {
                        System.Diagnostics.Process.Start(filePath);
                    }
                }
                catch(Exception e)
                {
                    MessageWindow.ShowMessage(e.Message, MessageType.WARNING);
                }
            }
        }

        /// <summary>
        /// 資產負債表明細匯出(20220419)
        /// </summary>
        /// <returns></returns>
        private string DetailToExcel(string path)
        {
            bool isExist_00 = true; //正資產類      //判斷是否已加入"正資產類"
            bool isExist_10 = true; //負資產類      //判斷是否已加入"負資產類"
            bool isExist_20 = true; //負股東權益類  //判斷是否已加入"負股東權益類"
            string result = "\r\n\r\n\r\n科別\t項目\t金額\r\n";
            foreach (DataTable table in _outputDataSet.Tables)
            {
                string tableName = Convert.ToString(table.TableName);
                if (tableName.Substring(0, 2) == "00" && isExist_00)
                {
                    result += "00" + "\t" + "正資產類" + "\t" + _amt_00 + "\r\n";
                    isExist_00 = false;
                }
                if (tableName.Substring(0, 2) == "10" && isExist_10)
                {
                    result += "10" + "\t" + "負資產類" + "\t" + _amt_10 + "\r\n";
                    isExist_10 = false;
                }
                if (tableName.Substring(0, 2) == "20" && isExist_20)
                {
                    result += "20" + "\t" + "負股東權益類" + "\t" + _amt_20 + "\r\n";
                    isExist_20 = false;
                }
                foreach (DataRow dr in table.Rows)
                {
                    foreach (DataColumn dc in table.Columns)
                    {
                        result += dr[dc.ColumnName] + "\t";
                    }
                    result += "\r\n";
                }
            }
            return result;
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
            DataSet dataSet = GetBalanceSheet();

            if (dataSet.Tables.Count != 4)
            {
                MessageWindow.ShowMessage("連線錯誤 請稍後再試!", MessageType.ERROR);
                return;
            }
            else
            {
                if(dataSet.Tables[0] != null)
                {
                    LeftBalanceSheetDatas = new BalanceSheetDatas(dataSet.Tables[0]);
                }
                if(dataSet.Tables[1] != null)
                {
                    RightBalanceSheetDatas = new BalanceSheetDatas(dataSet.Tables[1]);
                }
                if (dataSet.Tables[2] != null && dataSet.Tables[2].Rows.Count > 0)
                {
                    LeftTotal = (double)dataSet.Tables[2].Rows[0].Field<decimal>("Value");
                }
                if (dataSet.Tables[3] != null && dataSet.Tables[3].Rows.Count > 0)
                {
                    RightTotal = (double)dataSet.Tables[3].Rows[0].Field<decimal>("Value");
                }
                //if (dataSet.Tables[4] != null)
                //{
                //    MedPointViewModel.StrikeDatas = new StrikeDatas(dataSet.Tables[4]);
                //}
            }
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
                    NormalViewModel = new NormalViewModel("001", _endDate);
                    BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    BalanceSheetType = BalanceSheetTypeEnum.Normal;
                }
                else if (LeftSelectedData.Name.Contains("銀行"))
                {
                    BalanceSheetType = BalanceSheetTypeEnum.Transfer;
                    TransferViewModel.Target = "現金";
                    TransferViewModel.MaxValue = LeftSelectedData.Value;
                    BankViewModel = new BankViewModel(LeftSelectedData.ID, _endDate);
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
                            ProductViewModel = new ProductViewModel(LeftSelectedData.ID, _endDate);
                            BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                            BalanceSheetType = BalanceSheetTypeEnum.Product;
                        }
                        else if (LeftSelectedData.ID == "007")
                        {
                            BankViewModel = new BankViewModel(LeftSelectedData.ID,_endDate);
                            BalanceSheetType = BalanceSheetTypeEnum.Normal;
                            BalanceSheetType = BalanceSheetTypeEnum.Bank;
                        }
                        else if (LeftSelectedData.ID == "105")
                        {
                            NormalNoEditViewModel = new NormalNoEditViewModel(LeftSelectedData.ID,_endDate);
                            BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                            BalanceSheetType = BalanceSheetTypeEnum.NormalNoEdit;
                        }
                        else
                        {
                            NormalViewModel = new NormalViewModel(LeftSelectedData.ID, _endDate);
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
                    NormalViewModel = new NormalViewModel(RightSelectedData.ID, _endDate);
                    BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    BalanceSheetType = BalanceSheetTypeEnum.Normal;
                }
                else if (RightSelectedData.Name.Contains("代付"))
                {
                    NormalViewModel = new NormalViewModel(RightSelectedData.ID, _endDate);
                    BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    BalanceSheetType = BalanceSheetTypeEnum.Normal;
                }
                else
                {
                    if (RightSelectedData.ID == "105")
                    {
                        NormalViewModel = new NormalViewModel(RightSelectedData.ID, _endDate);
                        BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                        BalanceSheetType = BalanceSheetTypeEnum.Normal;
                    }
                    else if (RightSelectedData.ID == "201" || RightSelectedData.ID == "202" || RightSelectedData.ID == "204")
                    {
                        BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                    }
                    else if (RightSelectedData.ID.Length == 3)
                    {
                        NormalViewModel = new NormalViewModel(RightSelectedData.ID,_endDate);
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

        #region 資產負債表
        decimal _amt_00;//+資產總額
        decimal _amt_10;//-資產總額
        decimal _amt_20;//-股東權益
        private DataSet GetBalanceSheet()
        {
            _outputDataSet = new DataSet();
            DataSet ds = new DataSet();
            DataTable leftTable = new DataTable();//正資產--DataSet1
            DataColumn dcID = new DataColumn();
            dcID.ColumnName = "ID";
            dcID.DataType = typeof(string);
            leftTable.Columns.Add(dcID);
            DataColumn dcName = new DataColumn();
            dcName.ColumnName = "Header";
            dcName.DataType = typeof(string);
            leftTable.Columns.Add(dcName);
            DataColumn dcType = new DataColumn();
            dcType.ColumnName = "Type";
            dcType.DataType = typeof(string);
            leftTable.Columns.Add(dcType);
            DataColumn dcValue = new DataColumn();
            dcValue.ColumnName = "Value";
            dcValue.DataType = typeof(decimal);
            leftTable.Columns.Add(dcValue);
            DataTable rightTable = leftTable.Clone();//負資產--DataSet2
            DataTable leftTotal = new DataTable();//正資產總額--DataSet3
            DataColumn dcTotal = new DataColumn();
            dcTotal.ColumnName = "Value";
            dcTotal.DataType = typeof(decimal);
            leftTotal.Columns.Add(dcTotal);
            DataTable rightTotal = leftTotal.Clone();//正資產總額--DataSet4

            Dictionary<string, string> leftAccount = new Dictionary<string, string>();
            leftAccount.Add("001", "流動資產-現金");
            leftAccount.Add("002", "流動資產-銀行");
            leftAccount.Add("003", "流動資產-應收帳款");
            leftAccount.Add("004", "流動資產-申報應收帳款");
            leftAccount.Add("005", "流動資產-零用金");
            leftAccount.Add("006", "存貨-商品");
            leftAccount.Add("007", "預付款項-預付款項");
            leftAccount.Add("008", "預付款項-其他預付款");
            leftAccount.Add("009", "長期投資-長期投資");
            leftAccount.Add("010", "固定資產-資產");
            leftAccount.Add("011", "固定資產-累計折舊");
            leftAccount.Add("012", "其他資產-存出保證金");

            MainWindow.ServerConnection.OpenConnection();
            foreach (KeyValuePair<string,string> pair in leftAccount)
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("ID", pair.Key));
                parameters.Add(new SqlParameter("edate", _endDate));
                DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsDetail]", parameters);
                table.TableName = pair.Key;
                DataRow newRow = leftTable.NewRow();
                newRow["ID"] = pair.Key;
                newRow["Header"] = pair.Value;
                newRow["Type"] = pair.Key == "011"? "貸方" : "借方";
                if (table != null && table.Rows.Count > 0)
                {
                    decimal totalAmt = Convert.ToDecimal(table.Compute("Sum(Value)",""));
                    newRow["Value"] = totalAmt;
                    if (pair.Key == "004")
                    {
                        table.Columns["Name"].ColumnName = "Header";
                        MedPointViewModel.StrikeDatas = new StrikeDatas(table);
                    }
                    _outputDataSet.Tables.Add(table);
                }
                else
                {
                    newRow["Value"] = 0;
                }
                leftTable.Rows.Add(newRow);
            }
            DataRow newLeftRow = leftTable.NewRow();
            newLeftRow["ID"] = "00";
            newLeftRow["Header"] = "+資產類";
            newLeftRow["Type"] = string.Empty;
            if (leftTable != null && leftTable.Rows.Count > 0)
            {
                _amt_00 = Convert.ToDecimal(leftTable.Compute("Sum(Value)", ""));
                newLeftRow["Value"] = _amt_00;
            }
            else
            {
                _amt_00 = 0;
                newLeftRow["Value"] = 0;
            }
            leftTable.Rows.Add(newLeftRow);
            DataView dvLeft = leftTable.AsDataView();
            dvLeft.Sort = "ID";
            ds.Tables.Add(dvLeft.ToTable());

            Dictionary<string, string> rightAccount = new Dictionary<string, string>();
            rightAccount.Add("101", "流動負債-應付帳款");
            rightAccount.Add("102", "流動負債-應付費用");
            rightAccount.Add("103", "流動負債-應付稅捐");
            rightAccount.Add("104", "預收款項-預收款");
            rightAccount.Add("105", "代收-代付");
            rightAccount.Add("201", "資本-股本(登記)");
            rightAccount.Add("202", "公積及盈餘-本期損益");
            rightAccount.Add("203", "公積及盈餘-未分配損益");
            rightAccount.Add("204", "公積及盈餘-累積盈虧");

            foreach (KeyValuePair<string, string> pair in rightAccount)
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("ID", pair.Key));
                parameters.Add(new SqlParameter("edate", _endDate));
                DataTable table = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsDetail]", parameters);
                table.TableName = pair.Key;
                DataRow newRow = rightTable.NewRow();
                newRow["ID"] = pair.Key;
                newRow["Header"] = pair.Value;
                newRow["Type"] = "貸方";
                if(pair.Key == "204" && rightTable.Select("ID In ('101','102','103','104','105','201','202','203')").Length > 0)
                {
                    decimal leftAmt = Convert.ToDecimal(leftTable.Compute("Sum(Value)", "ID = '00'"));
                    decimal rightAmt = Convert.ToDecimal(rightTable.Compute("Sum(Value)", ""));
                    newRow["Value"] = leftAmt - rightAmt;
                    _outputDataSet.Tables.Add(table);
                }
                else if (table != null && table.Rows.Count > 0)
                {
                    decimal amt = Convert.ToDecimal(table.Compute("Sum(Value)", ""));
                    newRow["Value"] = amt;
                    _outputDataSet.Tables.Add(table);
                }
                else
                {
                    newRow["Value"] = 0;
                }
                
                rightTable.Rows.Add(newRow);
            }
            DataRow newRightRow = rightTable.NewRow();
            newRightRow["ID"] = "10";
            newRightRow["Header"] = "-負債類";
            newRightRow["Type"] = string.Empty;
            if (rightTable != null && rightTable.Rows.Count > 0)
            {
                _amt_10 = Convert.ToDecimal(rightTable.Compute("Sum(Value)", @"ID Like '10%'"));
                newRightRow["Value"] = _amt_10;
            }
            else
            {
                _amt_10 = 0;
                newRightRow["Value"] = 0;
            }
            rightTable.Rows.Add(newRightRow);
            newRightRow = rightTable.NewRow();
            newRightRow["ID"] = "20";
            newRightRow["Header"] = "-股東權益類";
            newRightRow["Type"] = string.Empty;
            if (rightTable != null && rightTable.Rows.Count > 0)
            {
                _amt_20 = Convert.ToDecimal(rightTable.Compute("Sum(Value)", @"ID Like '20%'"));
                newRightRow["Value"] = _amt_20;
            }
            else
            {
                _amt_20 = 0;
                newRightRow["Value"] = 0;
            }
            rightTable.Rows.Add(newRightRow);
            DataView dvRight = rightTable.AsDataView();
            dvRight.Sort = "ID";
            ds.Tables.Add(dvRight.ToTable());
            #region 總額
            DataRow leftTotalRow = leftTotal.NewRow();
            leftTotalRow["Value"] = Convert.ToDecimal(leftTable.Compute("Sum(Value)","ID = '00'"));
            leftTotal.Rows.Add(leftTotalRow);
            ds.Tables.Add(leftTotal);
            DataRow rightTotalRow = rightTotal.NewRow();
            rightTotalRow["Value"] = Convert.ToDecimal(rightTable.Compute("Sum(Value)", "ID In ('10','20')"));
            rightTotal.Rows.Add(rightTotalRow);
            ds.Tables.Add(rightTotal);
            #endregion
            return ds;
        }
        #endregion
    }
}