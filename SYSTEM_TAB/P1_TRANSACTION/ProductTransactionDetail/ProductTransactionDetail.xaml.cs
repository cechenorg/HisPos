using His_Pos.Class;
using His_Pos.FunctionWindow;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionDetail
{
    /// <summary>
    /// ProductTransactionDetail.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTransactionDetail : Window
    {
        string masID;

        public ProductTransactionDetail(DataRow masterRow, DataTable detailTable)
        {
            InitializeComponent();
            masID = masterRow["TraMas_ID"].ToString();
            string priceType = detailTable.Rows[0]["TraDet_PriceType"].ToString();
            AssignMasterValue(masterRow, priceType);
            ProductDataGrid.ItemsSource = detailTable.DefaultView;
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
            lblRealTotal.Content = masterRow["TraMas_RealTotal"];
            lblCashier.Content = masterRow["TraMas_Cashier"];
            tbCardNum.Text = masterRow["TraMas_CardNumber"].ToString();
            tbTaxNum.Text = masterRow["TraMas_TaxNumber"].ToString();
            tbInvoiceNum.Text = masterRow["TraMas_InvoiceNumber"].ToString();
            lblPreTotal.Content = masterRow["TraMas_PreTotal"];
            tbDiscountAmt.Text = masterRow["TraMas_DiscountAmt"].ToString();
            lblPriceType.Content = PriceTypeConverted;
            if (masterRow["TraMas_PayMethod"].ToString() == "信用卡") { rbCard.IsChecked = true; }
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

        }
    }
}
