using System.Data;
using System.Windows;
using System.Data.SqlClient;
using System.Collections.Generic;
using His_Pos.FunctionWindow;
using His_Pos.Class;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    /// <summary>
    /// CustomerDepositManageView.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerDepositManageView : Window
    {
        string cus;
        DataTable detail;
        DataTable depositTable;
        public CustomerDepositManageView(string cusID)
        {
            cus = cusID;
            InitializeComponent();
            detail=new DataTable();
            depositTable = new DataTable();
            CustomerDepositRecord();
            CustomerDepositManage();
        }
        public void CustomerDepositRecord() {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TraMas_CustomerID", cus));
            detail = MainWindow.ServerConnection.ExecuteProc("[POS].[DepositRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            ProductDepositRecordDataGrid.ItemsSource=detail.DefaultView;
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

        private void btnWithdraw_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dr in depositTable.Rows) {
                if ((int)dr["Amount"] > (int)dr["TraDet_DepositAmount"]) {
                    MessageWindow.ShowMessage("提取量不得大於寄庫量", MessageType.WARNING);
                    return;
                }
            }
            foreach (DataRow dr in depositTable.Rows)
            {
                if ((int)dr["Amount"]!=0)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("TraDet_ProductID", dr["TraDet_ProductID"]));
                    parameters.Add(new SqlParameter("Amount", dr["Amount"]));
                    parameters.Add(new SqlParameter("cusID", cus));
                    DataTable result=MainWindow.ServerConnection.ExecuteProc("[POS].[DepositBuckles]", parameters);
                    MainWindow.ServerConnection.CloseConnection();
                    if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                    {
                        MessageWindow.ShowMessage("提取成功！", MessageType.SUCCESS);
                        Close();
                    }
                    else { MessageWindow.ShowMessage("提取失敗！", MessageType.ERROR); }
                }
            }
        }
        }
    }

