using ClosedXML.Excel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Trade;
using His_Pos.NewClass.Trade.TradeRecord;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionRecord
{
    public class ProductTransactionRecordViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }
        public int SelectTab
        {
            get { return selectTab; }
            set
            {
                Set(() => SelectTab, ref selectTab, value);
            }
        }
        private int selectTab = 1;
        /// <summary>
        /// 查詢日期Start
        /// </summary>
        public string StartDate
        {
            get { return startDate; }
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private string startDate = ConvertTWDate(DateTime.Today);
        /// <summary>
        /// 查詢日期End
        /// </summary>
        public string EndDate
        {
            get { return endDate; }
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }
        private string endDate = ConvertTWDate(DateTime.Today);
        /// <summary>
        /// 發票Start
        /// </summary>
        public string SInvoice
        {
            get { return sInvoice; }
            set
            {
                Set(() => SInvoice, ref sInvoice, value);
            }
        }
        private string sInvoice = string.Empty;
        /// <summary>
        /// 發票end
        /// </summary>
        public string EInvoice
        {
            get { return eInvoice; }
            set
            {
                Set(() => EInvoice, ref eInvoice, value);
            }
        }
        private string eInvoice = string.Empty;
        public IEnumerable<Employee> Emps
        {
            get { return emps; }
            set
            {
                Set(() => Emps, ref emps, value);
            }
        }
        private IEnumerable<Employee> emps;
        public Employee Emp
        {
            get { return emp; }
            set
            {
                Set(() => Emp, ref emp, value);
            }
        }
        private Employee emp;
        /// <summary>
        /// 是否常規銷售
        /// </summary>
        public bool IsIrregular
        {
            get { return isIrregular; }
            set
            {
                Set(() => IsIrregular, ref isIrregular, value);
            }
        }
        private bool isIrregular = false;
        /// <summary>
        /// 是否退貨
        /// </summary>
        public bool IsReturn
        {
            get { return isReturn; }
            set
            {
                Set(() => IsReturn, ref isReturn, value);
            }
        }
        private bool isReturn = false;
        /// <summary>
        /// 查詢商品代號
        /// </summary>
        public string SearchProductID
        {
            get { return searchProductID; }
            set
            {
                Set(() => SearchProductID, ref searchProductID, value);
            }
        }
        private string searchProductID = string.Empty;
        /// <summary>
        /// 查詢商品名稱
        /// </summary>
        public string SearchProductName
        {
            get { return searchProductName; }
            set
            {
                Set(() => SearchProductName, ref searchProductName, value);
            }
        }
        private string searchProductName = string.Empty;
        /// <summary>
        /// 毛利率
        /// </summary>
        public float SProfitPercent
        {
            get { return sProfitPercent; }
            set
            {
                Set(() => SProfitPercent, ref sProfitPercent, value);
            }
        }
        private float sProfitPercent = -100;
        /// <summary>
        /// 毛利率
        /// </summary>
        public float EProfitPercent
        {
            get { return eProfitPercent; }
            set
            {
                Set(() => EProfitPercent, ref eProfitPercent, value);
            }
        }
        private float eProfitPercent = 100;
        /// <summary>
        /// 銷售筆數
        /// </summary>
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                Set(() => TotalCount, ref totalCount, value);
            }
        }
        private int totalCount = 0;
        /// <summary>
        /// 總銷售金額
        /// </summary>
        public int TotalAmount
        {
            get { return totalAmount; }
            set
            {
                Set(() => TotalAmount, ref totalAmount, value);
            }
        }
        private int totalAmount = 0;
        /// <summary>
        /// 銷售紀錄
        /// </summary>
        public TradeRecordDetails RecordList
        {
            get { return recordList; }
            set
            {
                Set(() => RecordList, ref recordList, value);
            }
        }
        private TradeRecordDetails recordList;
        public TradeRecordDetail CurrentRecord
        {
            get { return currentRecord; }
            set
            {
                Set(() => CurrentRecord, ref currentRecord, value);
            }
        }
        private TradeRecordDetail currentRecord;
        /// <summary>
        /// 銷售明細
        /// </summary>
        public TradeRecordDetails RecordDetailList
        {
            get { return recordDetailList; }
            set
            {
                Set(() => RecordDetailList, ref recordDetailList, value);
            }
        }
        private TradeRecordDetails recordDetailList;
        public TradeRecordDetail CurrentRecordDetail
        {
            get { return currentRecordDetail; }
            set
            {
                Set(() => CurrentRecordDetail, ref currentRecordDetail, value);
            }
        }
        private TradeRecordDetail currentRecordDetail;
        /// <summary>
        /// 銷售彙總
        /// </summary>
        public TradeRecordDetails RecordSumList
        {
            get { return recordSumList; }
            set
            {
                Set(() => RecordSumList, ref recordSumList, value);
            }
        }
        private TradeRecordDetails recordSumList;
        public TradeRecordDetail CurrentRecordSum
        {
            get { return currentRecordSum; }
            set
            {
                Set(() => CurrentRecordSum, ref currentRecordSum, value);
            }
        }
        private TradeRecordDetail currentRecordSum;

        
        /// <summary>
        /// 顧客彙總
        /// </summary>
        public TradeRecordDetails CustomSumList
        {
            get { return customSumList; }
            set
            {
                Set(() => CustomSumList, ref customSumList, value);
            }
        }
        private TradeRecordDetails customSumList;
        public TradeRecordDetail CurrentCustomSum
        {
            get { return currentCustomSum; }
            set
            {
                Set(() => CurrentCustomSum, ref currentCustomSum, value);
            }
        }
        private TradeRecordDetail currentCustomSum;
        public ProductTransactionRecordViewModel()
        {
            PrintCommand = new RelayCommand(PrintAction);
            SubmitCommand = new RelayCommand(SubmitAction);
            ClearCommand = new RelayCommand(ClearAction);
            ChangeUiTypeCommand = new RelayCommand<string>(ChangeUiTypeAction);
            DownloadInvoiceCommand = new RelayCommand(DownloadInvoiceAction);
            GetEmployeeList();
        }
        public RelayCommand PrintCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand DownloadInvoiceCommand { get; set; }
        public RelayCommand<string> ChangeUiTypeCommand { get; set; }
        private void GetEmployeeList()
        {
            Emps = TradeService.GetPosEmployee();
        }
        private void SubmitAction()
        {
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                if (!(string.IsNullOrEmpty(SInvoice) || SInvoice.Length == 8 || int.TryParse(SInvoice, out _)))
                {
                    MessageWindow.ShowMessage("搜尋發票號碼必須為8位數字!", MessageType.ERROR);
                    return;
                }
                if (!(string.IsNullOrEmpty(EInvoice) || EInvoice.Length == 8 || int.TryParse(EInvoice, out int _)))
                {
                    MessageWindow.ShowMessage("搜尋發票號碼必須為8位數字!", MessageType.ERROR);
                    return;
                }

                int cashierID = -1;
                if (Emp != null)
                {
                    cashierID = Emp.ID;
                }
                string sdate = ConvertMaskedDate(StartDate);
                string edate = ConvertMaskedDate(EndDate);

                TradeQueryInfo queryInfo = new TradeQueryInfo()
                {
                    StartDate = sdate,
                    EndDate = edate,
                    StartInvoice = SInvoice,
                    EndInvoice = EInvoice,
                    ShowIrregular = IsIrregular,
                    ShowReturn = IsReturn,
                    CashierID = cashierID,
                    ProID = SearchProductID,
                    ProName = SearchProductName,
                    Flag = "0",
                    sProfitPercent = sProfitPercent / 100,
                    eProfitPercent = eProfitPercent / 100
                };
                DataTable result = TradeService.GetTradeRecordTable(queryInfo);//銷售紀錄
                RecordList = new TradeRecordDetails(result);
                DataTable resultDetail = TradeService.GetTradeRecordDetail(queryInfo);//銷售明細
                RecordDetailList = new TradeRecordDetails(resultDetail);
                DataTable resultSum = TradeService.GetTradeRecordSum(queryInfo);//銷售彙總
                RecordSumList = new TradeRecordDetails(resultSum);
                DataTable resultCusSum = TradeService.GetTradeRecordCusSum(queryInfo);//客戶彙總
                CustomSumList = new TradeRecordDetails(resultCusSum);
                ChangeUiTypeAction(string.Empty);
            }
            else
            {
                MessageWindow.ShowMessage("銷售日期未填寫!", MessageType.ERROR);
                return;
            }
        }
        /// <summary>
        /// 轉西元年
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        private string ConvertMaskedDate(string dateString)
        {
            string[] strArr = dateString.Split('/');
            string year = (int.Parse(strArr[0]) + 1911).ToString();
            string month = string.Format("{0:D2}", strArr[1]);
            string date = string.Format("{0:D2}", strArr[2]);
            return year + "-" + month + "-" + date;
        }
        /// <summary>
        /// 轉民國年
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static string ConvertTWDate(DateTime date)
        {
            string year = (date.Year - 1911).ToString();
            string month = date.Month.ToString().PadLeft(2, '0');
            string day = date.Day.ToString().PadLeft(2, '0');
            return year + "/" + month + "/" + day;
        }
        /// <summary>
        /// 清除搜尋條件
        /// </summary>
        private void ClearAction()
        {
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                StartDate = ConvertTWDate(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
                EndDate = ConvertTWDate(DateTime.Today);
                SInvoice = string.Empty;
                EInvoice = string.Empty;
                Emp = null;
                IsIrregular = false;
                IsReturn = false;
                SearchProductID = string.Empty;
                SearchProductName = string.Empty;
            });
        }
        private void DownloadInvoiceAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Id", null));
            parameters.Add(new SqlParameter("sDate", Convert.ToDateTime(ConvertMaskedDate(StartDate))));
            parameters.Add(new SqlParameter("eDate", Convert.ToDateTime(ConvertMaskedDate(EndDate))));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeProfitDetailEmpRecordByDate]", parameters);

            List<SqlParameter> parameters2 = new List<SqlParameter>();
            parameters2.Add(new SqlParameter("Id", "1"));
            parameters2.Add(new SqlParameter("sDate", Convert.ToDateTime(ConvertMaskedDate(StartDate))));
            DataTable result2 = MainWindow.ServerConnection.ExecuteProc("[GET].[InvoiceRecordByDate]", parameters2);
            MainWindow.ServerConnection.CloseConnection();
            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog
            {
                Title = "下載發票",
                InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath,
                Filter = "XLSX檔案|*.xlsx",
                FileName = Convert.ToDateTime(ConvertMaskedDate(StartDate)).ToString("yyyyMM") + "-" + "當月發票",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                #region 工作列1
                XLWorkbook wb = new XLWorkbook();
                IXLStyle style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                IXLWorksheet ws = wb.Worksheets.Add("發票明細");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                IXLColumn col = ws.Column("E");
                col.Width = 55;

                ws.Cell(1, 1).Value = "發票明細";
                ws.Range(1, 1, 1, 5).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "時間";
                ws.Cell("B2").Value = "發票號碼";
                ws.Cell("C2").Value = "發票金額";
                ws.Cell("D2").Value = "作廢發票號碼";
                ws.Cell("E2").Value = "作廢發票金額";

                if (result.Rows.Count > 0)
                {
                    IXLRange rangeWithData = ws.Cell(3, 1).InsertData(result.AsEnumerable());
                    ws.Columns().AdjustToContents();//欄位寬度根據資料調整
                    rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
                #endregion

                #region 工作列2
                ws = wb.Worksheets.Add("發票明細2");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);

                col = ws.Column("B");
                col.Width = 15;

                ws.Cell(1, 1).Value = "發票明細";
                ws.Range(1, 1, 1, 4).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "日期";
                ws.Cell("B2").Value = "發票金額";
                ws.Cell("C2").Value = "發票號碼";
                ws.Cell("D2").Value = "統一編號";
                ws.Cell("E2").Value = "作廢";

                if (result2.Rows.Count > 0)
                {
                    IXLRange rangeWithData = ws.Cell(3, 1).InsertData(result2.AsEnumerable());
                    rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
                #endregion

                try
                {
                    ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                    ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                    ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                    ws.Columns().AdjustToContents();//欄位寬度根據資料調整
                    wb.SaveAs(fdlg.FileName);
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.FileName = fdlg.FileName;
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
        private void ChangeUiTypeAction(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                SelectTab = Convert.ToInt32(type);
            }
            switch (SelectTab)
            {
                case 1: // 銷售紀錄
                    if (RecordList != null && RecordList.Count() > 0)
                    {
                        TotalCount = RecordList.Count();
                        TotalAmount = RecordList.Sum(s => s.TraMas_RealTotal);
                    }
                    break;

                case 2: // 銷售明細
                    if (RecordDetailList != null && RecordDetailList.Count() > 0)
                    {
                        TotalCount = RecordDetailList.Count();
                        TotalAmount = RecordDetailList.Sum(s => s.TraDet_PriceSum);
                    }
                    break;

                case 3: // 銷售彙總
                    if (RecordSumList != null && RecordSumList.Count() > 0)
                    {
                        TotalCount = RecordSumList.Count();
                        TotalAmount = RecordSumList.Sum(s => s.TraDet_PriceSum);
                    }
                    break;

                default:
                    break;
            }
        }
        public void ShowSaleRecord(int traMas_ID)
        {
            DataTable result = TradeService.GetTradeRecord(traMas_ID);
            if (result != null && result.Rows.Count > 0)
            {
                ProductTransactionDetail.ProductTransactionDetail ptd = new ProductTransactionDetail.ProductTransactionDetail(result.Rows[0], result);
                ptd.ShowDialog();
                ptd.Activate();
            }
        }
        private void PrintAction()
        {
            string fileTitle = string.Empty;
            switch (SelectTab)
            {
                case 1:
                    fileTitle = "銷售紀錄";
                    break;

                case 2:
                    fileTitle = "銷售明細";
                    break;

                case 3:
                    fileTitle = "銷售彙總";
                    break;

                case 4:
                    fileTitle = "顧客彙總";
                    break;

                default:
                    break;
            }
            if (!string.IsNullOrEmpty(fileTitle))
            {
                SaveFileDialog fdlg = new SaveFileDialog
                {
                    Title = fileTitle,
                    InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath,
                    Filter = "XLSX檔案|*.xlsx",
                    FileName = fileTitle,
                    FilterIndex = 2,
                    RestoreDirectory = true
                };

                if (fdlg.ShowDialog() != DialogResult.OK)
                    return;

                XLWorkbook workbook = new XLWorkbook();
                IXLStyle style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;
                IXLWorksheet worksheet = workbook.Worksheets.Add(fileTitle);
                worksheet.Style.Font.SetFontName("Arial").Font.SetFontSize(14);

                if (SelectTab == 1)
                {
                    worksheet.Cell(1, 1).Value = "銷售紀錄";
                    worksheet.Range(1, 1, 1, 7).Merge().AddToNamed("Titles");
                    worksheet.Cell("A2").Value = "結帳時間";
                    worksheet.Cell("B2").Value = "顧客姓名";
                    worksheet.Cell("C2").Value = "結帳金額";
                    worksheet.Cell("D2").Value = "付款方式";
                    worksheet.Cell("E2").Value = "統一編號";
                    worksheet.Cell("F2").Value = "發票號碼";
                    worksheet.Cell("G2").Value = "收銀員";
                }
                else if (SelectTab == 2)
                {
                    worksheet.Cell(1, 1).Value = "銷售明細";
                    worksheet.Range(1, 1, 1, 9).Merge().AddToNamed("Titles");
                    worksheet.Cell("A2").Value = "結帳時間";
                    worksheet.Cell("B2").Value = "顧客姓名";
                    worksheet.Cell("C2").Value = "商品代碼";
                    worksheet.Cell("D2").Value = "商品名稱";
                    worksheet.Cell("E2").Value = "售價";
                    worksheet.Cell("F2").Value = "數量";
                    worksheet.Cell("G2").Value = "小計";
                    worksheet.Cell("H2").Value = "獎勵";
                    worksheet.Cell("I2").Value = "收銀員";
                    worksheet.Cell("J2").Value = "發票號";
                }
                else if (SelectTab == 3)
                {
                    worksheet.Cell(1, 1).Value = fileTitle;
                    worksheet.Range(1, 1, 1, 4).Merge().AddToNamed("Titles");
                    worksheet.Cell("A2").Value = "商品代碼";
                    worksheet.Cell("B2").Value = "商品名稱";
                    worksheet.Cell("C2").Value = "數量";
                    worksheet.Cell("D2").Value = "總售價";
                    worksheet.Cell("E2").Value = "總成本";
                    worksheet.Cell("F2").Value = "總毛利";
                    worksheet.Cell("G2").Value = "毛利率";
                }
                else if (SelectTab == 4)
                {
                    worksheet.Cell(1, 1).Value = fileTitle;
                    worksheet.Range(1, 1, 1, 4).Merge().AddToNamed("Titles");
                    worksheet.Cell("A2").Value = "顧客姓名";
                    worksheet.Cell("B2").Value = "總金額";
                    worksheet.Cell("C2").Value = "總毛利";
                    worksheet.Cell("D2").Value = "毛利率";
                }
                try
                {
                    DataTable table = SetPrintStructure();
                    IXLRange rangeWithData = worksheet.Cell(3, 1).InsertData(table.AsEnumerable());

                    worksheet.Columns().AdjustToContents();
                    if (rangeWithData != null)
                    {
                        rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                        worksheet.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                        worksheet.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                        workbook.SaveAs(fdlg.FileName);

                        ConfirmWindow cw = new ConfirmWindow("是否開啟檔案?", "確認");
                        if ((bool)cw.DialogResult)
                        {
                            Process myProcess = new Process();
                            myProcess.StartInfo.UseShellExecute = true;
                            myProcess.StartInfo.FileName = fdlg.FileName;
                            myProcess.StartInfo.CreateNoWindow = true;
                            myProcess.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
        private DataTable SetPrintStructure()
        {
            DataTable table = new DataTable();
            switch (SelectTab)
            {
                case 1:
                    DataColumn dc1_1 = new DataColumn("TraMas_ChkoutTime", typeof(string));
                    DataColumn dc1_2 = new DataColumn("Cus_Name", typeof(string));
                    DataColumn dc1_3 = new DataColumn("TraMas_RealTotal", typeof(string));
                    DataColumn dc1_4 = new DataColumn("TraMas_PayMethod", typeof(string));
                    DataColumn dc1_5 = new DataColumn("TraMas_TaxNumber", typeof(string));
                    DataColumn dc1_6 = new DataColumn("TraMas_InvoiceNumber", typeof(string));
                    DataColumn dc1_7 = new DataColumn("Emp_Name", typeof(string));
                    table.Columns.Add(dc1_1);
                    table.Columns.Add(dc1_2);
                    table.Columns.Add(dc1_3);
                    table.Columns.Add(dc1_4);
                    table.Columns.Add(dc1_5);
                    table.Columns.Add(dc1_6);
                    table.Columns.Add(dc1_7);
                    foreach (TradeRecordDetail record in RecordList)
                    {
                        DataRow dr = table.NewRow();
                        dr["TraMas_ChkoutTime"] = record.TraMas_ChkoutTime.ToString("yyyy/MM/dd HH:mm");
                        dr["Cus_Name"] = record.Cus_Name;
                        dr["TraMas_RealTotal"] = record.TraMas_RealTotal;
                        dr["TraMas_PayMethod"] = record.TraMas_PayMethod;
                        dr["TraMas_TaxNumber"] = record.TraMas_TaxNumber;
                        dr["TraMas_InvoiceNumber"] = record.TraMas_InvoiceNumber;
                        dr["Emp_Name"] = record.Emp_Name;
                        table.Rows.Add(dr);
                    }
                    break;

                case 2:
                    DataColumn dc2_1 = new DataColumn("TraMas_ChkoutTime", typeof(string));
                    DataColumn dc2_2 = new DataColumn("Cus_Name", typeof(string));
                    DataColumn dc2_3 = new DataColumn("TraDet_ProductID", typeof(string));
                    DataColumn dc2_4 = new DataColumn("TraDet_ProductName", typeof(string));
                    DataColumn dc2_5 = new DataColumn("TraDet_Price", typeof(string));
                    DataColumn dc2_6 = new DataColumn("TraDet_Amount", typeof(string));
                    DataColumn dc2_7 = new DataColumn("TraDet_PriceSum", typeof(string));
                    DataColumn dc2_8 = new DataColumn("TraDet_RewardPersonnelName", typeof(string));
                    DataColumn dc2_9 = new DataColumn("Emp_Name", typeof(string));
                    DataColumn dc2_10 = new DataColumn("TraMas_InvoiceNumber", typeof(string));
                    table.Columns.Add(dc2_1);
                    table.Columns.Add(dc2_2);
                    table.Columns.Add(dc2_3);
                    table.Columns.Add(dc2_4);
                    table.Columns.Add(dc2_5);
                    table.Columns.Add(dc2_6);
                    table.Columns.Add(dc2_7);
                    table.Columns.Add(dc2_8);
                    table.Columns.Add(dc2_9);
                    table.Columns.Add(dc2_10);
                    foreach (TradeRecordDetail record in RecordDetailList)
                    {
                        DataRow dr = table.NewRow();
                        dr["TraMas_ChkoutTime"] = record.TraMas_ChkoutTime.ToString("yyyy/MM/dd HH:mm");
                        dr["Cus_Name"] = record.Cus_Name;
                        dr["TraDet_ProductID"] = record.TraDet_ProductID;
                        dr["TraDet_ProductName"] = record.TraDet_ProductName;
                        dr["TraDet_Price"] = record.TraDet_Price;
                        dr["TraDet_Amount"] = record.TraDet_Amount;
                        dr["TraDet_PriceSum"] = record.TraDet_PriceSum;
                        dr["TraDet_RewardPersonnelName"] = record.TraDet_RewardPersonnelName;
                        dr["Emp_Name"] = record.Emp_Name;
                        dr["TraMas_InvoiceNumber"] = record.TraMas_InvoiceNumber;
                        table.Rows.Add(dr);
                    }
                    break;

                case 3:
                    DataColumn dc3_1 = new DataColumn("TraDet_ProductID", typeof(string));
                    DataColumn dc3_2 = new DataColumn("TraDet_ProductName", typeof(string));
                    DataColumn dc3_3 = new DataColumn("TraDet_Amount", typeof(string));
                    DataColumn dc3_4 = new DataColumn("TraDet_PriceSum", typeof(string));
                    DataColumn dc3_5 = new DataColumn("TotalCost", typeof(string));
                    DataColumn dc3_6 = new DataColumn("Profit", typeof(string));
                    DataColumn dc3_7 = new DataColumn("ProfitPercent", typeof(string));
                    table.Columns.Add(dc3_1);
                    table.Columns.Add(dc3_2);
                    table.Columns.Add(dc3_3);
                    table.Columns.Add(dc3_4);
                    table.Columns.Add(dc3_5);
                    table.Columns.Add(dc3_6);
                    table.Columns.Add(dc3_7);

                    foreach (TradeRecordDetail record in RecordSumList)
                    {
                        DataRow dr = table.NewRow();
                        dr["TraDet_ProductID"] = record.TraDet_ProductID;
                        dr["TraDet_ProductName"] = record.TraDet_ProductName;
                        dr["TraDet_Amount"] = record.TraDet_Amount;
                        dr["TraDet_PriceSum"] = record.TraDet_PriceSum;
                        dr["TotalCost"] = record.TotalCost;
                        dr["Profit"] = record.Profit;
                        dr["ProfitPercent"] = record.ProfitPercent.ToString("P");
                        table.Rows.Add(dr);
                    }
                    break;

                case 4:
                    DataColumn dc4_1 = new DataColumn("Cus_Name", typeof(string));
                    DataColumn dc4_2 = new DataColumn("TraMas_RealTotal", typeof(string));
                    DataColumn dc4_3 = new DataColumn("Profit", typeof(string));
                    DataColumn dc4_4 = new DataColumn("ProfitPercent", typeof(string));
                    table.Columns.Add(dc4_1);
                    table.Columns.Add(dc4_2);
                    table.Columns.Add(dc4_3);
                    table.Columns.Add(dc4_4);
                    foreach (TradeRecordDetail record in CustomSumList)
                    {
                        DataRow dr = table.NewRow();
                        dr["Cus_Name"] = record.Cus_Name;
                        dr["TraMas_RealTotal"] = record.TraMas_RealTotal;
                        dr["Profit"] = record.Profit;
                        dr["ProfitPercent"] = record.ProfitPercent.ToString("P");
                        table.Rows.Add(dr);
                    }
                    break;

                default:
                    break;
            }
            return table;
        }
    }
}