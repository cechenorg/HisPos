using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.NewIncomeStatement
{
    /// <summary>
    /// NewIncomeStatementView.xaml 的互動邏輯
    /// </summary>
    public partial class NewIncomeStatementView : UserControl
    {
        private BackgroundWorker bgWorker = new BackgroundWorker();
        private int year;

        public NewIncomeStatementView()
        {
            InitializeComponent();
            tbYear.Text = DateTime.Now.Year.ToString();
            InitBackgroundWorker();
            InitData();
        }

        private void InitBackgroundWorker()
        {
            bgWorker.DoWork += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    bi.BusyContent = "取得損益資料...";
                }));
                GetData(year);
            };

            bgWorker.RunWorkerCompleted += (sender, args) =>
            {
                bi.IsBusy = false;
            };
        }

        private void InitData()
        {
            int.TryParse(tbYear.Text, out year);

            if (year > 2000 && year < 2100)
            {
                bi.IsBusy = true;
                if (!bgWorker.IsBusy)
                {
                    bgWorker.RunWorkerAsync();
                }
            }
            else
            {
                MessageWindow.ShowMessage("查詢範圍有誤", MessageType.ERROR);
            }
        }

        private void GetData(int year)
        {
            MainWindow.ServerConnection.OpenConnection();

            List<SqlParameter> paraCount = new List<SqlParameter>();
            paraCount.Add(new SqlParameter("YEAR", year));
            List<SqlParameter> paraIncome = new List<SqlParameter>();
            paraIncome.Add(new SqlParameter("YEAR", year));
            List<SqlParameter> paraExpanse = new List<SqlParameter>();
            paraExpanse.Add(new SqlParameter("YEAR", year));
            List<SqlParameter> paraClosed = new List<SqlParameter>();
            paraClosed.Add(new SqlParameter("YEAR", year));

            DataTable count = MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionCountByYear]", paraCount);
            DataTable income = MainWindow.ServerConnection.ExecuteProc("[Get].[IncomeByYear]", paraIncome);
            DataTable expanse = MainWindow.ServerConnection.ExecuteProc("[Get].[ExpanseByYear]", paraExpanse);
            DataTable closed = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsClosedByYear]", paraClosed);

            foreach (DataRow dr in closed.Rows)
            {
                bool isZero = true;
                foreach (DataColumn dc in closed.Columns)
                {
                    bool isInt = int.TryParse(dr[dc].ToString(), out int value);
                    if (isInt && value != 0)
                    {
                        isZero = false;
                    }
                }
                if (isZero)
                {
                    dr.Delete();
                }
            }
            closed.AcceptChanges();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                dgPreCount.ItemsSource = count.DefaultView;
                dgIncome.ItemsSource = income.DefaultView;
                dgExpanse.ItemsSource = expanse.DefaultView;
                dgDiff.ItemsSource = closed.DefaultView;
            }));

            MainWindow.ServerConnection.CloseConnection();

            DataTable total = income.Clone();

            if ((income.Rows.Count > 0) && (expanse.Rows.Count > 0) && (closed.Rows.Count > 0))
            {
                total.Rows.Add(income.Rows[income.Rows.Count - 1].ItemArray);
                total.Rows[0][0] = "營業毛利";
                total.Rows.Add(expanse.Rows[expanse.Rows.Count - 1].ItemArray);
                total.Rows[1][0] = "費用";
                total.Rows.Add(closed.Rows[closed.Rows.Count - 1].ItemArray);
                total.Rows[2][0] = "結案差額";

                DataRow totalRow = total.NewRow();
                totalRow["MONTH"] = "總計";
                totalRow["JAN"] = total.Compute("Sum(JAN)", string.Empty);
                totalRow["FEB"] = total.Compute("Sum(FEB)", string.Empty);
                totalRow["MAR"] = total.Compute("Sum(MAR)", string.Empty);
                totalRow["APR"] = total.Compute("Sum(APR)", string.Empty);
                totalRow["MAY"] = total.Compute("Sum(MAY)", string.Empty);
                totalRow["JUN"] = total.Compute("Sum(JUN)", string.Empty);
                totalRow["JUL"] = total.Compute("Sum(JUL)", string.Empty);
                totalRow["AUG"] = total.Compute("Sum(AUG)", string.Empty);
                totalRow["SEP"] = total.Compute("Sum(SEP)", string.Empty);
                totalRow["OCT"] = total.Compute("Sum(OCT)", string.Empty);
                totalRow["NOV"] = total.Compute("Sum(NOV)", string.Empty);
                totalRow["DEC"] = total.Compute("Sum(DEC)", string.Empty);
                totalRow["TOTAL"] = total.Compute("Sum(TOTAL)", string.Empty);
                total.Rows.Add(totalRow);
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                dgTotal.ItemsSource = total.DefaultView;
                dgTotal.Items.Refresh();
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitData();
        }

        private void dg_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - e.Delta);
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            year -= 1;
            tbYear.Text = year.ToString();
            InitData();
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            year += 1;
            tbYear.Text = year.ToString();
            InitData();
        }

        private void tbYear_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        private void tbYear_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InitData();
            }
        }

        private void ExportExcelCommand(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog();
            diag.FileName = year +"損益表.csv";
            diag.Filter = "csv (*.csv)|*.csv";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                string filePath = diag.FileName;

                string result = "";
                result += GetDataViewData(dgTotal.ItemsSource as DataView); 
                result += GetDataViewData(dgPreCount.ItemsSource as DataView);
                result += GetDataViewData(dgIncome.ItemsSource as DataView);
                result += GetDataViewData(dgExpanse.ItemsSource as DataView);
                result += GetDataViewData(dgDiff.ItemsSource as DataView);

                File.WriteAllText(filePath, result,Encoding.Unicode);
            }
        }

        private string GetDataViewData(DataView grid)
        {
            string result = "\t1月\t2月\t3月\t4月\t5月\t6月\t7月\t8月\t9月\t10月\t11月\t12月\t總計\r\n";
            foreach (DataRow row in (grid as DataView).ToTable().Rows)
            {

                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    result += row.ItemArray[i];

                    if (i < row.ItemArray.Length - 1)
                        result += "\t";
                }
                result += "\r\n";
            }

            result += "\r\n";
            return result;
        }
    }
}