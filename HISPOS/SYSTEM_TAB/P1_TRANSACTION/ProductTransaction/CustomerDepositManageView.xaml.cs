using His_Pos.Class;
using His_Pos.FunctionWindow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using Microsoft.Reporting.WinForms;
using System.Text;
using System.Drawing.Imaging;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    /// <summary>
    /// CustomerDepositManageView.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerDepositManageView : Window
    {
        private string cus;
        private DataTable detail;
        private DataTable depositTable;

        public CustomerDepositManageView(string cusID)
        {
            cus = cusID;
            InitializeComponent();
            detail = new DataTable();
            depositTable = new DataTable();
            CustomerDepositRecord();
            CustomerDepositManage();
        }

        public void CustomerDepositRecord()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TraMas_CustomerID", cus));
            detail = MainWindow.ServerConnection.ExecuteProc("[POS].[DepositRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            detail.Columns.Add("TransTime_Format", typeof(string));
            foreach (DataRow dr in detail.Rows)
            {
                string ogTransTime = dr["DepRec_RecTime"].ToString();
                DateTime dt = DateTime.Parse(ogTransTime);
                CultureInfo culture = new CultureInfo("zh-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                dr["TransTime_Format"] = dt.ToString("yyy/MM/dd", culture);
            }
            ProductDepositRecordDataGrid.ItemsSource = detail.DefaultView;
        }

        public void CustomerDepositManage()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TraMas_CustomerID", cus));
            depositTable = MainWindow.ServerConnection.ExecuteProc("[POS].[DepositDetailQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            ProductDepositDataGrid.ItemsSource = depositTable.DefaultView;
        }
        public IEnumerable<ReportParameter> CreateContentParameter()
        {
            string[] sDetail = PrintDetail();
            return new List<ReportParameter>
            {
                new ReportParameter("CustusName",Convert.ToString(depositTable.Rows[0]["Cus_Name"])),
                new ReportParameter("Date", DateTime.Today.ToString("yyyy/MM/dd")),
                new ReportParameter("Medicine", sDetail[0]),
                new ReportParameter("Num", sDetail[1]),
            };
        }
        private string[] PrintDetail()
        {
            string[] sDetail = new string[3];

            foreach(DataRow dr in depositTable.Rows)
            {
                sDetail[0] += Convert.ToString(dr["Pro_ChineseName"]) + "\r\n";
                sDetail[1] += (Convert.ToInt32(dr["TraDet_DepositAmount"]) - Convert.ToInt32(dr["Amount"]))  + "\r\n";
            }
            return sDetail;
        }

        private void btnWithdraw_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dr in depositTable.Rows)
            {
                if ((int)dr["Amount"] > (int)dr["TraDet_DepositAmount"])
                {
                    MessageWindow.ShowMessage("提取量不得大於寄庫量", MessageType.WARNING);
                    return;
                }

                if ((int)dr["Amount"] < 0)
                {
                    MessageWindow.ShowMessage("提取量不得為負", MessageType.WARNING);
                    return;
                }
            }

            bool isSuccess = false;
            foreach (DataRow dr in depositTable.Rows)
            {
                if ((int)dr["Amount"] != 0)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("TraDet_ProductID", dr["TraDet_ProductID"]));
                    parameters.Add(new SqlParameter("Amount", dr["Amount"]));
                    parameters.Add(new SqlParameter("cusID", cus));
                    DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[DepositBuckles]", parameters);
                    MainWindow.ServerConnection.CloseConnection();
                    if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                    {
                        MessageWindow.ShowMessage("提取成功！", MessageType.SUCCESS);
                        isSuccess = true;
                        Close();
                    }
                    else { MessageWindow.ShowMessage("提取失敗！", MessageType.ERROR); }
                }
            }
            if(isSuccess && depositTable != null && depositTable.Rows.Count > 0)
            {
                ConfirmWindow cw = new ConfirmWindow("是否列印明細", "確認");//(20220420完成提取詢問是否列印明細)
                if ((bool)cw.DialogResult)
                {
                    var rptViewer = new ReportViewer();
                    rptViewer.LocalReport.DataSources.Clear();
                    rptViewer.LocalReport.ReportPath = @"RDLC\DepositRecord.rdlc";
                    rptViewer.PrinterSettings.PrinterName = Properties.Settings.Default.ReceiptPrinter;
                    rptViewer.LocalReport.Refresh();
                    rptViewer.ProcessingMode = ProcessingMode.Local;
                    var parameter = CreateContentParameter();
                    rptViewer.LocalReport.SetParameters(parameter);
                    MainWindow.Instance.Dispatcher.Invoke(() =>
                    {
                        ((VM)MainWindow.Instance.DataContext).StartPrintDeposit(rptViewer);
                    });
                }
            }
        }

        private void ProductAmountTextbox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            e.Handled = true;
            textBox.Focus();
        }

        private void ProductAmountTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            textBox.SelectAll();
        }
    }
}