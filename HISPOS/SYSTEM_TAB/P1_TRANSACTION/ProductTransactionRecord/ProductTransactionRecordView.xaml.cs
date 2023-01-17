using ClosedXML.Excel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using His_Pos.NewClass.Trade;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionRecord
{
    /// <summary>
    /// ProductTransactionRecordView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTransactionRecordView : System.Windows.Controls.UserControl
    {
        public DataTable RecordList;
        public DataTable RecordDetailList;
        public DataTable RecordSumList;
        public DataTable RecordPrint;
        public DataTable RecordDetailListPrint;
        private int queryTab = 1;

        private TradeService _tradeService = new TradeService();
        public ProductTransactionRecordView()
        {
            InitializeComponent();
            StartDate.Value = GetDefaultDate();
            EndDate.Value = GetDefaultDate();
            RecordList = new DataTable();
            GetEmployeeList();
        }

        private void ClearDataGrids()
        {
            RecordGrid.ItemsSource = null;
            RecordDetailGrid.ItemsSource = null;
            RecordSumGrid.ItemsSource = null;
        }

        private void GetEmployeeList()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[GetEmployee]");
            MainWindow.ServerConnection.CloseConnection();
            cbCashier.ItemsSource = result.DefaultView;
        }

        private int GetRowIndex(MouseButtonEventArgs e)
        {
            DataGridRow dgr = null;
            DependencyObject visParent = VisualTreeHelper.GetParent(e.OriginalSource as FrameworkElement);
            while (dgr == null && visParent != null)
            {
                dgr = visParent as DataGridRow;
                visParent = VisualTreeHelper.GetParent(visParent);
            }
            if (dgr == null) { return -1; }
            int rowIdx = dgr.GetIndex();
            return rowIdx;
        }

        private string GetDefaultDate()
        {
            DateTime dt = DateTime.Today;
            TaiwanCalendar tc = new TaiwanCalendar();
            string dts = string.Format("{0}/{1}/{2}",
                tc.GetYear(dt),
                dt.Month.ToString("d2"),
                dt.Day.ToString("d2")
                );
            return dts;
        }

        private void GetData(int querytype)
        {
            GetDefaultDate();
            if (StartDate.Text.Contains("-") || EndDate.Text.Contains("-")) { return; }
            string sDate = ConvertMaskedDate(StartDate.Text);
            string eDate = ConvertMaskedDate(EndDate.Text);
            bool isIrregular = chkIsIrregular.IsChecked.Value;
            bool isReturn = chkIsReturn.IsChecked.Value;
            string sInvoice = StartInvoice.Text;
            string eInvoice = EndInvoice.Text;
            int Cashier;
            string proID = ProId.Text;
            string proNAME = ProName.Text;
            if (cbCashier.SelectedValue == null)
            {
                Cashier = -1;
            }
            else
            {
                Cashier = (int)cbCashier.SelectedValue;
            }
            if (sInvoice.Length == 8 || sInvoice.Length == 0 || sInvoice == "" || int.TryParse(sInvoice, out int S))
            {
            }
            else
            {
                MessageWindow.ShowMessage("搜尋發票號碼必須為8位數字!", MessageType.ERROR);
                return;
            }
            if (eInvoice.Length == 8 || eInvoice.Length == 0 || eInvoice == "" || int.TryParse(eInvoice, out int E))
            {
            }
            else
            {
                MessageWindow.ShowMessage("搜尋發票號碼必須為8位數字!", MessageType.ERROR);
                return;
            }

            TradeQueryInfo queryInfo = new TradeQueryInfo()
            {
                StartDate = sDate,
                EndDate = eDate,
                StartInvoice = sInvoice,
                EndInvoice = eInvoice,
                ShowIrregular = isIrregular,
                ShowReturn = isReturn,
                CashierID = Cashier,
                ProID = proID,
                ProName = proNAME,
                Flag = "0"
            };

            switch (querytype)
            {
                case 1: // 銷售紀錄
                default:
                    queryTab = 1;
                    ClearDataGrids();

                    DataTable result = _tradeService.GetTradeRecord(queryInfo);
                    FormatData(result);
                    RecordList = result.Copy();
                    RecordList.Columns.Add("NO"); // 序
                    int countRL = 1;
                    foreach (DataRow dr in RecordList.Rows)
                    {
                        dr["NO"] = countRL.ToString();
                        countRL += 1;
                    }
                    RecordGrid.ItemsSource = RecordList.DefaultView;
                    lblCount.Content = RecordList.Rows.Count;
                    lblTotal.Content = "(含折扣):" + RecordList.Compute("Sum(TraMas_RealTotal)", string.Empty);
                    if (RecordList.Rows.Count == 0) { MessageWindow.ShowMessage("查無資料", MessageType.WARNING); }
                    break;

                case 2: // 銷售明細
                    queryTab = 2;
                    ClearDataGrids();
                    DataTable resultDetail = _tradeService.GetTradeRecordDetail(queryInfo);
                    FormatData(resultDetail);
                    RecordDetailList = resultDetail.Copy();

                    RecordDetailList.Columns.Add("NO"); // 序
                    int countDL = 1;
                    foreach (DataRow dr in RecordDetailList.Rows)
                    {
                        dr["NO"] = countDL.ToString();
                        countDL += 1;
                    }
                    RecordDetailGrid.ItemsSource = RecordDetailList.DefaultView;

                    lblCount.Content = RecordDetailList.Rows.Count;
                    lblTotal.Content = "(不含折扣):" + RecordDetailList.Compute("Sum(TraDet_PriceSum)", string.Empty);
                    break;

                case 3: // 銷售彙總
                    queryTab = 3;
                    ClearDataGrids();
                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> parametersSum = new List<SqlParameter>();
                    parametersSum.Add(new SqlParameter("CustomerID", DBNull.Value));
                    parametersSum.Add(new SqlParameter("MasterID", DBNull.Value));
                    parametersSum.Add(new SqlParameter("sDate", sDate));
                    parametersSum.Add(new SqlParameter("eDate", eDate));
                    parametersSum.Add(new SqlParameter("sInvoice", sInvoice));
                    parametersSum.Add(new SqlParameter("eInvoice", eInvoice));
                    parametersSum.Add(new SqlParameter("flag", "0"));
                    parametersSum.Add(new SqlParameter("ShowIrregular", isIrregular));
                    parametersSum.Add(new SqlParameter("ShowReturn", isReturn));
                    parametersSum.Add(new SqlParameter("Cashier", Cashier));
                    parametersSum.Add(new SqlParameter("ProID", proID));
                    parametersSum.Add(new SqlParameter("ProName", proNAME));
                    DataTable resultSum = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordSum]", parametersSum);
                    MainWindow.ServerConnection.CloseConnection();
                    RecordSumList = resultSum.Copy();

                    RecordSumList.Columns.Add("NO"); // 序
                    int countSUM = 1;
                    foreach (DataRow dr in RecordSumList.Rows)
                    {
                        dr["NO"] = countSUM.ToString();
                        countSUM += 1;
                    }
                    RecordSumGrid.ItemsSource = RecordSumList.DefaultView;
                    break;

                case 4: // 銷售紀錄(列印)
                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> parametersPrint = new List<SqlParameter>();
                    parametersPrint.Add(new SqlParameter("CustomerID", DBNull.Value));
                    parametersPrint.Add(new SqlParameter("MasterID", DBNull.Value));
                    parametersPrint.Add(new SqlParameter("sDate", sDate));
                    parametersPrint.Add(new SqlParameter("eDate", eDate));
                    parametersPrint.Add(new SqlParameter("sInvoice", sInvoice));
                    parametersPrint.Add(new SqlParameter("eInvoice", eInvoice));
                    parametersPrint.Add(new SqlParameter("flag", "0"));
                    parametersPrint.Add(new SqlParameter("ShowIrregular", isIrregular));
                    parametersPrint.Add(new SqlParameter("ShowReturn", isReturn));
                    parametersPrint.Add(new SqlParameter("Cashier", Cashier));
                    parametersPrint.Add(new SqlParameter("ProID", proID));
                    parametersPrint.Add(new SqlParameter("ProName", proNAME));

                    DataTable resultprint = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQueryPrint]", parametersPrint);
                    MainWindow.ServerConnection.CloseConnection();
                    RecordPrint = resultprint.Copy();
                    break;

                case 5: // 銷售明細(列印)
                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> parametersDetailPrint = new List<SqlParameter>();
                    parametersDetailPrint.Add(new SqlParameter("CustomerID", DBNull.Value));
                    parametersDetailPrint.Add(new SqlParameter("MasterID", DBNull.Value));
                    parametersDetailPrint.Add(new SqlParameter("sDate", sDate));
                    parametersDetailPrint.Add(new SqlParameter("eDate", eDate));
                    parametersDetailPrint.Add(new SqlParameter("sInvoice", sInvoice));
                    parametersDetailPrint.Add(new SqlParameter("eInvoice", eInvoice));
                    parametersDetailPrint.Add(new SqlParameter("flag", "0"));
                    parametersDetailPrint.Add(new SqlParameter("ShowIrregular", isIrregular));
                    parametersDetailPrint.Add(new SqlParameter("ShowReturn", isReturn));
                    parametersDetailPrint.Add(new SqlParameter("Cashier", Cashier));
                    parametersDetailPrint.Add(new SqlParameter("ProID", proID));
                    parametersDetailPrint.Add(new SqlParameter("ProName", proNAME));
                    DataTable resultDetailPrint = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordDetailQueryPrint]", parametersDetailPrint);
                    MainWindow.ServerConnection.CloseConnection();

                    RecordDetailListPrint = resultDetailPrint.Copy();
                    break;
            }
        }

        private void FormatData(DataTable result)
        {
            result.Columns.Add("TransTime_Format", typeof(string));
            foreach (DataRow dr in result.Rows)
            {
                string ogTransTime = dr["TraMas_ChkoutTime"].ToString();
                DateTime dTime = DateTime.Parse(ogTransTime);
                string formatTransTime = dTime.ToString("yyyy-MM-dd HH:mm");
                dr["TransTime_Format"] = formatTransTime;
            }
        }

        private string ConvertMaskedDate(string dateString)
        {
            string[] strArr = dateString.Split('/');
            string year = (int.Parse(strArr[0]) + 1911).ToString();
            string month = string.Format("{0:D2}", strArr[1]);
            string date = string.Format("{0:D2}", strArr[2]);
            return year + "-" + month + "-" + date;
        }

        private void ShowSelectedDetailWindow(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            string TradeID;
            DataRow masterRow;
            if (RecordGrid.Visibility == Visibility.Visible)
            {
                masterRow = RecordList.Rows[index];
                TradeID = RecordList.Rows[index]["TraMas_ID"].ToString();
            }
            else
            {
                masterRow = RecordDetailList.Rows[index];
                TradeID = RecordDetailList.Rows[index]["TraMas_ID"].ToString();
            }



            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MasterID", TradeID));
            parameters.Add(new SqlParameter("CustomerID", DBNull.Value));
            parameters.Add(new SqlParameter("sDate", ""));
            parameters.Add(new SqlParameter("eDate", ""));
            parameters.Add(new SqlParameter("sInvoice", ""));
            parameters.Add(new SqlParameter("eInvoice", ""));
            parameters.Add(new SqlParameter("flag", "1"));
            parameters.Add(new SqlParameter("ShowIrregular", DBNull.Value));
            parameters.Add(new SqlParameter("ShowReturn", DBNull.Value));
            parameters.Add(new SqlParameter("Cashier", -1));
            parameters.Add(new SqlParameter("ProID", DBNull.Value));
            parameters.Add(new SqlParameter("ProName", DBNull.Value));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            ProductTransactionDetail.ProductTransactionDetail ptd = new ProductTransactionDetail.ProductTransactionDetail(masterRow, result);
            ptd.Closed += DetailWindowClosed;
            ptd.ShowDialog();
            ptd.Activate();
        }

        public void DetailWindowClosed(object sender, EventArgs e)
        {
            GetData(queryTab);
        }

        private void StartDate_OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                EndDate.Focus();
                EndDate.SelectionStart = 0;
            }
        }

        private void EndDate_OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
            }
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            GetData(queryTab);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ClearDataGrids();
                StartInvoice.Text = "";
                EndInvoice.Text = "";
                cbCashier.SelectedIndex = -1;
            }), DispatcherPriority.ContextIdle);
        }

        private void btnTrade_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GetData(1);
            btnTradeDetail.Background = Brushes.Transparent;
            btnTradeDetail.Foreground = Brushes.DimGray;
            btnTrade.Foreground = Brushes.White;
            btnTrade.Background = Brushes.DimGray;
            RecordGrid.Visibility = Visibility.Visible;
            RecordDetailGrid.Visibility = Visibility.Collapsed;

            btnTradeSum.Foreground = Brushes.DimGray;
            btnTradeSum.Background = Brushes.Transparent;
            RecordSumGrid.Visibility = Visibility.Collapsed;

            btnPrint1.Visibility = Visibility.Visible;
            btnPrint2.Visibility = Visibility.Collapsed;
            btnPrint3.Visibility = Visibility.Collapsed;
        }

        private void btnTradeDetail_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GetData(2);
            btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnTradeDetail.Foreground = Brushes.White;
            btnTradeDetail.Background = Brushes.DimGray;
            RecordGrid.Visibility = Visibility.Collapsed;
            RecordDetailGrid.Visibility = Visibility.Visible;

            btnTradeSum.Foreground = Brushes.DimGray;
            btnTradeSum.Background = Brushes.Transparent;
            RecordSumGrid.Visibility = Visibility.Collapsed;
            btnPrint1.Visibility = Visibility.Collapsed;
            btnPrint2.Visibility = Visibility.Visible;
            btnPrint3.Visibility = Visibility.Collapsed;
        }

        private void btnTradeSum_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GetData(3);
            btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnTradeDetail.Foreground = Brushes.DimGray;
            btnTradeDetail.Background = Brushes.Transparent;
            RecordGrid.Visibility = Visibility.Collapsed;
            RecordDetailGrid.Visibility = Visibility.Collapsed;

            btnTradeSum.Foreground = Brushes.White;
            btnTradeSum.Background = Brushes.DimGray;
            RecordSumGrid.Visibility = Visibility.Visible;
            btnPrint1.Visibility = Visibility.Collapsed;
            btnPrint2.Visibility = Visibility.Collapsed;
            btnPrint3.Visibility = Visibility.Visible;
        }

        private void btnPrint1_Click(object sender, RoutedEventArgs e)
        {
            GetData(4);
            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "銷售紀錄";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = "銷售紀錄";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add("銷售紀錄");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);

                var col1 = ws.Column("A");
                col1.Width = 20;
                var col2 = ws.Column("B");
                col2.Width = 10;
                var col3 = ws.Column("C");
                col3.Width = 10;
                var col4 = ws.Column("D");
                col4.Width = 10;
                var col5 = ws.Column("E");
                col5.Width = 10;
                var col6 = ws.Column("F");
                col6.Width = 10;
                var col7 = ws.Column("G");
                col7.Width = 10;
                ws.Cell(1, 1).Value = "銷售紀錄";
                ws.Range(1, 1, 1, 7).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "結帳時間";
                ws.Cell("B2").Value = "顧客姓名";
                ws.Cell("C2").Value = "結帳金額";
                ws.Cell("D2").Value = "付款方式";
                ws.Cell("E2").Value = "統一編號";
                ws.Cell("F2").Value = "發票號碼";
                ws.Cell("G2").Value = "收銀員";

                var rangeWithData = ws.Cell(3, 1).InsertData(RecordPrint.AsEnumerable());

                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                //wb.SaveAs(fdlg.FileName);
                try
                {
                    wb.SaveAs(fdlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = (fdlg.FileName);
                myProcess.StartInfo.CreateNoWindow = true;
                //myProcess.StartInfo.Verb = "print";
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
            }
        }

        private void btnPrint2_Click(object sender, RoutedEventArgs e)
        {
            GetData(5);
            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "銷售明細";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = "銷售明細";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add("銷售明細");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                var col1 = ws.Column("A");
                col1.Width = 20;
                var col2 = ws.Column("B");
                col2.Width = 10;
                var col3 = ws.Column("C");
                col3.Width = 10;
                var col4 = ws.Column("D");
                col4.Width = 10;
                var col5 = ws.Column("E");
                col5.Width = 10;
                var col6 = ws.Column("F");
                col6.Width = 10;
                var col7 = ws.Column("G");
                col7.Width = 10;
                var col8 = ws.Column("H");
                col8.Width = 10;
                var col9 = ws.Column("I");
                col9.Width = 10;
                var col10 = ws.Column("J");
                col10.Width = 30;
                ws.Cell(1, 1).Value = "銷售明細";
                ws.Range(1, 1, 1, 9).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "結帳時間";
                ws.Cell("B2").Value = "顧客姓名";
                ws.Cell("C2").Value = "商品代碼";
                ws.Cell("D2").Value = "商品名稱";
                ws.Cell("E2").Value = "售價";
                ws.Cell("F2").Value = "數量";
                ws.Cell("G2").Value = "小計";
                ws.Cell("H2").Value = "獎勵";
                ws.Cell("I2").Value = "收銀員";
                ws.Cell("J2").Value = "發票號";
                var rangeWithData = ws.Cell(3, 1).InsertData(RecordDetailListPrint.AsEnumerable());

                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                //wb.SaveAs(fdlg.FileName);
                try
                {
                    wb.SaveAs(fdlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = (fdlg.FileName);
                myProcess.StartInfo.CreateNoWindow = true;
                //myProcess.StartInfo.Verb = "print";
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
            }
        }

        private void btnPrint3_Click(object sender, RoutedEventArgs e)
        {
            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "銷售彙總";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = "銷售彙總";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
               
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add("銷售彙總");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);

                var col1 = ws.Column("A");
                col1.Width = 20;
                var col2 = ws.Column("B");
                col2.Width = 33;
                var col3 = ws.Column("C");
                col3.Width = 10;
                var col4 = ws.Column("D");
                col4.Width = 10;

                ws.Cell(1, 1).Value = "銷售彙總";
                ws.Range(1, 1, 1, 4).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "商品代碼";
                ws.Cell("B2").Value = "商品名稱";
                ws.Cell("C2").Value = "數量";
                ws.Cell("D2").Value = "總售價";

                var tempRecordSumList = RecordSumList.Copy();
                tempRecordSumList.Columns.Remove("NO"); 
                var rangeWithData = ws.Cell(3, 1).InsertData(tempRecordSumList.AsEnumerable());

                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                //wb.SaveAs(fdlg.FileName);
                try
                {
                    wb.SaveAs(fdlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = (fdlg.FileName);
                myProcess.StartInfo.CreateNoWindow = true;
                //myProcess.StartInfo.Verb = "print";
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
            }
        }


    }
}