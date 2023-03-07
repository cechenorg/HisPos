using DomainModel.Enum;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddCustomerWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Trade;
using His_Pos.NewClass.Trade.TradeRecord;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSearchWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
        //private string masID;
        private DataTable detail;
        private string cusID;
        //private string payMethod;
        //public int ID;
        public CustomerSearchCondition Con;
        //public AddCustomerWindow addCustomerWindow;


        public ProductTransactionDetail(DataRow masterRow, DataTable detailTable)
        {
            InitializeComponent();
            DataTable masTable = masterRow.Table.Clone();
            masTable.ImportRow(masterRow);
            ((ProductTransactionDetailViewModel)DataContext).MasterTable = masTable;
            ((ProductTransactionDetailViewModel)DataContext).Details = TableToClass(detailTable);
            ((ProductTransactionDetailViewModel)DataContext).MasterID = masterRow["TraMas_ID"].ToString();
            ((ProductTransactionDetailViewModel)DataContext).PayMethod = masterRow["TraMas_PayMethod"].ToString();
            ((ProductTransactionDetailViewModel)DataContext).GetData();

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "YesAction":
                        Close();
                        break;
                }
            });

            detail = detailTable;

            //string priceType = detailTable.Rows[0]["TraDet_PriceType"].ToString();
            int.TryParse(lbDiscountAmt.Content.ToString(), out int dis);
            if (dis > 0)
            {
                lbDiscountAmt.Foreground = Brushes.Red;
            }
            
            btnSubmit.IsEnabled = false;
            lblChanged.Content = "未修改";
            lblChanged.Foreground = Brushes.Black;

            DateTime dt = new DateTime();
            if (!string.IsNullOrEmpty(lblTradeTime.Content.ToString()))
            {
                dt = DateTime.Parse(lblTradeTime.Content.ToString());
            }
            Authority userAuthority = ViewModelMainWindow.CurrentUser.Authority;
            if (userAuthority == Authority.Admin)
            {
                btnDelete.IsEnabled = true;
            }
            else
            {
                btnDelete.IsEnabled = false;
            }
            if (DateTime.Compare(ViewModelMainWindow.ClosingDate.AddDays(1), dt) < 0)
            {
                btnReturn.IsEnabled = true;
            }
            else
            {
                btnReturn.IsEnabled = false;
            }
            if (masterRow.Table.Columns.Contains("TraMas_IsEnable"))
            {
                int isEnable = Convert.ToInt32(masterRow["TraMas_IsEnable"]);
                if(isEnable == 0)
                {
                    btnDelete.IsEnabled = false;
                    btnReturn.IsEnabled = false;
                    tbCardNum.IsEnabled = false;
                    tbTaxNum.IsEnabled = false;
                    tbCusName.IsEnabled = false;
                    DelContent.Visibility = Visibility.Visible;
                }
            }
        }

        private List<TradeDetail> TableToClass(DataTable table)
        {
            List<TradeDetail> details = new List<TradeDetail>();

            table.Columns.Add("TraDet_RewardPersonnelName", typeof(string));
            table.Columns.Add("IsReward_Format", typeof(bool));
            foreach (DataRow dr in table.Rows)
            {
                TradeDetail trade = new TradeDetail();
                trade.TraDet_DetailID = Convert.ToString(dr["TraDet_DetailID"]);
                trade.TraDet_ProductID = Convert.ToString(dr["TraDet_ProductID"]);
                trade.TraDet_ProductName = Convert.ToString(dr["TraDet_ProductName"]);
                trade.TraDet_Amount = Convert.ToInt32(dr["TraDet_Amount"]);
                trade.TraDet_PriceType = Convert.ToString(dr["TraDet_PriceType"]);
                trade.TraDet_Price = Convert.ToInt32(dr["TraDet_Price"]);
                trade.TraDet_PriceSum = Convert.ToInt32(dr["TraDet_PriceSum"]);
                trade.TraDet_IsGift = Convert.ToBoolean(dr["TraDet_IsGift"]);
                trade.TraDet_DepositAmount = Convert.ToInt32(dr["TraDet_DepositAmount"]);
                trade.TraDet_RewardPersonnel = Convert.ToString(dr["TraDet_RewardPersonnel"]);
                trade.TraDet_RewardPersonnelName = Convert.ToString(dr["TraDet_RewardPersonnelName"]);
                trade.TraDet_RewardPercent = Convert.ToInt32(dr["TraDet_RewardPercent"] is DBNull ? 0 : dr["TraDet_RewardPercent"]);
                trade.Pro_IsReward = Convert.ToBoolean(dr["Pro_IsReward"] is DBNull ? false : dr["Pro_IsReward"]);
                trade.IsReward_Format = Convert.ToBoolean(dr["TraMas_IsEnable"] is DBNull ? false : dr["TraMas_IsEnable"]);
                trade.Irr = Convert.ToString(dr["Irr"] is DBNull ? string.Empty : dr["Irr"]);
                trade.Emps = TradeService.GetPosEmployee();
                if (trade.Emps != null && trade.Emps.Count() > 0)
                {
                    trade.Emp = trade.Emps.First(w => w.CashierID == Convert.ToString(dr["TraDet_RewardPersonnel"]));
                }
                details.Add(trade);
            }
            return details;
        }
        //private void GetEmployeeList()
        //{
        //    DataTable result = TradeService.GetEmployeeList();
        //    DataTable noEmpty = result.Copy();
        //    DataRow toInsert = result.NewRow();
        //    result.Rows.InsertAt(toInsert, 0);
        //    var CashierList = new List<ComboBox>();
        //    NewFunction.FindChildGroup(ProductDataGrid, "cbCashier", ref CashierList);

        //    string res = string.Join(Environment.NewLine, result.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));

        //    foreach (DataRow dr in detail.Rows)
        //    {
        //        int index = detail.Rows.IndexOf(dr);
        //        CashierList[index].ItemsSource = result.DefaultView;
        //        CashierList[index].SelectedIndex = 0;
        //        if (!string.IsNullOrEmpty(dr["TraDet_RewardPersonnel"].ToString()))
        //        {
        //            CashierList[index].ItemsSource = noEmpty.DefaultView;
        //            CashierList[index].SelectedValue = dr["TraDet_RewardPersonnel"];
        //        }
        //    }
        //}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //GetEmployeeList();
        }

        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            //DateTime dt = new DateTime();
            //if(!string.IsNullOrEmpty(lblTradeTime.Content.ToString()))
            //{
            //    dt = DateTime.Parse(lblTradeTime.Content.ToString());
            //}
            //if(DateTime.Compare(ViewModelMainWindow.ClosingDate.AddDays(1), dt) < 0)
            //{
            //    ChangeContent();
            //}
        }

        private void cbCashier_DropDownClosed(object sender, System.EventArgs e)
        {
            ChangeContent();
        }

        private void ChangeContent()
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

        //private void lbCusName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (cusID is null) return;

        //    CustomerManageViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).CustomerManageView;

        //    Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, cusID, ""));
        //    WindowState = WindowState.Minimized;
        //}

        private void GetSelectedCustomer(NotificationMessage<NewClass.Person.Customer.Customer> receiveSelectedCustomer)
        {
            Messenger.Default.Unregister<NotificationMessage<NewClass.Person.Customer.Customer>>(this);
            if (receiveSelectedCustomer.Content is null)
            {
                if (!receiveSelectedCustomer.Notification.Equals("AskAddCustomerData")) return;
            }
            else
            {
                Customer customer = new Customer();
                customer = receiveSelectedCustomer.Content;
                cusID = customer.ID.ToString();
                tbCusName.Text = customer.Name;

                ((ProductTransactionDetailViewModel)DataContext).CusID = cusID;
                ((ProductTransactionDetailViewModel)DataContext).CusName = tbCusName.Text;

                ChangeContent();
            }
        }

        private void tbCusName_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (tb.Text.Length == 0)
                {
                    cusID = "0";

                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> para = new List<SqlParameter>();
                    para.Add(new SqlParameter("Cus_Id", int.Parse(cusID)));
                    DataTable res = MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerByCusId]", para);
                    MainWindow.ServerConnection.CloseConnection();
                    Customer customer = CustomerDb.GetCustomerByCusId(int.Parse(cusID));
                    if (customer != null && !string.IsNullOrEmpty(customer.Name))
                    {
                        tbCusName.Text = res.Rows[0]["Person_Name"].ToString();
                        ChangeContent();
                    }
                    //if (res.Rows.Count > 0)
                    //{
                    //    tbCusName.Text = res.Rows[0]["Person_Name"].ToString();
                    //    MainWindow.ServerConnection.CloseConnection();
                    //    ChangeContent();
                    //}
                    return;
                }

                // 電話查詢
                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                bool isCell = tb.Text.StartsWith("09");
                if (isCell)
                {
                    parameters.Add(new SqlParameter("Cus_Cellphone", tb.Text));
                    parameters.Add(new SqlParameter("Cus_Telephone", DBNull.Value));
                    Con = CustomerSearchCondition.CellPhone;
                }
                else if (tb.Text.Length >= 7 && tb.Text.Length <= 10 && !tb.Text.StartsWith("1"))
                {
                    parameters.Add(new SqlParameter("Cus_Cellphone", DBNull.Value));
                    parameters.Add(new SqlParameter("Cus_Telephone", tb.Text));
                    Con = CustomerSearchCondition.Tel;
                }
                else
                {
                    parameters.Add(new SqlParameter("Cus_Cellphone", DBNull.Value));
                    parameters.Add(new SqlParameter("Cus_Telephone", DBNull.Value));
                }

                // 姓名查詢
                if (!int.TryParse(tb.Text, out int i))
                {
                    parameters.Add(new SqlParameter("@Cus_Name", tb.Text));
                    Con = CustomerSearchCondition.Name;
                }
                else
                {
                    parameters.Add(new SqlParameter("@Cus_Name", DBNull.Value));
                }

                // 生日查詢
                if (tb.Text.Length == 6)
                {
                    int.TryParse(tb.Text.Substring(0, 2), out int year);
                    int.TryParse(tb.Text.Substring(2, 2), out int month);
                    int.TryParse(tb.Text.Substring(4, 2), out int day);
                    string yearStr = (year + 1911).ToString();
                    string dateStr = yearStr + month.ToString("00") + day.ToString("00");

                    parameters.Add(new SqlParameter("@Cus_Birthday", dateStr));
                    Con = CustomerSearchCondition.Birthday;
                }
                else if (tb.Text.Length == 7 && tb.Text.StartsWith("1"))
                {
                    int.TryParse(tb.Text.Substring(0, 3), out int year);
                    int.TryParse(tb.Text.Substring(3, 2), out int month);
                    int.TryParse(tb.Text.Substring(5, 2), out int day);
                    string yearStr = (year + 1911).ToString();
                    string dateStr = yearStr + month.ToString("00") + day.ToString("00");
                    parameters.Add(new SqlParameter("@Cus_Birthday", dateStr));
                    Con = CustomerSearchCondition.Birthday;
                }
                else
                {
                    parameters.Add(new SqlParameter("@Cus_Birthday", DBNull.Value));
                }

                DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[CustomerQuery]", parameters);
                MainWindow.ServerConnection.CloseConnection();
                if (result.Rows.Count == 0)
                {
                    MessageWindow.ShowMessage("查無資料！", MessageType.ERROR);
                }
                else if (result.Rows.Count > 0)
                {
                    CustomerSearchWindow customerSearch;
                    if (Con == CustomerSearchCondition.Birthday)
                    {
                        Messenger.Default.Register<NotificationMessage<Customer>>(this, GetSelectedCustomer);
                        var twCulture = new CultureInfo("zh-TW", true);
                        twCulture.DateTimeFormat.Calendar = new TaiwanCalendar();

                        var dateString = tb.Text.Trim();
                        dateString = dateString.PadLeft(8, '0');
                        var date = DateTime.ParseExact(dateString, "yMMdd", twCulture);

                        customerSearch = new CustomerSearchWindow(date);
                        Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    }
                    else
                    {
                        Messenger.Default.Register<NotificationMessage<Customer>>(this, GetSelectedCustomer);
                        customerSearch = new CustomerSearchWindow(Con, 0, tb.Text.Trim());
                        Messenger.Default.Unregister<NotificationMessage<Customer>>(this);
                    }
                }
            }
        }
    }
}