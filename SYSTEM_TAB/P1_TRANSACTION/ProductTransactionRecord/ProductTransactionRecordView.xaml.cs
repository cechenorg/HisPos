using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionRecord
{
    /// <summary>
    /// ProductTransactionRecordView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTransactionRecordView : UserControl
    {
        public DataTable RecordList;
        public DataTable RecordDetailList;
        public DataTable RecordSumList;

        public ProductTransactionRecordView()
        {
            InitializeComponent();
            StartDate.Value = GetDefaultDate();
            EndDate.Value = GetDefaultDate();
            RecordList = new DataTable();
            GetEmployeeList();


            MainWindow.ServerConnection.OpenConnection();
            DataTable resultdetail = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordDetailQuery]");
            MainWindow.ServerConnection.CloseConnection();
            FormatData(resultdetail);
            RecordDetailList = resultdetail.Copy();
            RecordDetailGrid.ItemsSource = RecordDetailList.DefaultView;
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

        private void GetData()
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
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CustomerID", DBNull.Value));
            parameters.Add(new SqlParameter("MasterID", DBNull.Value));
            parameters.Add(new SqlParameter("sDate", sDate));
            parameters.Add(new SqlParameter("eDate", eDate));
            parameters.Add(new SqlParameter("sInvoice", sInvoice));
            parameters.Add(new SqlParameter("eInvoice", eInvoice));
            parameters.Add(new SqlParameter("flag", "0"));
            parameters.Add(new SqlParameter("ShowIrregular", isIrregular));
            parameters.Add(new SqlParameter("ShowReturn", isReturn));
            parameters.Add(new SqlParameter("Cashier", Cashier));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            FormatData(result);
            RecordList = result.Copy();
            RecordGrid.ItemsSource = RecordList.DefaultView;
            lblCount.Content = RecordList.Rows.Count;
            lblTotal.Content = RecordList.Compute("Sum(TraMas_RealTotal)", string.Empty);
            if (RecordList.Rows.Count == 0) { MessageWindow.ShowMessage("查無資料", MessageType.WARNING); }

            MainWindow.ServerConnection.OpenConnection();
            DataTable resultdetail = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordDetailQuery]");
            MainWindow.ServerConnection.CloseConnection();
            FormatData(resultdetail);
            RecordDetailList = resultdetail.Copy();
            RecordDetailGrid.ItemsSource = RecordDetailList.DefaultView;



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
            DataTable resultSum = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordSum]", parametersSum);
            MainWindow.ServerConnection.CloseConnection();
            RecordSumList = resultSum.Copy();
            RecordSumGrid.ItemsSource = RecordSumList.DefaultView;

            
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
            DataRow masterRow = RecordList.Rows[index];
            string TradeID = RecordList.Rows[index]["TraMas_ID"].ToString();

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
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            ProductTransactionDetail.ProductTransactionDetail ptd = new ProductTransactionDetail.ProductTransactionDetail(masterRow, result);
            ptd.Closed += DetailWindowClosed;
            ptd.ShowDialog();
            ptd.Activate();
        }

        public void DetailWindowClosed(object sender, System.EventArgs e)
        {
            GetData();
        }

        private void StartDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                EndDate.Focus();
                EndDate.SelectionStart = 0;
            }
        }

        private void EndDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
            }
        }

        private void btnQuery_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GetData();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            RecordList.Clear();
            StartInvoice.Text = "";
            EndInvoice.Text = "";
            cbCashier.SelectedIndex = -1;
        }

        private void btnTrade_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            btnTradeDetail.Background = Brushes.Transparent;
            btnTradeDetail.Foreground = Brushes.DimGray;
            btnTrade.Foreground = Brushes.White;
            btnTrade.Background = Brushes.DimGray;
            RecordGrid.Visibility = Visibility.Visible;
            RecordDetailGrid.Visibility = Visibility.Collapsed;

            btnTradeSum.Foreground = Brushes.DimGray;
            btnTradeSum.Background = Brushes.Transparent;
            RecordSumGrid.Visibility = Visibility.Collapsed;

        }

        private void btnTradeDetail_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnTradeDetail.Foreground = Brushes.White;
            btnTradeDetail.Background = Brushes.DimGray;
            RecordGrid.Visibility = Visibility.Collapsed;
            RecordDetailGrid.Visibility = Visibility.Visible;

            btnTradeSum.Foreground = Brushes.DimGray;
            btnTradeSum.Background = Brushes.Transparent;
            RecordSumGrid.Visibility = Visibility.Collapsed;
        }

        private void btnTradeSum_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnTradeDetail.Foreground = Brushes.DimGray;
            btnTradeDetail.Background = Brushes.Transparent;
            RecordGrid.Visibility = Visibility.Collapsed;
            RecordDetailGrid.Visibility = Visibility.Collapsed;

            btnTradeSum.Foreground = Brushes.White;
            btnTradeSum.Background = Brushes.DimGray;
            RecordSumGrid.Visibility = Visibility.Visible;
        }
    }
}
