using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Medicine.NotEnoughMedicine;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.CustomerDataControl;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.FunctionWindow.NotEnoughOTCPurchaseWindow;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    /// <summary>
    /// ProductTransactionView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTransactionView : UserControl
    {
        private NoCustomerControl CustomerView { get; set; }

        private DataTable ProductList;
        private string AppliedPrice;

        private int preTotal = 0;
        private int discountAmount = 0;
        private int realTotal = 0;
        private double totalProfit = 0;

        private bool isGift = false;

        private static readonly Regex _regex = new Regex("^[0-9]+$");
        private static bool IsTextAllowed(string text) { return !_regex.IsMatch(text); }

        public ProductTransactionView()
        {
            InitializeComponent();
            CustomerView = new NoCustomerControl();
            GetEmployeeList();
            ProductList = new DataTable();
            ProductDataGrid.ItemsSource = ProductList.DefaultView;
        }

        private void GetEmployeeList()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[GetEmployee]");
            MainWindow.ServerConnection.CloseConnection();
            cbCashier.ItemsSource = result.DefaultView;
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

        private string GetPayMethod()
        {
            List<string> list = new List<string>();
            bool CashParse = int.TryParse(tbCash.Text, out int Cash);
            bool CardParse = int.TryParse(tbCard.Text, out int Card);
            bool VoucherParse = int.TryParse(tbVoucher.Text, out int Voucher);
            int Total = Cash + Card + Voucher;

            if (Total != realTotal) 
            {
                return "NOT_MATCH";
            }

            if (CashParse && Cash > 0)
            {
                list.Add("現金");
            }
            if (CardParse && Card > 0)
            {
                list.Add("信用卡");
            }
            if (VoucherParse && Voucher > 0)
            {
                list.Add("禮券");
            }

            string[] arr = list.ToArray();
            string result = string.Join("/", arr);
            return result;
        }

        private void AddProductByInputAction(string searchString, int rowIndex)
        {
            if (string.IsNullOrEmpty(searchString)) return;
            if (searchString.Length == 0)
            {
                if (rowIndex < ProductList.Rows.Count)
                {
                    ProductList.Rows.RemoveAt(rowIndex);
                    CalculateTotal("AMT");
                }
            }
            if (searchString.Length < 5)
            {
                MessageWindow.ShowMessage("搜尋字長度不得小於5", MessageType.WARNING);
                return;
            }

            // 相同商品疊加
            foreach (DataRow dr in ProductList.Rows)
            {
                if (dr["Pro_ID"].ToString() == searchString && isGift == false)
                {
                    dr["Amount"] = int.Parse(dr["Amount"].ToString()) + 1;
                    return;
                }
            }

            MainWindow.ServerConnection.OpenConnection();
            int productCount = ProductStructs.GetProductStructCountBySearchString(searchString, AddProductEnum.Trade);
            MainWindow.ServerConnection.CloseConnection();

            if (productCount == 0)
            {
                MessageWindow.ShowMessage("查無商品", MessageType.WARNING);
                if (rowIndex < ProductList.Rows.Count)
                {
                    ProductList.Rows.RemoveAt(rowIndex);
                    CalculateTotal("AMT");
                }
            }
            else
            {
                if (productCount > 0)
                {
                    int WareID = 0;
                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("SEARCH_STRING", searchString));
                    parameters.Add(new SqlParameter("WAREHOUSE_ID", WareID));
                    DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[SearchProductsByID]", parameters);
                    MainWindow.ServerConnection.CloseConnection();


                    if (ProductList.Rows.Count == 0)
                    {
                        ProductList = result.Clone();

                        DataColumn id = new DataColumn("ID", typeof(int));
                        id.DefaultValue = 0;
                        ProductList.Columns.Add(id);

                        DataColumn amt = new DataColumn("Amount", typeof(int));
                        amt.DefaultValue = 1;
                        ProductList.Columns.Add(amt);

                        DataColumn calc = new DataColumn("Calc", typeof(int));
                        calc.DefaultValue = 0;
                        ProductList.Columns.Add(calc);

                        DataColumn cp = new DataColumn("CurrentPrice", typeof(int));
                        cp.DefaultValue = 0;
                        ProductList.Columns.Add(cp);

                        DataColumn gift = new DataColumn("IsGift", typeof(int));
                        gift.DefaultValue = 0;
                        ProductList.Columns.Add(gift);

                        DataColumn ptt = new DataColumn("PriceTooltip", typeof(string));
                        ptt.DefaultValue = "";
                        ProductList.Columns.Add(ptt);

                        DataColumn profit = new DataColumn("Profit", typeof(double));
                        ptt.DefaultValue = 0;
                        ProductList.Columns.Add(profit);

                        DataColumn deposit = new DataColumn("Deposit", typeof(int));
                        ptt.DefaultValue = 0;
                        ProductList.Columns.Add(deposit);
                    }

                    DataRow newRow = ProductList.NewRow();
                    if (result.Rows.Count == 1)
                    {
                        DataRow NewProduct = result.Rows[0];
                        newRow.ItemArray = NewProduct.ItemArray;
                        if (rowIndex < ProductList.Rows.Count)
                        {
                            ProductList.Rows.RemoveAt(rowIndex);
                            ProductList.Rows.InsertAt(newRow, rowIndex);
                        }
                        else { ProductList.ImportRow(NewProduct); }
                        ProductDataGrid.ItemsSource = ProductList.DefaultView;
                    }
                    else if (result.Rows.Count > 1)
                    {
                        TradeAddProductWindow tapw = new TradeAddProductWindow(result);
                        tapw.ShowDialog();
                        DataRow NewProduct = tapw.SelectedProduct;
                        newRow.ItemArray = NewProduct.ItemArray;
                        if (rowIndex < ProductList.Rows.Count)
                        {
                            ProductList.Rows.RemoveAt(rowIndex);
                            ProductList.Rows.InsertAt(newRow, rowIndex);
                        }
                        else { ProductList.ImportRow(NewProduct); }
                        ProductDataGrid.ItemsSource = ProductList.DefaultView;
                    }

                    // Focus Next Row
                    Dispatcher.InvokeAsync(() =>
                    {
                        var ProductIDList = new List<TextBox>();
                        NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox",
                            ref ProductIDList);
                        ProductIDList[ProductIDList.Count - 1].Focus();
                    }, DispatcherPriority.ApplicationIdle);

                    SetPrice();
                    if (isGift)
                    {
                        ProductList.Rows[rowIndex]["CurrentPrice"] = 0;
                        ProductList.Rows[rowIndex]["IsGift"] = 1;
                        isGift = false;
                        btnGift.IsEnabled = true;
                    }
                    CalculateTotal("AMT");
                }
                else { MessageWindow.ShowMessage("查無此商品", MessageType.WARNING); }
            }
        }

        private void SetPrice()
        {
            if (ProductList != null)
            {
                foreach (DataRow dr in ProductList.Rows)
                {
                    bool tp = int.TryParse(dr["IsGift"].ToString(), out int ig);
                    if (!tp || ig != 1)
                    {
                        dr["CurrentPrice"] = dr[AppliedPrice];
                    }
                }
            }
        }

        private void CalculateTotal(string type)
        {
            if (ProductList == null) { return; }
            if (ProductList.Rows.Count > 0)
            {
                foreach (DataRow dr in ProductList.Rows)
                {
                    if (dr["CurrentPrice"].ToString() != "" && dr["Amount"].ToString() != "")
                    {
                        dr["Calc"] = int.Parse(dr["CurrentPrice"].ToString()) * int.Parse(dr["Amount"].ToString());
                        dr["Profit"] = (double.Parse(dr["CurrentPrice"].ToString()) -
                        double.Parse(dr["Inv_LastPrice"].ToString())) * int.Parse(dr["Amount"].ToString());
                    }
                }
                preTotal = int.Parse(ProductList.Compute("SUM(Calc)", string.Empty).ToString());
                lblPreTotal.Content = preTotal;
            }
            if (ProductList.Rows.Count == 0)
            {
                preTotal = 0;
                lblPreTotal.Content = preTotal;
                totalProfit = 0;
                lblTotalProfit.Content = totalProfit;
            }
            CalculateDiscount(type);
            CalculateChange();
        }

        private async void CalculateDiscount(string type)
        {
            if (type == "AMT" && tbDiscountAmt.Text != "")
            {
                double amt = double.Parse(tbDiscountAmt.Text);
                if (amt == 0) { tbDiscountPer.Text = ""; }
                else { tbDiscountPer.Text = ((preTotal - amt) / preTotal * 100).ToString("N0").Replace("0", ""); }
            }
            else if (type == "PER" && tbDiscountPer.Text != "")
            {
                double per = double.Parse(tbDiscountPer.Text);
                if (per > 10) { tbDiscountAmt.Text = (preTotal - preTotal * per / 100).ToString("N0"); }
                else { tbDiscountAmt.Text = (preTotal - preTotal * per / 10).ToString("N0"); }
            }
            discountAmount = int.Parse(tbDiscountAmt.Text);
            realTotal = preTotal - discountAmount;
            lblRealTotal.Content = realTotal;

            double preProfit = 0;
            double.TryParse(ProductList.Compute("SUM(Profit)", string.Empty).ToString(), out preProfit);
            totalProfit = preProfit - discountAmount;
            lblTotalProfit.Content = totalProfit;

            if (int.Parse(lblRealTotal.Content.ToString()) < 0)
            {
                MessageWindow.ShowMessage("折扣後金額小於0！", MessageType.ERROR);
                tbDiscountAmt.Text = "0";
                CalculateTotal(type);
                switch (type)
                {
                    case "AMT":
                        await Task.Delay(20);
                        tbDiscountAmt.Focus();
                        break;
                    case "PER":
                        await Task.Delay(20);
                        tbDiscountPer.Focus();
                        break;
                }
            }
        }

        private void CalculateChange()
        {
            if (tbPaid.Text.Length > 0 && !IsTextAllowed(tbPaid.Text))
            {
                int change = int.Parse(tbPaid.Text) - int.Parse(lblRealTotal.Content.ToString());
                if (change >= 0) { lblChange.Content = change; }
                else
                {
                    MessageWindow.ShowMessage("實收金額小於應收金額！", MessageType.ERROR);
                    tbPaid.Text = "";
                    CalculateChange();
                    tbPaid.Focus();
                }
            }
            else { lblChange.Content = "0"; }
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
            foreach (DataRow dr in ProductList.Rows)
            {
                dt.Rows.Add(
                    dr["ID"],
                    dr["Pro_ID"],
                    dr["Amount"],
                    AppliedPrice,
                    dr[AppliedPrice],
                    dr["Calc"],
                    dr["IsGift"]);
            }
            return dt;
        }

        private void ClearPage()
        {
            ProductList.Clear();
            tbDiscountAmt.Text = "0";
            tbNote.Text = "";
            tbTaxNum.Text = "";
            tbCardNum.Text = "";
            tbInvoiceNum.Text = "";
            tbPaid.Text = "";

            tbCash.Text = "";
            tbCard.Text = "";
            tbVoucher.Text = "";

            AppliedPrice = "Pro_RetailPrice";
            CalculateTotal("AMT");
            PriceCombo.SelectedIndex = 0;
        }

        #region ----- Events -----

        private void ProductDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var ProductIDList = new List<TextBox>();
            NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox",
                ref ProductIDList);
            ProductIDList[ProductIDList.Count - 1].Focus();
        }

        #region ProductID

        private void ProductIDTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int currentRowIndex = ProductDataGrid.Items.IndexOf(ProductDataGrid.CurrentItem);
            int preCount = ProductList.Rows.Count;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                AddProductByInputAction(tb.Text, currentRowIndex);
                int newCount = ProductList.Rows.Count;
                foreach (DataRow dr in ProductList.Rows)
                {
                    dr["ID"] = ProductList.Rows.IndexOf(dr) + 1;
                }
                tb.Text = "";
                //if (currentRowIndex == ProductList.Rows.Count - 1 && newCount > preCount) { tb.Text = ""; }
            }
        }

        private void ProductIDTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text.Length == 13)
            {
                Key key = Key.Enter;
                IInputElement target = Keyboard.FocusedElement;
                RoutedEvent routedEvent = Keyboard.KeyDownEvent;
                if (target != null)
                {
                    target.RaiseEvent(new KeyEventArgs(
                    Keyboard.PrimaryDevice,
                    Keyboard.PrimaryDevice.ActiveSource, 0, key)
                    { RoutedEvent = routedEvent });
                }
            }
        }

        #endregion

        #region Amount

        private void Amount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (IsTextAllowed(tb.Text)) { tb.Text = ""; }
            if (tb.Text == "") { tb.Text = "0"; }
            CalculateTotal("AMT");
        }

        private void Amount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                MoveFocus(request);
            }
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotal("AMT");
        }

        #endregion

        #region Price

        private void Price_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (IsTextAllowed(tb.Text)) { tb.Text = ""; }
            if (tb.Text == "") { tb.Text = "0"; }
            CalculateTotal("AMT");
        }

        private void Price_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotal("AMT");
            foreach (DataRow dr in ProductList.Rows)
            {
                dr["PriceTooltip"] = string.Format("{0:F2}", dr["Inv_LastPrice"]) + "/" + 
                    (double.Parse(dr["CurrentPrice"].ToString()) -
                    double.Parse(dr["Inv_LastPrice"].ToString())).ToString();
            }
        }

        private void Price_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                MoveFocus(request);
            }
        }

        #endregion

        #region Discount

        private void tbDiscountAmt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (IsTextAllowed(tb.Text)) { tb.Text = ""; }
            if (tbDiscountAmt.Text == "")
            {
                tbDiscountAmt.Text = "0";
                CalculateTotal("AMT");
                return;
            }
            if (preTotal != 0 && int.Parse(tbDiscountAmt.Text) >= 0) { CalculateTotal("AMT"); }
            else { tbDiscountAmt.Text = "0"; }
        }

        private void tbDiscountPer_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (IsTextAllowed(tb.Text)) { tb.Text = ""; }
            if (tbDiscountPer.Text == "")
            {
                tbDiscountAmt.Text = "0";
                CalculateTotal("AMT");
                return;
            }
            if (preTotal != 0 && int.Parse(tbDiscountPer.Text) > 0) { CalculateTotal("PER"); }
            else { tbDiscountPer.Text = ""; }
        }

        private void tbDiscountAmt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbPaid.Focus(); }
        }

        private void tbDiscountPer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbPaid.Focus(); }
        }

        #endregion

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (ProductList.Rows.Count > 0 && index < ProductList.Rows.Count)
            {
                ProductList.Rows.Remove(ProductList.Rows[index]);
            }
            CalculateTotal("AMT");
        }

        private void PriceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (PriceCombo.SelectedIndex)
            {
                case 0:
                    AppliedPrice = "Pro_RetailPrice";
                    break;
                case 1:
                    AppliedPrice = "Pro_MemberPrice";
                    break;
                case 2:
                    AppliedPrice = "Pro_EmployeePrice";
                    break;
                case 3:
                    AppliedPrice = "Pro_SpecialPrice";
                    break;
                default:
                    AppliedPrice = "Pro_RetailPrice";
                    break;
            }

            SetPrice();
            CalculateTotal("AMT");
        }

        private void next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (ProductList.Rows.Count == 0 || index >= ProductList.Rows.Count) { return; }

            int original = int.Parse(ProductList.Rows[index]["Amount"].ToString());
            int stock = int.Parse(ProductList.Rows[index]["Inv_Inventory"].ToString());
            if (original < stock) { ProductList.Rows[index]["Amount"] = original + 1; }
        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (ProductList.Rows.Count == 0 || index >= ProductList.Rows.Count) { return; }

            int original = int.Parse(ProductList.Rows[index]["Amount"].ToString());
            if (original > 0) { ProductList.Rows[index]["Amount"] = original - 1; }
        }

        private void tbPaid_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculateChange();
        }

        private void tbPaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            { 
                CalculateChange();
                tbCash.Text = realTotal.ToString();
            }
        }

        private void tbCardNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbTaxNum.Focus(); }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ConfirmWindow cw = new ConfirmWindow("是否清除頁面資料?", "清除頁面確認");
            if (!(bool)cw.DialogResult) { return; }
            else { ClearPage(); }
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dr in ProductList.Rows)
            {
                if (int.Parse(dr["Amount"].ToString()) == 0)
                {
                    int index = ProductList.Rows.IndexOf(dr);
                    ProductList.Rows[index].Delete();
                }
            }
            ProductList.AcceptChanges();

            if (ProductList.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("尚未新增售出商品項目！", MessageType.ERROR);
                return;
            }

            if (GetPayMethod() == "NOT_MATCH") 
            {
                MessageWindow.ShowMessage("付款金額與應收金額不符！", MessageType.WARNING);
                return;
            }

            //9.14欠OTC採購
            var notEnoughMedicines = new NotEnoughMedicines();
            foreach (DataRow dr in ProductList.Rows)
            {
                if (int.Parse(dr["Amount"].ToString()) > int.Parse(dr["Inv_Inventory"].ToString()))
                {
                    int buckle;
                    if (int.Parse(dr["Inv_Inventory"].ToString()) <= 0)
                    {
                        buckle = 0;

                        notEnoughMedicines.Add(new NotEnoughMedicine(dr["Pro_ID"].ToString(), dr["Pro_ChineseName"].ToString(), int.Parse(dr["Amount"].ToString()) - buckle, true, false, 0, 0, int.Parse(dr["Amount"].ToString()) - buckle));
                    }
                    else {
                        notEnoughMedicines.Add(new NotEnoughMedicine(dr["Pro_ID"].ToString(), dr["Pro_ChineseName"].ToString(), int.Parse(dr["Amount"].ToString()) - int.Parse(dr["Inv_Inventory"].ToString()), true, false, 0, 0, int.Parse(dr["Amount"].ToString()) - int.Parse(dr["Inv_Inventory"].ToString())));

                    }

                }
            }
            if (notEnoughMedicines.Count > 0 )
            {
                
                
                var purchaseWindow = new NotEnoughOTCPurchaseWindow("欠OTC採購", "OTC", notEnoughMedicines);
                if (purchaseWindow.DialogResult is null || !(bool)purchaseWindow.DialogResult)
                {
                    MessageWindow.ShowMessage("欠OTC採購取消。", MessageType.WARNING);
                }
                else
                {
                    
                    MessageWindow.ShowMessage("採購單已送出。", MessageType.WARNING);
                    
                }
            }
            //9.14欠OTC採購

            ConfirmWindow confirmWindow = new ConfirmWindow("是否送出結帳資料?", "結帳確認");
            if (!(bool)confirmWindow.DialogResult) { return; }

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CustomerID", CustomerView.CusID));
            parameters.Add(new SqlParameter("ChkoutTime", DateTime.Now));
            parameters.Add(new SqlParameter("PayMethod", GetPayMethod()));
            parameters.Add(new SqlParameter("CashAmount", tbCash.Text));
            parameters.Add(new SqlParameter("CardAmount", tbCard.Text));
            parameters.Add(new SqlParameter("VoucherAmount", tbVoucher.Text));
            parameters.Add(new SqlParameter("PreTotal", preTotal));
            parameters.Add(new SqlParameter("DiscountAmt", discountAmount));
            parameters.Add(new SqlParameter("RealTotal", realTotal));
            parameters.Add(new SqlParameter("CardNumber", tbCardNum.Text));
            parameters.Add(new SqlParameter("InvoiceNumber", tbInvoiceNum.Text));
            parameters.Add(new SqlParameter("TaxNumber", tbTaxNum.Text));
            parameters.Add(new SqlParameter("Cashier", cbCashier.Text));
            parameters.Add(new SqlParameter("Note", tbNote.Text));
            parameters.Add(new SqlParameter("DETAILS", TransferDetailTable()));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordInsert]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                ClearPage();
                MessageWindow.ShowMessage("資料傳送成功！", MessageType.SUCCESS);
            }
            else { MessageWindow.ShowMessage("資料傳送失敗！", MessageType.ERROR); }
        }

        private async void lblProductName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (index < ProductList.Rows.Count)
            {
                string proID = ProductList.Rows[index]["Pro_ID"].ToString();
                ProductDetailWindow.ShowProductDetailWindow();
                Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { proID, "0" }, "ShowProductDetail"));
                await Task.Delay(20);
                ProductDetailWindow.ActivateProductDetailWindow();
            }
        }

        #endregion

        private void Price_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void btnGift_Click(object sender, RoutedEventArgs e)
        {
            isGift = true;
            btnGift.IsEnabled = false;

            // Focus Next Row
            Dispatcher.InvokeAsync(() =>
            {
                var ProductIDList = new List<TextBox>();
                NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox",
                    ref ProductIDList);
                ProductIDList[ProductIDList.Count - 1].Focus();
            }, DispatcherPriority.ApplicationIdle);
        }

        private void tbCash_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter) 
            {
                if (tbCash.Text == "" && tbCard.Text == "" && tbVoucher.Text == "") 
                {
                    tb.Text = realTotal.ToString();
                }
            }
        }

        private void tbCard_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                if (tbCash.Text == "" && tbCard.Text == "" && tbVoucher.Text == "")
                {
                    tb.Text = realTotal.ToString();
                }
            }
        }

        private void tbVoucher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                if (tbCash.Text == "" && tbCard.Text == "" && tbVoucher.Text == "")
                {
                    tb.Text = realTotal.ToString();
                }
            }
        }

        private void Deposit_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Deposit_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Deposit_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
