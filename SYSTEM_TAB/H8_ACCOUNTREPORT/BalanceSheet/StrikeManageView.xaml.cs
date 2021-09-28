using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class StrikeManageView : UserControl, INotifyPropertyChanged
    {
        #region /// Variables ///

        private DataTable debitAccList = new DataTable();
        private DataTable creditAccList = new DataTable();
        private DataTable transferAccList = new DataTable();

        private DataTable dgDetails = new DataTable();

        private int count = 0;
        private int subtotal = 0;
        private BalanceSheetTypeEnum BalanceSheetType;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private bool isSelectAll = false;

        public bool IsSelectAll
        {
            get { return isSelectAll; }
            set
            {
                isSelectAll = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsSelectAll"));
            }
        }

        private DateTime sDate;

        public DateTime SDate
        {
            get => sDate;
            set
            {
                sDate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SDate"));
            }
        }

        private DateTime eDate;

        public DateTime EDate
        {
            get => eDate;
            set
            {
                eDate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("EDate"));
            }
        }

        #endregion /// Variables ///

        public StrikeManageView()
        {
            InitializeComponent();
            DataContext = this;
            InitView();
        }

        private void InitView()
        {
            GetAccountList();
            SetCombobox();
            DateTime dt = DateTime.Now;
            string year = dt.Year.ToString();
            string month = dt.Month.ToString();
            dpSDate.SelectedDate = Convert.ToDateTime(year + "-" + month + "-" + "1");
            dpEDate.SelectedDate = dt;
        }

        private void ReloadDetail()
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
            CalculateSubTotal();
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

                DataColumn cc = new DataColumn("CanClose", typeof(bool));
                sn.DefaultValue = true;
                dgDetails.Columns.Add(cc);

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
                if ((bool)dr["IsSelected"])
                {
                    count++;
                    subtotal += (int)dr["StrikeAmount"];
                }
            }
            lbSelectedCount.Content = count.ToString();
            lbSelectedSum.Content = subtotal.ToString();
        }

        private void listDC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = (ListBox)sender;
            isSelectAll = false;

            if (dgStrikeDataGrid != null)
            {
                dgStrikeDataGrid.ItemsSource = null;
                dgStrikeDataGrid.Items.Refresh();
            }

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

                if (cbTargetAccount.SelectedValue.ToString().StartsWith("002"))
                {
                    foreach (DataRow dr in dgDetails.Rows)
                    {
                        dr["CanClose"] = false;
                    }
                }
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
                if (string.IsNullOrEmpty(cmb.Text))
                {
                    return false;
                }
                else
                {
                    if (((string)o).StartsWith(cmb.Text))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            };

            cmb.IsDropDownOpen = true;
            itemsViewOriginal.Refresh();
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (index < dgDetails.Rows.Count && index >= 0)
            {
                int value = Convert.ToInt32(dgDetails.Rows[index]["Value"]);
                dgDetails.Rows[index]["StrikeAmount"] = value;
                dgDetails.Rows[index]["IsSelected"] = true;
                CalculateSubTotal();
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new StrikeHistoryWindow();
            _ = historyWindow.ShowDialog();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (IsSelectAll)
            {
                foreach (DataRow dr in dgDetails.Rows)
                {
                    dr["IsSelected"] = true;
                    int value = Convert.ToInt32(dr["Value"]);
                    dr["StrikeAmount"] = value;
                }
            }
            else
            {
                foreach (DataRow dr in dgDetails.Rows)
                {
                    dr["IsSelected"] = false;
                }
            }
            CalculateSubTotal();
        }

        private void cbSelect_Checked(object sender, RoutedEventArgs e)
        {
            int index = GetRowIndexRouted(e);
            if (index >= 0)
            {
                dgDetails.Rows[index]["IsSelected"] = true;
            }
            CalculateSubTotal();
        }

        private void cbSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            int index = GetRowIndexRouted(e);
            if (index >= 0)
            {
                dgDetails.Rows[index]["IsSelected"] = false;
            }
            CalculateSubTotal();
        }

        #region /// 沖帳 ///

        private bool StrikeAction(object sender, RoutedEventArgs e)
        {
            int index = GetRowIndexRouted(e);
            _ = double.TryParse(dgDetails.Rows[index]["StrikeAmount"].ToString(), out double amount);
            string note = dgDetails.Rows[index]["StrikeNote"].ToString();
            string sourceID = dgDetails.Rows[index]["ID"].ToString();
            BalanceSheetTypeEnum enu = GetStrikeTypeEnum();

            var left = cbTargetAccount.SelectedValue;
            var right = cbSourceAccount.SelectedValue;

            if (left == null || right == null)
            {
                MessageWindow.ShowMessage("尚未選擇帳戶！", MessageType.ERROR);
                return false;
            }
            if (amount == 0)
            {
                MessageWindow.ShowMessage("沖帳金額為零！", MessageType.ERROR);
                return false;
            }

            if (enu == BalanceSheetTypeEnum.Bank)
            {
                sourceID = left.ToString();
            }

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", amount));
            parameters.Add(new SqlParameter("TYPE", sourceID));
            parameters.Add(new SqlParameter("NOTE", note));
            parameters.Add(new SqlParameter("TARGET", right.ToString()));
            parameters.Add(new SqlParameter("SOURCE_ID", left.ToString()));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheet]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            return result.Rows.Count > 0 && result.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
        }

        private void btnStrike_Click(object sender, RoutedEventArgs e)
        {
            if (StrikeAction(sender, e))
            {
                MessageWindow.ShowMessage("沖帳成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("沖帳失敗，請稍後再試", MessageType.ERROR);
            }
            ReloadDetail();
        }

        private void btnBatchStrike_Click(object sender, RoutedEventArgs e)
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否進行批次沖帳?", "批次沖帳", true);
            if ((bool)confirmWindow.DialogResult)
            {
                bool success = true;
                var left = cbTargetAccount.SelectedValue;
                var right = cbSourceAccount.SelectedValue;
                BalanceSheetTypeEnum enu = GetStrikeTypeEnum();

                if (left == null || right == null)
                {
                    MessageWindow.ShowMessage("尚未選擇帳戶！", MessageType.ERROR);
                    return;
                }

                MainWindow.ServerConnection.OpenConnection();
                foreach (DataRow dr in dgDetails.Rows)
                {
                    if ((bool)dr["IsSelected"])
                    {
                        _ = double.TryParse(dr["StrikeAmount"].ToString(), out double amount);
                        string note = dr["StrikeNote"].ToString();
                        string sourceID = dr["ID"].ToString();

                        if (amount == 0)
                        {
                            continue;
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
                        if (result.Rows.Count > 0 && result.Rows[0].Field<string>("RESULT").Equals("SUCCESS")) { }
                        else
                        {
                            success = false;
                        }
                    }
                }
                MainWindow.ServerConnection.CloseConnection();

                if (!success)
                {
                    MessageWindow.ShowMessage("批次沖帳發生錯誤", MessageType.ERROR);
                }
                else
                {
                    MessageWindow.ShowMessage("批次沖帳成功", MessageType.SUCCESS);
                }
                ReloadDetail();
            }
        }

        #endregion /// 沖帳 ///

        #region /// 結案 ///

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            int index = GetRowIndexRouted(e);
            _ = double.TryParse(dgDetails.Rows[index]["StrikeAmount"].ToString(), out double amount);
            string sourceID = dgDetails.Rows[index]["ID"].ToString();
            var left = cbTargetAccount.SelectedValue;

            if (amount != 0)
            {
                if (!StrikeAction(sender, e))
                {
                    return;
                }
            }

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Emp", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("Detail", sourceID));
            parameters.Add(new SqlParameter("ID", left.ToString()));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[DeclareClosed]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (result.Rows.Count > 0 && result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("結案成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("結案失敗，請稍後再試", MessageType.ERROR);
            }
            ReloadDetail();
        }

        private void btnBatchClose_Click(object sender, RoutedEventArgs e)
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否進行批次結案?", "批次結案", true);
            if ((bool)confirmWindow.DialogResult)
            {
                bool success = true;
                var left = cbTargetAccount.SelectedValue;
                var right = cbSourceAccount.SelectedValue;
                BalanceSheetTypeEnum enu = GetStrikeTypeEnum();

                MainWindow.ServerConnection.OpenConnection();
                foreach (DataRow dr in dgDetails.Rows)
                {
                    if ((bool)dr["IsSelected"])
                    {
                        _ = double.TryParse(dr["StrikeAmount"].ToString(), out double amount);
                        string note = dr["StrikeNote"].ToString();
                        string sourceID = dr["ID"].ToString();

                        if (amount != 0)
                        {
                            if (left == null || right == null)
                            {
                                MessageWindow.ShowMessage("尚未選擇帳戶！", MessageType.ERROR);
                                ReloadDetail();
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
                            if (result.Rows.Count > 0 && result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                            {
                                List<SqlParameter> para = new List<SqlParameter>();
                                para.Add(new SqlParameter("Emp", ViewModelMainWindow.CurrentUser.ID));
                                para.Add(new SqlParameter("Detail", sourceID));
                                para.Add(new SqlParameter("ID", left.ToString()));
                                DataTable res = MainWindow.ServerConnection.ExecuteProc("[Set].[DeclareClosed]", para);
                                if (res.Rows.Count > 0 && res.Rows[0].Field<string>("RESULT").Equals("SUCCESS")) { }
                                else // 結案失敗
                                {
                                    success = false;
                                }
                            }
                            else // 沖帳失敗
                            {
                                success = false;
                            }
                        }
                        else
                        {
                            List<SqlParameter> para = new List<SqlParameter>();
                            para.Add(new SqlParameter("Emp", ViewModelMainWindow.CurrentUser.ID));
                            para.Add(new SqlParameter("Detail", sourceID));
                            para.Add(new SqlParameter("ID", left.ToString()));
                            DataTable res = MainWindow.ServerConnection.ExecuteProc("[Set].[DeclareClosed]", para);
                            if (res.Rows.Count > 0 && res.Rows[0].Field<string>("RESULT").Equals("SUCCESS")) { }
                            else // 結案失敗
                            {
                                success = false;
                            }
                        }
                    }
                }
                MainWindow.ServerConnection.CloseConnection();
                if (!success)
                {
                    MessageWindow.ShowMessage("批次結案發生錯誤", MessageType.ERROR);
                }
                else
                {
                    MessageWindow.ShowMessage("批次結案成功", MessageType.SUCCESS);
                }
                ReloadDetail();
            }
        }

        #endregion /// 結案 ///

        private void tbAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateSubTotal();
        }

        private void tbAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            _ = textBox.CaptureMouse();
        }

        private void tbAmount_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void tbAmount_IsMouseCaptureWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void btnDateFilter_Click(object sender, RoutedEventArgs e)
        {
            DateTime sd = (DateTime)dpSDate.SelectedDate;
            int sdInt = int.Parse(sd.ToString("yyyyMMdd"));
            DateTime ed = (DateTime)dpEDate.SelectedDate;
            int edInt = int.Parse(ed.ToString("yyyyMMdd"));

            if (sd != null && ed != null)
            {
                DataTable dt = dgDetails.Copy();
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i]["Date"].ToString()))
                    {
                        string[] split = dt.Rows[i]["Date"].ToString().Split('-');
                        if (split.Length == 3)
                        {
                            _ = int.TryParse(split[0] + split[1] + split[2], out int mergeDate2K);
                            _ = int.TryParse(split[0], out int yy);
                            _ = int.TryParse(split[1], out int mm);
                            _ = int.TryParse(split[2], out int dd);
                            if (yy > 1000 && (mergeDate2K < sdInt || mergeDate2K > edInt))
                            {
                                dt.Rows[i].Delete();
                            }
                            else if (yy <= 1000)
                            {
                                int mergeDateMG = int.Parse((yy + 1911).ToString() + mm.ToString() + dd.ToString());
                                if (mergeDateMG < sdInt || mergeDateMG > edInt)
                                {
                                    dt.Rows[i].Delete();
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                dt.AcceptChanges();
                dgStrikeDataGrid.ItemsSource = dt.DefaultView;
            }
        }
    }
}