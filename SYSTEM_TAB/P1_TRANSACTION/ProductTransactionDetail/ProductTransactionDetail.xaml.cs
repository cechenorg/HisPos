using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSearchWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
        private string masID;
        private DataTable detail;
        private string cusID;
        private string payMethod;
        public int ID;
        public CustomerSearchCondition Con;

        public NewClass.Prescription.Prescription CurrentPrescription
        {
            get; set;
        }

        public ProductTransactionDetail(DataRow masterRow, DataTable detailTable)
        {
            InitializeComponent();
            masID = masterRow["TraMas_ID"].ToString();
            string priceType = detailTable.Rows[0]["TraDet_PriceType"].ToString();
            AssignMasterValue(masterRow, priceType);
            detail = detailTable;
            ProductDataGrid.ItemsSource = detail.DefaultView;
            var Price = new List<DataGridTextColumn>();
            NewFunction.FindChildGroup(ProductDataGrid, "Price", ref Price);
            detail.Columns.Add("IsReward_Format", typeof(bool));
            foreach (DataRow dr in detail.Rows)
            {
                int PerPrice = Math.Abs((int)dr["TraDet_Price"]);
                if (GetPriceList(dr["TraDet_ProductID"].ToString()).Rows[0]["Pro_MemberPrice"].ToString() == PerPrice.ToString() || GetPriceList(dr["TraDet_ProductID"].ToString()).Rows[0]["Pro_RetailPrice"].ToString() == PerPrice.ToString() || dr["TraDet_IsGift"].ToString() == "1")
                {
                    dr["Irr"] = "";
                }
                else
                {
                    dr["Irr"] = "Yes";
                }

                if ((bool)dr["Pro_IsReward"] == false)
                {
                    dr["IsReward_Format"] = false;
                }
                else
                {
                    dr["IsReward_Format"] = true;
                }
            }
        }

        private void GetEmployeeList()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[GetEmployee]");
            MainWindow.ServerConnection.CloseConnection();
            DataTable noEmpty = result.Copy();
            DataRow toInsert = result.NewRow();
            result.Rows.InsertAt(toInsert, 0);
            var CashierList = new List<ComboBox>();
            NewFunction.FindChildGroup(ProductDataGrid, "cbCashier", ref CashierList);

            string res = string.Join(Environment.NewLine, result.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));

            foreach (DataRow dr in detail.Rows)
            {
                int index = detail.Rows.IndexOf(dr);
                CashierList[index].ItemsSource = result.DefaultView;
                CashierList[index].SelectedIndex = 0;
                if (!string.IsNullOrEmpty(dr["TraDet_RewardPersonnel"].ToString()))
                {
                    CashierList[index].ItemsSource = noEmpty.DefaultView;
                    CashierList[index].SelectedValue = dr["TraDet_RewardPersonnel"];
                }
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

            lbCusName.Content = masterRow["Cus_Name"].ToString();
            lblRealTotal.Content = masterRow["TraMas_RealTotal"];
            lblCashier.Content = masterRow["Emp_Name"];
            tbCardNum.Text = masterRow["TraMas_CardNumber"].ToString();
            tbTaxNum.Text = masterRow["TraMas_TaxNumber"].ToString();
            tbInvoiceNum.Content = masterRow["TraMas_InvoiceNumber"].ToString();
            lblPreTotal.Content = masterRow["TraMas_PreTotal"];
            lbDiscountAmt.Content = masterRow["TraMas_DiscountAmt"].ToString();
            lblPriceType.Content = PriceTypeConverted;
            cusID = masterRow["TraMas_CustomerID"].ToString();
            payMethod = masterRow["TraMas_PayMethod"].ToString();
            lbCash.Content = masterRow["TraMas_CashAmount"].ToString();
            lbCard.Content = masterRow["TraMas_CardAmount"].ToString();
            lbVoucher.Content = masterRow["TraMas_VoucherAmount"].ToString();
            lbCashCoupon.Content = masterRow["TraMas_CashCoupon"].ToString();
            lblTradeTime.Content = masterRow["TransTime_Format"];
            tbNote.Text = masterRow["TraMas_Note"].ToString();
            tbPhone.Content= masterRow["Cus_Phone"].ToString();

            /* string ogTransTime = masterRow["TraMas_UpdateTime"].ToString();
             DateTime dTime = DateTime.Parse(ogTransTime);
             string formatTransTime = dTime.ToString("yyyy-MM-dd HH:mm");*/

            if (masterRow["TraMas_UpdateTime"] != DBNull.Value)
            {
                lblUpdateTime.Content = Convert.ToDateTime(masterRow["TraMas_UpdateTime"]).ToString("MM/dd/yyyy h:mm tt");
            }
            else
            {
                lblUpdateTime.Content = "";
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ConfirmWindow cw = new ConfirmWindow("是否刪除交易紀錄?", "刪除紀錄確認");
            if (!(bool)cw.DialogResult) { return; }
            else
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
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            ConfirmWindow cw = new ConfirmWindow("是否進行退貨?", "退貨確認");
            if (!(bool)cw.DialogResult) { return; }
            else
            {
                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("MasterID", masID));
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordReturn]", parameters);
                MainWindow.ServerConnection.CloseConnection();

                if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                {
                    MessageWindow.ShowMessage("退貨成功！", MessageType.SUCCESS);
                    Close();
                }
                else { MessageWindow.ShowMessage("退貨失敗！", MessageType.ERROR); }
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", masID));
            parameters.Add(new SqlParameter("CustomerID", cusID));
            parameters.Add(new SqlParameter("PayMethod", payMethod));
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
            parameters.Add(new SqlParameter("TraMas_CashCoupon", lbCashCoupon.Content));
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
                string Id = null;
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
                    Id);
            }
            return dt;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetEmployeeList();
        }

        private DataTable GetPriceList(string id)
        {
            int war = 0;
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", id));
            parameters.Add(new SqlParameter("WAREHOUSE_ID", war));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[SearchProductPriceByID]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return result;
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

        private async void ProductDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (index < detail.Rows.Count && index >= 0)
            {
                string proID = detail.Rows[index]["TraDet_ProductID"].ToString();
                ProductDetailWindow.ShowProductDetailWindow();
                Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { proID, "0" }, "ShowProductDetail"));
                await Task.Delay(20);
                ProductDetailWindow.ActivateProductDetailWindow();
            }
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

        private void lbCusName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (cusID is null) return;

            CustomerManageViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).CustomerManageView;

            Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, cusID, ""));
            WindowState = WindowState.Minimized;
        }

        private void GetSelectedCustomer(NotificationMessage<NewClass.Person.Customer.Customer> receiveSelectedCustomer)
        {
            Messenger.Default.Unregister<NotificationMessage<NewClass.Person.Customer.Customer>>(this);
            if (receiveSelectedCustomer.Content is null)
            {
                if (!receiveSelectedCustomer.Notification.Equals("AskAddCustomerData")) return;
            }
            else
            {
                CurrentPrescription = new NewClass.Prescription.Prescription();
                CurrentPrescription.Patient = new NewClass.Person.Customer.Customer();
                CurrentPrescription.Patient = receiveSelectedCustomer.Content;
                cusID = CurrentPrescription.Patient.ID.ToString();

                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("Cus_Id", int.Parse(cusID)));
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerByCusId]", parameters);
                MainWindow.ServerConnection.CloseConnection();
                if (result.Rows.Count > 0)
                {
                    lbCusName.Content = result.Rows[0]["Person_Name"].ToString();
                    MainWindow.ServerConnection.CloseConnection();
                    btnSubmit.IsEnabled = true;
                    lblChanged.Content = "已修改";
                    lblChanged.Foreground = Brushes.Red;
                }
            }
        }
    }
}