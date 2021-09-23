using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;
using His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet
{
    /// <summary>
    /// BalanceSheetView.xaml 的互動邏輯
    /// </summary>
    public partial class StrikeManageView : UserControl
    {
        private DataTable debitAccList = new DataTable();
        private DataTable creditAccList = new DataTable();
        private DataTable transferAccList = new DataTable();

        private DataTable dgDetails = new DataTable();

        private int count = 0;
        private int subtotal = 0;
        private BalanceSheetTypeEnum BalanceSheetType;

        public StrikeManageView()
        {
            InitializeComponent();
            InitView();
        }

        private void InitView()
        {
            GetAccountList();
            SetCombobox();
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

        private int GetRowIndexRouted(RoutedEventArgs e)
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

        private void GetAccountList()
        {
            try
            {
                MainWindow.ServerConnection.OpenConnection();
                DataSet results = MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[Accounts]");
                MainWindow.ServerConnection.CloseConnection();
                debitAccList = results.Tables[0];
                creditAccList = results.Tables[1];
                transferAccList = results.Tables[2];
            }
            catch
            {
                MessageWindow.ShowMessage("發生錯誤請再試一次", MessageType.ERROR);
                return;
            }
        }

        private void SetCombobox()
        {
            int dcSwitch = listDC.SelectedIndex;

            if (dcSwitch == 0)
            {
                cbTargetAccount.ItemsSource = debitAccList.DefaultView;
                lbDirection.Content = ">";
            }
            else
            {
                cbTargetAccount.ItemsSource = creditAccList.DefaultView;
                lbDirection.Content = "<";
            }
            cbTargetAccount.DisplayMemberPath = "Accounts_Name";
            cbTargetAccount.SelectedValuePath = "Accounts_ID";

            cbSourceAccount.ItemsSource = transferAccList.DefaultView;
            cbSourceAccount.DisplayMemberPath = "Accounts_Name";
            cbSourceAccount.SelectedValuePath = "Accounts_ID";
        }

        private void GetAccountRecords(string AccID)
        {
            try
            {
                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("ID", AccID));
                DataTable results = MainWindow.ServerConnection.ExecuteProc("[Get].[AccountsDetailDetailReport]", parameters);
                MainWindow.ServerConnection.CloseConnection();

                dgDetails = results.Copy();

                DataColumn iss = new DataColumn("IsSelected", typeof(bool));
                iss.DefaultValue = false;
                dgDetails.Columns.Add(iss);

                DataColumn no = new DataColumn("NO", typeof(int));
                no.DefaultValue = "0";
                dgDetails.Columns.Add(no);
                foreach (DataRow dr in dgDetails.Rows)
                {
                    int index = dgDetails.Rows.IndexOf(dr);
                    dr["NO"] = index + 1;
                }

                DataColumn sa = new DataColumn("StrikeAmount", typeof(int));
                sa.DefaultValue = "0";
                dgDetails.Columns.Add(sa);

                DataColumn sn = new DataColumn("StrikeNote", typeof(string));
                sn.DefaultValue = "";
                dgDetails.Columns.Add(sn);

                dgStrikeDataGrid.ItemsSource = dgDetails.DefaultView;
            }
            catch
            {
                MessageWindow.ShowMessage("發生錯誤請再試一次", MessageType.SUCCESS);
                return;
            }
        }

        private BalanceSheetTypeEnum GetStrikeTypeEnum()
        {
            string selected = cbTargetAccount.SelectedValue.ToString();
            BalanceSheetType = selected.StartsWith("002") || selected.StartsWith("007")
                ? BalanceSheetTypeEnum.Bank
                : selected.StartsWith("004") ? BalanceSheetTypeEnum.MedPoint : BalanceSheetTypeEnum.Normal;
            return BalanceSheetType;
        }

        private void CalculateSubTotal()
        {
            count = 0;
            subtotal = 0;
            foreach (DataRow dr in dgDetails.Rows)
            {
                int index = dgDetails.Rows.IndexOf(dr);
                if ((bool)dr["IsSelected"])
                {
                    count++;
                    subtotal = subtotal + (int)dr["StrikeAmount"];
                }
            }
            lbSelectedCount.Content = count.ToString();
            lbSelectedSum.Content = subtotal.ToString();
        }

        private void listDC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = (ListBox)sender;
            if (listbox.SelectedItem == null)
            {
                if (e.RemovedItems.Count > 0)
                {
                    object itemToReselect = e.RemovedItems[0];
                    if (listbox.Items.Contains(itemToReselect))
                    {
                        listbox.SelectedItem = itemToReselect;
                    }
                }
            }
            else
            {
                if (cbTargetAccount != null && cbSourceAccount != null)
                {
                    SetCombobox();
                }
            }
        }

        private void cbTargetAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTargetAccount.SelectedValue != null)
            {
                DataTable dt = transferAccList.Copy();
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (dt.Rows[i]["Accounts_ID"].ToString() == cbTargetAccount.SelectedValue.ToString())
                    {
                        dt.Rows[i].Delete();
                    }
                }
                dt.AcceptChanges();
                cbSourceAccount.ItemsSource = dt.DefaultView;
                GetAccountRecords(cbTargetAccount.SelectedValue.ToString());
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cbTargetAccount_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            var itemsViewOriginal = CollectionViewSource.GetDefaultView(cmb.ItemsSource);

            itemsViewOriginal.Filter = (o) =>
            {
                if (string.IsNullOrEmpty(cmb.Text)) return false;
                else
                {
                    if (((string)o).StartsWith(cmb.Text)) return true;
                    else return false;
                }
            };

            cmb.IsDropDownOpen = true;
            itemsViewOriginal.Refresh();
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (index < dgDetails.Rows.Count)
            {
                int value = Convert.ToInt32(dgDetails.Rows[index]["Value"]);
                dgDetails.Rows[index]["StrikeAmount"] = value;
                dgDetails.Rows[index]["IsSelected"] = true;
                CalculateSubTotal();
            }
        }

        private void btnBatchStrike_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnStrike_Click(object sender, RoutedEventArgs e)
        {
            int index = GetRowIndexRouted(e);
            double.TryParse(dgDetails.Rows[index]["StrikeAmount"].ToString(), out double amount);
            string note = dgDetails.Rows[index]["StrikeNote"].ToString();
            string sourceID = dgDetails.Rows[index]["ID"].ToString();
            BalanceSheetTypeEnum enu = GetStrikeTypeEnum();

            var left = cbTargetAccount.SelectedValue;
            var right = cbSourceAccount.SelectedValue;

            if (left == null || right == null)
            {
                MessageWindow.ShowMessage("尚未選擇帳戶！", MessageType.ERROR);
                return;
            }
            if (amount == 0)
            {
                MessageWindow.ShowMessage("沖帳金額為零！", MessageType.ERROR);
                return;
            }

            if (enu == BalanceSheetTypeEnum.Bank)
            {
                sourceID = left.ToString();
            }

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", amount));
            parameters.Add(new SqlParameter("TYPE", sourceID));
            parameters.Add(new SqlParameter("NOTE", note));
            parameters.Add(new SqlParameter("TARGET", right.ToString()));
            parameters.Add(new SqlParameter("SOURCE_ID", left.ToString()));

            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheet]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (result.Rows.Count > 0 && result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("沖帳成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("沖帳失敗，請稍後再試", MessageType.ERROR);
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new StrikeHistoryWindow();
            historyWindow.ShowDialog();
        }
    }
}