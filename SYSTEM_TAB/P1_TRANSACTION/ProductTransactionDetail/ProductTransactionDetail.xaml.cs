using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionDetail
{
    /// <summary>
    /// ProductTransactionDetail.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTransactionDetail : Window
    {
        string masID;
        DataTable detail;
        string cusID;
        string payMethod;
        public ProductTransactionDetail(DataRow masterRow, DataTable detailTable)
        {
            InitializeComponent();
            masID = masterRow["TraMas_ID"].ToString();
            string priceType = detailTable.Rows[0]["TraDet_PriceType"].ToString();
            AssignMasterValue(masterRow, priceType);
            detail = detailTable;
            ProductDataGrid.ItemsSource = detail.DefaultView;
        }

        private void GetEmployeeList()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[GetEmployee]");
            MainWindow.ServerConnection.CloseConnection();
            var CashierList = new List<ComboBox>();
            NewFunction.FindChildGroup(ProductDataGrid, "cbCashier", ref CashierList);
            foreach (DataRow dr in detail.Rows) 
            {
                int index = detail.Rows.IndexOf(dr);
                CashierList[index].ItemsSource = result.DefaultView;
            }
        }
        private void AssignMasterValue(DataRow masterRow, string PriceType) 
        {
            string PriceTypeConverted;
            switch (PriceType)
            {
                case "Pro_RetailPrice":
                    PriceTypeConverted = "零售價";
                    break;
                case "Pro_MemberPrice":
                    PriceTypeConverted = "會員價";
                    break;
                case "Pro_EmployeePrice":
                    PriceTypeConverted = "員工價";
                    break;
                case "Pro_SpecialPrice":
                    PriceTypeConverted = "特殊價";
                    break;
                default:
                    PriceTypeConverted = "零售價";
                    break;
            }

            lbCusName.Content = masterRow["Cus_Name"];
            lblRealTotal.Content = masterRow["TraMas_RealTotal"];
            lblCashier.Content = masterRow["TraMas_Cashier"];
            tbCardNum.Text = masterRow["TraMas_CardNumber"].ToString();
            tbTaxNum.Text = masterRow["TraMas_TaxNumber"].ToString();
            tbInvoiceNum.Content = masterRow["TraMas_InvoiceNumber"].ToString();
            lblPreTotal.Content = masterRow["TraMas_PreTotal"];
            lbDiscountAmt.Content = masterRow["TraMas_DiscountAmt"].ToString();
            lblPriceType.Content = PriceTypeConverted;
            cusID = masterRow["TraMas_CustomerID"].ToString();
            payMethod= masterRow["TraMas_PayMethod"].ToString();
            lbCash.Content = masterRow["TraMas_CashAmount"].ToString();
            lbCard.Content = masterRow["TraMas_CardAmount"].ToString();
            lbVoucher.Content = masterRow["TraMas_VoucherAmount"].ToString();
            lblTradeTime.Content = masterRow["TransTime_Format"];
            tbNote.Text = masterRow["TraMas_Note"].ToString();
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /*int index = GetRowIndex(e);
            if (ProductList.Rows.Count > 0 && index < ProductList.Rows.Count)
            {
                ProductList.Rows.Remove(ProductList.Rows[index]);
            }
            CalculateTotal("AMT");*/
        }

        private void lblProductName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            /*int index = GetRowIndex(e);
            if (index < ProductList.Rows.Count)
            {
                string proID = ProductList.Rows[index]["Pro_ID"].ToString();
                ProductDetailWindow.ShowProductDetailWindow();
                Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { proID, "0" }, "ShowProductDetail"));
            }*/
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MasterID", masID));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordDelete]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("刪除成功！", MessageType.SUCCESS);
                Close();
            }
            else { MessageWindow.ShowMessage("刪除失敗！", MessageType.ERROR); }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", masID));
            parameters.Add(new SqlParameter("CustomerID", cusID));
            parameters.Add(new SqlParameter("PayMethod",payMethod));

            parameters.Add(new SqlParameter("PreTotal", lblPreTotal.Content));
            parameters.Add(new SqlParameter("DiscountAmt", lbDiscountAmt.Content));
            parameters.Add(new SqlParameter("RealTotal", lblRealTotal.Content));
            parameters.Add(new SqlParameter("CardNumber", tbCardNum.Text));
            parameters.Add(new SqlParameter("InvoiceNumber", tbInvoiceNum.Content));
            parameters.Add(new SqlParameter("TaxNumber", tbTaxNum.Text));
            parameters.Add(new SqlParameter("Cashier", lblCashier.Content));
            parameters.Add(new SqlParameter("Note", tbNote.Text));
            parameters.Add(new SqlParameter("TraMas_CashAmount", lbCash.Content));
            parameters.Add(new SqlParameter("TraMas_CardAmount", lbCard.Content));
            parameters.Add(new SqlParameter("TraMas_VoucherAmount", lbVoucher.Content));
            parameters.Add(new SqlParameter("DETAILS", TransferDetailTable()));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordEdit]", parameters);
            MainWindow.ServerConnection.CloseConnection();


            if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("修改成功！", MessageType.SUCCESS);
                Close();
            }
            else { MessageWindow.ShowMessage("修改失敗！", MessageType.ERROR); }

        }
        private DataTable TransferDetailTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TraDet_DetailID", typeof(int));
            dt.Columns.Add("TraDet_ProductID", typeof(string));
            dt.Columns.Add("TraDet_Amount", typeof(int));
            dt.Columns.Add("TraDet_PriceType", typeof(string));
            dt.Columns.Add("TraDet_Price", typeof(int));
            dt.Columns.Add("TraDet_PriceSum", typeof(int));
            dt.Columns.Add("TraDet_IsGift", typeof(int));
            dt.Columns.Add("TraDet_DepositAmount", typeof(int));
            dt.Columns.Add("TraDet_RewardPersonnel", typeof(string));
            foreach (DataRow dr in detail.Rows)
            {
                int index = detail.Rows.IndexOf(dr);
                var CashierList = new List<ComboBox>();
                NewFunction.FindChildGroup(ProductDataGrid, "cbCashier", ref CashierList);
                string Id=null;
                if (CashierList[index].SelectedItem != null)
                {
                    DataRowView drv = (DataRowView)CashierList[index].SelectedItem;
                    Id = drv.Row["Emp_ID"].ToString();
                }
                dt.Rows.Add(
                    dr["TraDet_DetailID"],
                    dr["TraDet_ProductID"],
                    dr["TraDet_Amount"],
                    dr["TraDet_PriceType"],
                    dr["TraDet_Price"],
                    dr["TraDet_PriceSum"],
                    dr["TraDet_IsGift"],
                    dr["TraDet_DepositAmount"],
                    Id
                    );
            }
            return dt;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetEmployeeList();

            var CashierList = new List<ComboBox>();
            NewFunction.FindChildGroup(ProductDataGrid, "cbCashier", ref CashierList);
            foreach (DataRow dr in detail.Rows)
            {
                int index = detail.Rows.IndexOf(dr);
                CashierList[index].SelectedValue= dr["TraDet_RewardPersonnel"];
            }
        }

        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSubmit.IsEnabled = true;
            lblChanged.Content = "已修改";
            lblChanged.Foreground = Brushes.Red;
        }


        private void cbCashier_DropDownClosed(object sender, System.EventArgs e)
        {
            btnSubmit.IsEnabled = true;
            lblChanged.Content = "已修改";
            lblChanged.Foreground = Brushes.Red;
        }
    }
}
