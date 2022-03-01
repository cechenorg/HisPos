using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet
{
    /// <summary>
    /// AccountManage.xaml 的互動邏輯
    /// </summary>
    public partial class AccountManage : Window
    {
        private DataTable accList;

        public AccountManage()
        {
            InitializeComponent();
            GetAccounts();
        }

        private void GetAccounts()
        {
            MainWindow.ServerConnection.OpenConnection();
            accList = MainWindow.ServerConnection.ExecuteProc("[Get].[ReportAccounts]");
            MainWindow.ServerConnection.CloseConnection();

            accList.Columns.Add("Accounts_Prefix", typeof(string));

            foreach (DataRow dr in accList.Rows)
            {
                // LEFT
                if (dr["Accounts_ID"].ToString().StartsWith("001"))
                {
                    dr["Accounts_Prefix"] = "流動資產-現金";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("002"))
                {
                    dr["Accounts_Prefix"] = "流動資產-銀行";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("003"))
                {
                    dr["Accounts_Prefix"] = "流動資產-應收帳款";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("004"))
                {
                    dr["Accounts_Prefix"] = "流動資產-申報應收帳款";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("005"))
                {
                    dr["Accounts_Prefix"] = "流動資產-零用金";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("006"))
                {
                    dr["Accounts_Prefix"] = "存貨-商品";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("007"))
                {
                    dr["Accounts_Prefix"] = "預付款項-預付款項";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("008"))
                {
                    dr["Accounts_Prefix"] = "預付款項-其他預付款";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("009"))
                {
                    dr["Accounts_Prefix"] = "長期投資-長期投資";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("010"))
                {
                    dr["Accounts_Prefix"] = "固定資產-資產";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("011"))
                {
                    dr["Accounts_Prefix"] = "固定資產-累計折舊";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("012"))
                {
                    dr["Accounts_Prefix"] = "其他資產-存出保證金";
                }

                // RIGHT
                if (dr["Accounts_ID"].ToString().StartsWith("101"))
                {
                    dr["Accounts_Prefix"] = "流動負債-應付帳款";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("102"))
                {
                    dr["Accounts_Prefix"] = "流動負債-應付費用";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("103"))
                {
                    dr["Accounts_Prefix"] = "流動負債-應付稅捐";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("104"))
                {
                    dr["Accounts_Prefix"] = "預收款項-預收款";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("105"))
                {
                    dr["Accounts_Prefix"] = "代收-代收";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("201"))
                {
                    dr["Accounts_Prefix"] = "資本-股本(登記)";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("202"))
                {
                    dr["Accounts_Prefix"] = "公積及盈餘-本期損益";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("203"))
                {
                    dr["Accounts_Prefix"] = "公積及盈餘-本年損益";
                }
                if (dr["Accounts_ID"].ToString().StartsWith("204"))
                {
                    dr["Accounts_Prefix"] = "公積及盈餘-未實現損益";
                }
            }

            AccountList.ItemsSource = accList.DefaultView;
        }

        private void UpdateAccounts()
        {
            DataTable dt = accList.Copy();
            dt.Columns.Remove("IsZero");
            dt.Columns.Remove("Accounts_Prefix");
            try
            {
                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("ACCList", dt));
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateAccounts]", parameters);
                MainWindow.ServerConnection.CloseConnection();
            }
            catch
            {
                MessageWindow.ShowMessage("科目設定失敗!", MessageType.ERROR);
                return;
            }
            MessageWindow.ShowMessage("科目設定成功!", MessageType.SUCCESS);
            Close();
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            UpdateAccounts();
        }
    }
}