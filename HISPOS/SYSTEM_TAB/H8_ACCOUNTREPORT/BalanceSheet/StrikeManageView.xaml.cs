﻿using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Accounts;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.Accounts;
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

        public List<Accounts> SourceAccounts;
        public List<Accounts> DestinationAccounts;
        

        private int count = 0;
        private int subtotal = 0;
        private BalanceSheetTypeEnum BalanceSheetType;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public bool IsSelectAll { get; set; }

        #endregion /// Variables ///

        public StrikeManageView()
        {
            InitializeComponent();
            DataContext = this;
            InitView();
            dpStrikeDate.DisplayDateStart = ViewModelMainWindow.ClosingDate.AddDays(1);
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
            dpStrikeDate.SelectedDate = dt;
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
                TableToType(results.Tables[0], results.Tables[1]);
                cbTargetAccount.ItemsSource = SourceAccounts;
                cbSourceAccount.ItemsSource = DestinationAccounts;
            }
            catch
            {
                MessageWindow.ShowMessage("發生錯誤請再試一次", MessageType.ERROR);
                return;
            }
        }
        /// <summary>
        /// (20220331)載入會計科目至Model
        /// </summary>
        /// <param name="Source_Table"></param>
        /// <param name="Des_Table"></param>
        private void TableToType(DataTable Source_Table,DataTable Des_Table)
        {
            SourceAccounts = new List<Accounts>();
            foreach (DataRow dr in Source_Table.Rows)
            {
                Accounts accounts = new Accounts();
                accounts.Accounts_ID = Convert.ToString(dr["Accounts_ID"]);
                accounts.Accounts_Name = " " + Convert.ToString(dr["Accounts_Name"]);//預留空格，防止關鍵字搜尋帶入第一筆
                accounts.Accounts_Merge = accounts.Accounts_ID + "-" + accounts.Accounts_Name;
                accounts.Accounts_InsertTime = Convert.ToDateTime(dr["Accounts_InsertTime"]);
                accounts.Accounts_Enable = Convert.ToInt32(dr["Accounts_Enable"]);
                SourceAccounts.Add(accounts);
            }
            DestinationAccounts = new List<Accounts>();
            foreach (DataRow dr in Des_Table.Rows)
            {
                Accounts accounts = new Accounts();
                accounts.Accounts_ID = Convert.ToString(dr["Accounts_ID"]);
                accounts.Accounts_Name = " " + Convert.ToString(dr["Accounts_Name"]);//預留空格，防止關鍵字搜尋帶入第一筆
                accounts.Accounts_Merge = accounts.Accounts_ID + "-" + accounts.Accounts_Name;
                accounts.Accounts_InsertTime = Convert.ToDateTime(dr["Accounts_InsertTime"]);
                accounts.Accounts_Enable = Convert.ToInt32(dr["Accounts_Enable"]);
                DestinationAccounts.Add(accounts);
            }
        }

        private void SetCombobox()
        {
            int dcSwitch = listDC.SelectedIndex;
            dgDetails.Rows.Clear();

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
                DataTable table = table = AccountsDb.GetAccountsDetailDetailReport(AccID);
                if (AccID.Equals("203999"))
                {
                    NormalViewModel normal = new NormalViewModel(true);
                    table = normal.GetProfit(table);
                }

                dgDetails = table.Copy();

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
                if(!table.Columns.Contains("OrderID"))
                {
                    dgStrikeDataGrid.Columns[3].Visibility = Visibility.Visible;
                    dgStrikeDataGrid.Columns[4].Visibility = Visibility.Collapsed;
                }
                else
                {
                    dgStrikeDataGrid.Columns[3].Visibility = Visibility.Collapsed;
                    dgStrikeDataGrid.Columns[4].Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
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

                if (cbTargetAccount.SelectedValue.ToString().StartsWith("002") || cbTargetAccount.SelectedValue.ToString().StartsWith("007"))
                {
                    foreach (DataRow dr in dgDetails.Rows)
                    {
                        dr["CanClose"] = false;
                    }
                }
                foreach (DataRow dr in dgDetails.Rows)
                {
                    if (dr["ID"].ToString() == "0")
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
            ICollectionView itemsViewOriginal = CollectionViewSource.GetDefaultView(cmb.ItemsSource);
            int dcSwitch = listDC.SelectedIndex;
            if(dcSwitch != null && dcSwitch == 0)
            {
                cbTargetAccount.ItemsSource = SourceAccounts;
                cbSourceAccount.ItemsSource = DestinationAccounts;
            }
            else
            {
                cbTargetAccount.ItemsSource = DestinationAccounts;
                cbSourceAccount.ItemsSource = SourceAccounts;
            }
            
            bool isFilter = itemsViewOriginal.CanFilter;
            if (isFilter)
            {
                itemsViewOriginal.Filter = (o) =>
                {
                    Accounts accounts = (Accounts)o;
                    if (string.IsNullOrEmpty(cmb.Text))
                    {
                        return true;
                    }
                    else
                    {
                        if (accounts.Accounts_ID.StartsWith(cmb.Text) || accounts.Accounts_Name.Contains(cmb.Text))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                };
            }
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
            //_ = historyWindow.Show();
            historyWindow.Show();
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

            string TransferID = string.Empty;
            if(dgDetails.Columns.Contains("TransferID"))
            {
                TransferID = dgDetails.Rows[index]["TransferID"].ToString();
            }
            BalanceSheetTypeEnum enu = GetStrikeTypeEnum();

            var left = cbTargetAccount.SelectedValue;
            var right = cbSourceAccount.SelectedValue;

            if (left == null || right == null)
            {
                MessageWindow.ShowMessage("尚未選擇帳戶！", MessageType.ERROR);
                return false;
            }
            //if (amount <= 0)
            //{
            //    MessageWindow.ShowMessage("沖帳金額有誤！", MessageType.ERROR);
            //    return false;
            //}

            if (enu == BalanceSheetTypeEnum.Bank)
            {
                sourceID = left.ToString();
            }

            if(!string.IsNullOrEmpty(TransferID))
            {
                sourceID = TransferID;
            }
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", amount));
            parameters.Add(new SqlParameter("TYPE", sourceID));
            parameters.Add(new SqlParameter("NOTE", note));
            parameters.Add(new SqlParameter("TARGET", right.ToString()));
            parameters.Add(new SqlParameter("SOURCE_ID", left.ToString()));
            parameters.Add(new SqlParameter("StrikeDate", (DateTime)dpStrikeDate.SelectedDate));
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
            DataRow[] drs = dgDetails.Select("IsSelected = 1");
            if(drs == null || drs.Length == 0)
            {
                MessageWindow.ShowMessage("未選取需沖帳的項目!", MessageType.ERROR);
                return;
            }
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
                        parameters.Add(new SqlParameter("StrikeDate", (DateTime)dpStrikeDate.SelectedDate));
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
            ConfirmWindow confirmWindow = new ConfirmWindow("確定是否結案?", "確認");
            if (!(bool)confirmWindow.DialogResult)
            {
                return;
            }
            int index = GetRowIndexRouted(e);
            _ = double.TryParse(dgDetails.Rows[index]["StrikeAmount"].ToString(), out double amount);
            string sourceID = dgDetails.Rows[index]["ID"].ToString();
            if(dgDetails.Columns.Contains("TransferID"))
            {
                string TransferID = Convert.ToString(dgDetails.Rows[index]["TransferID"]);
                if(TransferID.Substring(0,2) == "TR")
                {
                    sourceID = Convert.ToString(dgDetails.Rows[index]["TransferID"]);
                }
            }
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
                    if ((bool)dr["IsSelected"] && dr["CanClose"].ToString() != "False")
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
            ReloadDetail();
            if (dpSDate.SelectedDate != null && dpEDate.SelectedDate != null && cbSourceAccount.SelectedItem != null)
            {
                DateTime sd = (DateTime)dpSDate.SelectedDate;
                int sdInt = int.Parse(sd.ToString("yyyyMMdd"));
                DateTime ed = (DateTime)dpEDate.SelectedDate;
                int edInt = int.Parse(ed.ToString("yyyyMMdd"));

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
                            if (yy > 1000 && (mergeDate2K < sdInt || mergeDate2K > edInt))
                            {
                                dt.Rows[i].Delete();
                            }
                            else if (yy <= 1000)
                            {
                                int mergeDateMG = int.Parse((yy + 1911).ToString() + split[1] + split[2]);
                                if (mergeDateMG < sdInt || mergeDateMG > edInt)
                                {
                                    dt.Rows[i].Delete();
                                }
                            }
                        }
                        else
                        {
                            dt.Rows[i].Delete();
                        }
                    }
                }
                dt.AcceptChanges();
                dgDetails.Rows.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    int index = dt.Rows.IndexOf(dr);
                    dr["NO"] = index + 1;
                    dgDetails.ImportRow(dr);
                }
                if (cbTargetAccount.SelectedValue.ToString().StartsWith("002") || cbTargetAccount.SelectedValue.ToString().StartsWith("002"))
                {
                    foreach (DataRow dr in dgDetails.Rows)
                    {
                        dr["CanClose"] = false;
                    }
                }
                foreach (DataRow dr in dgDetails.Rows)
                {
                    if (dr["ID"].ToString() == "0")
                    {
                        dr["CanClose"] = false;
                    }
                }
                dgStrikeDataGrid.ItemsSource = dgDetails.DefaultView;
            }
        }

        private void btnDateClear_Click(object sender, RoutedEventArgs e)
        {
            dpSDate.SelectedDate = null;
            dpEDate.SelectedDate = null;
            ReloadDetail();
        }
    }
}