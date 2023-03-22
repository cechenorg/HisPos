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
using System.ComponentModel;
using His_Pos.NewClass.Report.Accounts;
using His_Pos.NewClass.StockValue;
using System.Windows.Threading;
using His_Pos.NewClass.Report;
using System.Linq;
using System.Diagnostics;
using ClosedXML.Excel;

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

        /// <summary>
        /// 2022052預付款新增預付訂金
        /// </summary>
        public ProductViewModel ProductViewModel_104 { get; set; }
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
        private DataTable balanceData;
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
        private string busyContent;
        public string BusyContent
        {
            get => busyContent;
            set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                Set(() => IsBusy, ref isBusy, value);
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

            DetailToExcel();
            /*
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
            */
        }
        private void DetailToExcel()
        {
            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "資產負債表";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = string.Format("資產負債表{0}", EndDate.ToString("yyyyMMdd"));
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add("資產負債表");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                /*
                var col1 = ws.Column("A");
                col1.Width = 20;
                var col2 = ws.Column("B");
                col2.Width = 20;
                var col3 = ws.Column("C");
                col3.Width = 20;
                var col4 = ws.Column("D");
                col4.Width = 20;
                var col5 = ws.Column("E");
                col5.Width = 30;
                */
                var col6 = ws.Column("F");
                col6.Width = 20;
                
                ws.Cell("A1").Value = "會計科目代號1";
                ws.Cell("B1").Value = "會計科目代號2";
                ws.Cell("C1").Value = "會計科目名稱2";
                ws.Cell("D1").Value = "會計科目代號3";
                ws.Cell("E1").Value = "會計科目名稱3";
                ws.Cell("F1").Value = "金額";

                DataTable data = AccountsDb.GetBalanceSheet(_endDate);
                var items = data.AsEnumerable().Where(w => w.Field<decimal>("acctValue") != 0);
                var rangeWithData = ws.Cell(2, 1).InsertData(items);
                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                ws.Columns().AdjustToContents();//欄位寬度根據資料調整
                try
                {
                    wb.SaveAs(fdlg.FileName);
                    ConfirmWindow cw = new ConfirmWindow("是否開啟檔案", "確認");
                    if ((bool)cw.DialogResult)
                    {
                        myProcess.StartInfo.UseShellExecute = true;
                        myProcess.StartInfo.FileName = fdlg.FileName;
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
        /*
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
        */

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
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                Dispatcher.CurrentDispatcher.Invoke(delegate()
                {
                    BusyContent = "報表查詢中...";
                    NewGetBalanceSheet();
                });
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void ShowHistoryAction()
        {
            var historyWindow = new StrikeHistoryWindow();
            historyWindow.Show();
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
                NormalViewModel = new NormalViewModel(balanceData, LeftSelectedData.ID, EndDate);
                BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                BalanceSheetType = BalanceSheetTypeEnum.Normal;
            }
            else if (RightSelectedData != null)
            {
                NormalViewModel = new NormalViewModel(balanceData, RightSelectedData.ID, EndDate);
                BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                BalanceSheetType = BalanceSheetTypeEnum.Normal;
            }
            else
            {
                BalanceSheetType = BalanceSheetTypeEnum.NoDetail;
                return;
            }
        }

        #endregion ----- Define Functions -----

        #region 資產負債表
        /*
        decimal _amt_00;//+資產總額
        decimal _amt_10;//-資產總額
        decimal _amt_20;//-股東權益
        */
        #region old
        /*
        private DataSet GetBalanceSheet()
        {
            _outputDataSet = new DataSet();
            DataSet ds = new DataSet();
            DataTable leftTable = NormalViewModel.tableClone();//正資產--DataSet1
            DataTable rightTable = leftTable.Clone();//負資產--DataSet2
            DataTable leftTotal = new DataTable();//正資產總額--DataSet3
            DataColumn dcTotal = new DataColumn();
            dcTotal.ColumnName = "Value";
            dcTotal.DataType = typeof(decimal);
            leftTotal.Columns.Add(dcTotal);
            DataTable rightTotal = leftTotal.Clone();//正資產總額--DataSet4

            Dictionary<string, string> leftAccount = new Dictionary<string, string>
            {
                { "001", "流動資產-現金" },
                { "002", "流動資產-銀行" },
                { "003", "流動資產-應收帳款" },
                { "004", "流動資產-申報應收帳款" },
                { "005", "流動資產-零用金" },
                { "006", "存貨-商品" },
                { "007", "預付款項-預付款項" },
                { "008", "預付款項-其他預付款" },
                { "009", "長期投資-長期投資" },
                { "010", "固定資產-資產" },
                { "011", "固定資產-累計折舊" },
                { "012", "其他資產-存出保證金" }
            };

            foreach (KeyValuePair<string,string> pair in leftAccount)
            {
                DataTable table;
                if (pair.Key.Equals("006"))
                {
                    table = StockValueDb.GetStockVale(_endDate.AddDays(-7), _endDate);//直接取庫存現值報表
                }
                else
                {
                    table = AccountsDb.GetAccountsDetail(pair.Key, _endDate);
                }
                table.TableName = pair.Key;
                DataRow newRow = leftTable.NewRow();
                newRow["ID"] = pair.Key;
                newRow["Header"] = pair.Value;
                newRow["Type"] = pair.Key == "011" ? "貸方" : "借方";
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

            Dictionary<string, string> rightAccount = new Dictionary<string, string>
            {
                { "101", "流動負債-應付帳款" },
                { "102", "流動負債-應付費用" },
                { "103", "流動負債-應付稅捐" },
                { "104", "預收款項-預收款" },
                { "105", "代收-代付" },
                { "201", "資本-股本(登記)" },
                { "202", "公積及盈餘-本期損益" },
                { "203", "公積及盈餘-未分配損益" }
                //{ "204", "公積及盈餘-累積盈虧" }
            };

            foreach (KeyValuePair<string, string> pair in rightAccount)
            {
                DataTable table;
                if (pair.Key == "202")
                {
                    table = NormalViewModel.tableClone();
                    table = NormalViewModel.GetProfit(table, true);
                }
                else if (pair.Key == "203")
                {
                    table = AccountsDb.GetAccountsDetail(pair.Key, _endDate);
                    table = NormalViewModel.GetProfit(table, false);
                }
                else
                {
                    table = AccountsDb.GetAccountsDetail(pair.Key, _endDate);
                }
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
        */
        #endregion
        private void NewGetBalanceSheet()
        {
            balanceData = AccountsDb.GetBalanceSheet(_endDate);
            //noStrikeData = AccountsDb.GetSourceDataInLocal(_endDate);
            //noSourceData = AccountsDb.GetNoSourceDataInLocal(_endDate);

            DataTable tbLeftGrid = balanceData.Clone();
            DataTable tbRightGrid = balanceData.Clone();

            foreach (DataRow dr in balanceData.Rows)
            {
                int acctLevel1 = Convert.ToInt32(dr["acctLevel1"]);
                int acctLevel2 = Convert.ToInt32(dr["acctLevel2"]);
                if (acctLevel1 == 1)
                {
                    DataRow newRow = tbLeftGrid.NewRow();
                    if (tbLeftGrid.Select(string.Format("acctLevel2 = {0}", acctLevel2)).Length > 0)
                    {
                        continue;
                    }
                    newRow["acctLevel1"] = acctLevel1;
                    newRow["acctLevel2"] = acctLevel2;
                    newRow["acctName2"] = dr["acctName2"];
                    newRow["acctValue"] = Convert.ToInt32(balanceData.Compute("Sum(acctValue)", string.Format("acctLevel1 = {0} And acctLevel2 = {1}", acctLevel1, acctLevel2)));
                    
                    tbLeftGrid.Rows.Add(newRow);
                }
                else
                {
                    DataRow newRow = tbRightGrid.NewRow();
                    if (tbRightGrid.Select(string.Format("acctLevel2 = {0}", acctLevel2)).Length > 0)
                    {
                        continue;
                    }
                    newRow["acctLevel1"] = acctLevel1;
                    newRow["acctLevel2"] = acctLevel2;
                    newRow["acctName2"] = dr["acctName2"];
                    newRow["acctValue"] = Convert.ToInt32(balanceData.Compute("Sum(acctValue)", string.Format("acctLevel1 = {0} And acctLevel2 = {1}", acctLevel1, acctLevel2)));

                    tbRightGrid.Rows.Add(newRow);
                }
            }
            

            #region 預設彙總
            DataRow newrow1 = tbLeftGrid.NewRow();
            newrow1["acctLevel1"] = 1;
            newrow1["acctLevel2"] = "1000";
            newrow1["acctName2"] = "+資產類";
            newrow1["acctValue"] = Convert.ToInt32(tbLeftGrid.Compute("Sum(acctValue)", "acctLevel1 = 1"));
            tbLeftGrid.Rows.Add(newrow1);
            DataRow newrow2 = tbRightGrid.NewRow();
            newrow2["acctLevel1"] = 2;
            newrow2["acctLevel2"] = "2000";
            newrow2["acctName2"] = "-負債類";
            newrow2["acctValue"] = Convert.ToInt32(tbRightGrid.Compute("Sum(acctValue)", "acctLevel1 = 2"));
            tbRightGrid.Rows.Add(newrow2);
            DataRow newrow3 = tbRightGrid.NewRow();
            newrow3["acctLevel1"] = 3;
            newrow3["acctLevel2"] = "3000";
            newrow3["acctName2"] = "-股東權益";
            newrow3["acctValue"] = Convert.ToInt32(tbRightGrid.Compute("Sum(acctValue)", "acctLevel1 = 3"));
            tbRightGrid.Rows.Add(newrow3);
            #endregion
            DataView dv = new DataView(tbLeftGrid);
            dv.Sort = "acctLevel1,acctLevel2";
            tbLeftGrid = dv.ToTable();
            DataView dv2 = new DataView(tbRightGrid);
            dv2.Sort = "acctLevel1,acctLevel2";
            tbRightGrid = dv2.ToTable();

            LeftBalanceSheetDatas = new BalanceSheetDatas(tbLeftGrid);
            RightBalanceSheetDatas = new BalanceSheetDatas(tbRightGrid);
            LeftTotal = Convert.ToInt32(tbLeftGrid.Compute("Sum(acctValue)", "acctLevel2 = 1000"));
            RightTotal = Convert.ToInt32(tbRightGrid.Compute("Sum(acctValue)", "acctLevel2 = 2000 Or acctLevel2 = 3000"));
            BalanceSheetType = BalanceSheetTypeEnum.Normal;
            //return ds;
        }

        #endregion
    }
}