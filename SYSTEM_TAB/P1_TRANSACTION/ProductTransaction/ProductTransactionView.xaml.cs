using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO.Ports;
using System.Text;
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
using His_Pos.FunctionWindow.AddCustomerWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Medicine.NotEnoughMedicine;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Product;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.CustomerDataControl;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.FunctionWindow.NotEnoughOTCPurchaseWindow;
using His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.InvoiceControl;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    /// <summary>
    /// ProductTransactionView.xaml 的互動邏輯
    /// </summary>
    /// 
    public partial class ProductTransactionView : UserControl
    {
        public static Label InvoiceNumLable;
        public static TextBox Cuslblcheck;
        public static TextBox FromHISCuslblcheck;
        private DataTable ProductList;
        private string AppliedPrice;
        private int preTotal = 0;
        private int discountAmount = 0;
        private int realTotal = 0;
        private double totalProfit = 0;
        private string cusID = "0";
        private bool isGift = false;
        private bool isReturn = false;
        public AddCustomerWindow addCustomerWindow;
       

        private static readonly Regex _regex = new Regex("^[0-9]+$");
        private static bool IsTextAllowed(string text) { return !_regex.IsMatch(text); }

        public Pharmacy MyPharmacy;

        /*public static RoutedCommand CheckoutCommand = new RoutedCommand();
        public static RoutedCommand PaidAmountCommand = new RoutedCommand();
        public static RoutedCommand CashierCommand = new RoutedCommand();
        public static RoutedCommand CashAmountCommand = new RoutedCommand();
        public static RoutedCommand CardAmountCommand = new RoutedCommand();
        public static RoutedCommand VoucherAmountCommand = new RoutedCommand();
        public static RoutedCommand CardNumberCommand = new RoutedCommand();
        public static RoutedCommand TaxNumberCommand = new RoutedCommand();
        public static RoutedCommand DiscountCommand = new RoutedCommand();
        public static RoutedCommand GiftCommand = new RoutedCommand();
        public static RoutedCommand ReturnCommand = new RoutedCommand();
        public static RoutedCommand CustomerCommand = new RoutedCommand();
        public static RoutedCommand FocusLastRowCommand = new RoutedCommand();*/

        public ProductTransactionView()
        {
            
            InitializeComponent();
            
            InvoiceNumLable = this.tbInvoiceNum;
            Cuslblcheck = this.tbCUS;
            FromHISCuslblcheck = this.tbFromHIS;
            GetEmployeeList();
            ProductList = new DataTable();
            ProductDataGrid.ItemsSource = ProductList.DefaultView;
          
            if (Properties.Settings.Default.InvoiceCheck == "1")
            {
                tbInvoiceNum.Content = Properties.Settings.Default.InvoiceNumber.ToString();
            }
            else {
                tbInvoiceNum.Content = "";
            }
        }

        public static void Send(Key key)
        {
            if (Keyboard.PrimaryDevice != null)
            {
                if (Keyboard.PrimaryDevice.ActiveSource != null)
                {
                    var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
                    {
                        RoutedEvent = Keyboard.PreviewKeyDownEvent
                    };
                    InputManager.Current.ProcessInput(e);

                    // Note: Based on your requirements you may also need to fire events for:
                    // RoutedEvent = Keyboard.PreviewKeyDownEvent
                    // RoutedEvent = Keyboard.KeyUpEvent
                    // RoutedEvent = Keyboard.PreviewKeyUpEvent
                }
            }
        }

        /*private void PaidAmountCommandExecuted(object sender, ExecutedRoutedEventArgs e) 
        {

        }
        private void CashierCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void CashAmountCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void CardAmountCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void VoucherAmountCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void CardNumberCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void TaxNumberCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void DiscountCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void CustomerCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void FocusLastRowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FocusLastRow();
        }*/

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

        private void GetNotEnoughMedicines()
        {
            // 9.14欠OTC採購
            var notEnoughMedicines = new NotEnoughMedicines();
            DataTable check = CheckOTCFromSingde();
            foreach (DataRow dr in ProductList.Rows)
            {
                var amount = dr["Amount"].ToString();
                var inventory = dr["Inv_Inventory"].ToString();
                if (int.Parse(amount) > int.Parse(inventory))
                {
                    var foundRow = check.Select("OTC_Code= '"+dr["Pro_ID"].ToString()+"'");
                    var foundRowBar = check.Select("OTC_Barcode= '"+dr["Pro_ID"].ToString()+"'");
                    if (foundRow.Length> 0 || foundRowBar.Length>0) {
                        int buckle;
                        if (int.Parse(inventory) <= 0)
                        {
                            buckle = 0;
                            notEnoughMedicines.Add(new NotEnoughMedicine(
                                dr["Pro_ID"].ToString(),
                                dr["Pro_ChineseName"].ToString(),
                                int.Parse(amount) - buckle,
                                true,
                                false,
                                0,
                                0,
                                int.Parse(amount) - buckle));
                        }
                        else
                        {
                            notEnoughMedicines.Add(new NotEnoughMedicine(
                                dr["Pro_ID"].ToString(),
                                dr["Pro_ChineseName"].ToString(),
                                int.Parse(amount) - int.Parse(inventory),
                                true,
                                false,
                                0,
                                0,
                                int.Parse(amount) - int.Parse(inventory)));
                        }
                    }
                }
            }
            if (notEnoughMedicines.Count > 0)
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
        }

        private void FocusLastRow()
        {
            Dispatcher.InvokeAsync(() =>
            {
                var ProductIDList = new List<TextBox>();
                NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox",
                    ref ProductIDList);
                ProductIDList[ProductIDList.Count - 1].Focus();
            }, DispatcherPriority.ApplicationIdle);
        }

        private DataTable CheckOTCFromSingde()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[CheckOTCFromSingde]");
            MainWindow.ServerConnection.CloseConnection();
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

            if (int.TryParse(searchString, out int n))
            {
                if (searchString.Length < 5)
                {
                    MessageWindow.ShowMessage("商品代碼長度不得小於5", MessageType.WARNING);
                    FocusLastRow();
                    return;
                }
            }
            else
            {
                if (searchString.Length < 2)
                {
                    MessageWindow.ShowMessage("搜尋字串長度不得小於2", MessageType.WARNING);
                    FocusLastRow();
                    return;
                }
            }
            foreach (DataRow dr in ProductList.Rows) // 相同商品疊加
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
                    
                    if (ProductList.Rows.Count == 0) // Add Columns
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
                        profit.DefaultValue = 0;
                        ProductList.Columns.Add(profit);

                        DataColumn deposit = new DataColumn("Deposit", typeof(int));
                        deposit.DefaultValue = 0;
                        ProductList.Columns.Add(deposit);
                    }

                    DataRow newRow = ProductList.NewRow();
                    
                    if (result.Rows.Count == 1)
                    {
                        DataRow NewProduct = result.Rows[0];
                        newRow.ItemArray = NewProduct.ItemArray;
                        int amt = int.Parse(result.Rows[0]["Available_Amount"].ToString());
                        if (amt < 1)
                        {
                            MessageWindow.ShowMessage("該品項可用量不足", MessageType.WARNING);
                            return;
                        }
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
                        int amt = 0;
                        if (NewProduct != null)
                        {
                            newRow.ItemArray = NewProduct.ItemArray; 
                            amt = int.Parse(NewProduct["Available_Amount"].ToString());
                            if (amt < 1)
                            {
                                MessageWindow.ShowMessage("該品項可用量不足", MessageType.WARNING);
                                return;
                            }
                        }
                        if (rowIndex < ProductList.Rows.Count)
                        {
                            ProductList.Rows.RemoveAt(rowIndex);
                            ProductList.Rows.InsertAt(newRow, rowIndex);
                        }
                        else { ProductList.ImportRow(NewProduct); }
                        ProductDataGrid.ItemsSource = ProductList.DefaultView;
                    }

                    FocusLastRow();
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
                    if (!tp || ig != 1 && dr["CurrentPrice"].ToString() == "0")
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

            if (ProductList.Rows.Count > 0)
            {
                double.TryParse(ProductList.Compute("SUM(Profit)", string.Empty).ToString(), out double preProfit);
                totalProfit = preProfit - discountAmount;
                lblTotalProfit.Content = totalProfit;
            }

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
            dt.Columns.Add("TraDet_DepositAmount", typeof(int));
            foreach (DataRow dr in ProductList.Rows)
            {
                dt.Rows.Add(
                    dr["ID"],
                    dr["Pro_ID"],
                    dr["Amount"],
                    AppliedPrice,
                    dr["CurrentPrice"],
                    dr["Calc"],
                    dr["IsGift"],
                    dr["Deposit"]);
            }
            return dt;
        }

        private void ClearPage()
        {
            ClearCustomerView();

            ProductList.Clear();
            tbDiscountAmt.Text = "0";
            tbNote.Text = "";
            tbTaxNum.Text = "";
            tbCardNum.Text = "";
            tbPaid.Text = "";
            tbCash.Text = "";
            tbCard.Text = "";
            tbVoucher.Text = "";
            cbCashier.SelectedItem = null;
            AppliedPrice = "Pro_RetailPrice";

            CalculateTotal("AMT");
            PriceCombo.SelectedIndex = 0;
        }

        private void CheckoutSubmit()
        {
            GetNotEnoughMedicines();

            ConfirmWindow confirmWindow = new ConfirmWindow("是否送出結帳資料?", "結帳確認" , true);
            if (!(bool)confirmWindow.DialogResult) { return; }

            try
            {
                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("CustomerID", cusID));
                parameters.Add(new SqlParameter("ChkoutTime", DateTime.Now));
                parameters.Add(new SqlParameter("PayMethod", GetPayMethod()));
                parameters.Add(new SqlParameter("CashAmount", tbCash.Text));
                parameters.Add(new SqlParameter("CardAmount", tbCard.Text));
                parameters.Add(new SqlParameter("VoucherAmount", tbVoucher.Text));
                parameters.Add(new SqlParameter("PreTotal", preTotal));
                parameters.Add(new SqlParameter("DiscountAmt", discountAmount));
                parameters.Add(new SqlParameter("RealTotal", realTotal));
                parameters.Add(new SqlParameter("CardNumber", tbCardNum.Text));
                parameters.Add(new SqlParameter("InvoiceNumber", tbInvoiceNum.Content));
                parameters.Add(new SqlParameter("TaxNumber", tbTaxNum.Text));
                parameters.Add(new SqlParameter("Cashier", cbCashier.SelectedValue));
                parameters.Add(new SqlParameter("Note", tbNote.Text));
                parameters.Add(new SqlParameter("DETAILS", TransferDetailTable()));
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordInsert]", parameters);
                MainWindow.ServerConnection.CloseConnection();
               
                if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                {
                    DepositInsert();
                    if (Properties.Settings.Default.InvoiceCheck == "1")
                    {
                        InvoicePrint(TransferDetailTable());
                        InvoiceControlViewModel vm = new InvoiceControlViewModel();
                        vm.InvoiceNumPlusOneAction();
                        tbInvoiceNum.Content = Properties.Settings.Default.InvoiceNumber.ToString();
                    }
                    ClearPage();
                    MessageWindow.ShowMessage("資料傳送成功！", MessageType.SUCCESS);
                }
                else { MessageWindow.ShowMessage("資料傳送失敗！", MessageType.ERROR); }
            }
            catch (Exception)
            {
                //MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
            }
        }

        private void ReturnSubmit()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否送出退貨資料?", "退貨確認");
            if (!(bool)confirmWindow.DialogResult) { return; }

            try
            {
                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("ChkoutTime", DateTime.Now));
                parameters.Add(new SqlParameter("PreTotal", preTotal));
                parameters.Add(new SqlParameter("RealTotal", realTotal));
                parameters.Add(new SqlParameter("InvoiceNumber", tbInvoiceNum.Content));
                parameters.Add(new SqlParameter("Cashier", cbCashier.SelectedValue));
                parameters.Add(new SqlParameter("Note", tbNote.Text));
                parameters.Add(new SqlParameter("DETAILS", TransferDetailTable()));
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeReturnInsert]", parameters);
                MainWindow.ServerConnection.CloseConnection();

                if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                {
                    if (Properties.Settings.Default.InvoiceCheck == "1")
                    {
                        InvoicePrint(TransferDetailTable());
                        InvoiceControlViewModel vm = new InvoiceControlViewModel();
                        vm.InvoiceNumPlusOneAction();
                        tbInvoiceNum.Content = Properties.Settings.Default.InvoiceNumber.ToString();
                    }
                    ClearPage();
                    MessageWindow.ShowMessage("資料傳送成功！", MessageType.SUCCESS);
                }
                else { MessageWindow.ShowMessage("資料傳送失敗！", MessageType.ERROR); }
            }
            catch (Exception)
            {
                //MessageWindow.ShowMessage("發票列表機設定錯誤", MessageType.ERROR);
            }
        }
        
        private void InvoicePrint(DataTable detail) //9.16發票
        {
            MyPharmacy = Pharmacy.GetCurrentPharmacy();

            SerialPort port = new SerialPort(Properties.Settings.Default.InvoiceComPort, 9600, Parity.None, 8, StopBits.One);
            try
            {
                port.Open();
            }
            catch (Exception e)
            {
                ClearPage();
                MessageWindow.ShowMessage("發票列表機設定錯誤", MessageType.ERROR);
                MessageWindow.ShowMessage(e.Message, MessageType.ERROR);
                return;
            }

            byte[] strArr;
            Encoding big5 = Encoding.GetEncoding("big5");
            char lf = Convert.ToChar(10);
            char cr = Convert.ToChar(13);
            char esc = Convert.ToChar(27);

            port.Write(esc + "@");
            port.Write(esc + "z" + Convert.ToChar(1));
            port.Write(esc + "d" + Convert.ToChar(4));
            strArr = big5.GetBytes(MyPharmacy.Name.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("地址:" + MyPharmacy.Address.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("統編:" + MyPharmacy.ID.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("電話:" + MyPharmacy.Tel.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("客編:" + cusID.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            port.Write(esc + "d" + Convert.ToChar(1));
            strArr = big5.GetBytes("統一編號:" + tbTaxNum.Text.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);

            int j = detail.Rows.Count;
            int priceSum = 0;
            for (int i = 0; i < j; i++)
            {
                strArr = big5.GetBytes(detail.Rows[i]["TraDet_ProductID"].ToString().PadRight(13, ' ')
                    + " *" + detail.Rows[i]["TraDet_Amount"].ToString()
                    + "= " + detail.Rows[i]["TraDet_PriceSum"].ToString().PadLeft(4, ' ') + "TX");
                port.Write(strArr, 0, strArr.Length);
                priceSum += (int)detail.Rows[i]["TraDet_PriceSum"];
                port.Write("" + cr + lf);

                if (i != 0 && (i % 7) == 0 && i != j)
                {
                    strArr = big5.GetBytes("下頁");
                    port.Write(strArr, 0, strArr.Length);
                    port.Write("" + Convert.ToChar(12));
                    port.Write(esc + "d" + Convert.ToChar(4));
                }
            }
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("折價:            "
                + ("-" + discountAmount.ToString()).ToString().PadLeft(5, ' ') + "TX");
            port.Write(strArr, 0, strArr.Length);
            port.Write(esc + "d" + Convert.ToChar(4));
            strArr = big5.GetBytes("實收金額:        $"
                + tbPaid.Text.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("應找金額:        $"
                + lblChange.Content.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("合計:            $"
                + realTotal.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            port.Write("" + Convert.ToChar(29) + Convert.ToChar(86) + Convert.ToChar(66) + cr + lf);
            port.Close();
        }
        
        private void DepositInsert() //9.24寄庫
        {
            foreach (DataRow dr in ProductList.Rows)
            {
                if ((int)dr["Deposit"] != 0)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("DepRec_ProductID", dr["Pro_ID"]));
                    parameters.Add(new SqlParameter("DepRec_Amount", dr["Deposit"]));
                    DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[DepositRecordInsert]", parameters);
                    MainWindow.ServerConnection.CloseConnection();
                }
            }
        }

        #region ----- Events -----

        private void ProductDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var ProductIDList = new List<TextBox>();
            NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox", ref ProductIDList);
            ProductIDList[ProductIDList.Count - 1].Focus();
        }

        #region ProductID

        private void ProductIDTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int currentRowIndex = ProductDataGrid.Items.IndexOf(ProductDataGrid.CurrentItem);

            if (e.Key == Key.Enter)
            {
                if (tb.Text == "") 
                {
                    tbDiscountAmt.Focus();
                }
                else 
                {
                    AddProductByInputAction(tb.Text, currentRowIndex);
                    foreach (DataRow dr in ProductList.Rows)
                    {
                        dr["ID"] = ProductList.Rows.IndexOf(dr) + 1;
                    }
                    // Focus Next Row
                    Dispatcher.InvokeAsync(() =>
                    {
                        var ProductIDList = new List<TextBox>();
                        NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox",
                            ref ProductIDList);
                        ProductIDList[ProductIDList.Count - 1].Focus();
                    }, DispatcherPriority.ApplicationIdle);
                    tb.Text = "";
                }
            }
        }

        private void ProductIDTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text.Length == 13)
            {
                Send(Key.Enter);
            }
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
           /* if (e.Key == Key.Return)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                MoveFocus(request);
            }*/
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int.TryParse(tb.Text, out int amt);
            if (amt < 0) { tb.Text = "0"; }
            int index = GetRowIndexRouted(e);
            int stock = int.Parse(ProductList.Rows[index]["Available_Amount"].ToString());
            if (amt > stock) 
            {
                MessageWindow.ShowMessage("輸入量大於可用量！", MessageType.WARNING);
                tb.Text = stock.ToString(); 
            }
            CalculateTotal("AMT");
        }

        private void next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (ProductList.Rows.Count == 0 || index >= ProductList.Rows.Count) { return; }

            int original = int.Parse(ProductList.Rows[index]["Amount"].ToString());
            int stock = int.Parse(ProductList.Rows[index]["Inv_Inventory"].ToString());
            ProductList.Rows[index]["Amount"] = original + 1;
        }

        private void back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (ProductList.Rows.Count == 0 || index >= ProductList.Rows.Count) { return; }

            int original = int.Parse(ProductList.Rows[index]["Amount"].ToString());
            if (original > 0) { ProductList.Rows[index]["Amount"] = original - 1; }
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
            TextBox tb = (TextBox)sender;
            int.TryParse(tb.Text, out int tryprice);
            if (tryprice < 0)
            {
                tb.Text = "0";
                return;
            }

            CalculateTotal("AMT");
            foreach (DataRow dr in ProductList.Rows)
            {
                dr["PriceTooltip"] = string.Format("{0:F2}", dr["Inv_LastPrice"]) + " / " +
                    (double.Parse(dr["CurrentPrice"].ToString()) -
                    double.Parse(dr["Inv_LastPrice"].ToString())).ToString();
            }
        }

        private void Price_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
            //{
            //    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
            //    MoveFocus(request);
            //}
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

        #region Buttons

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

            if (cbCashier.SelectedItem == null)
            {
                MessageWindow.ShowMessage("尚未選擇結帳人員！", MessageType.ERROR);
                cbCashier.Focus();
                cbCashier.IsDropDownOpen = true;
                return;
            }
            if (ProductList.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("尚未新增商品項目！", MessageType.ERROR);
                FocusLastRow();
                return;
            }

            if (isReturn)
            {
                ReturnSubmit();
            }
            else
            {
                if (GetPayMethod() == "NOT_MATCH")
                {
                    MessageWindow.ShowMessage("付款金額與應收金額不符！", MessageType.WARNING);
                    return;
                }
                CheckoutSubmit();
            }
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            ClearCustomerView();
            if (isReturn)
            {
                isReturn = false;

                tbPaid.IsEnabled = true;
                tbCash.IsEnabled = true;
                tbCard.IsEnabled = true;
                tbVoucher.IsEnabled = true;
                tbCardNum.IsEnabled = true;
                tbTaxNum.IsEnabled = true;
                tbDiscountAmt.IsEnabled = true;
                tbDiscountPer.IsEnabled = true;
                btnGift.IsEnabled = true;

                btnCheckout.Content = "結帳";
                btnCheckout.Background = Brushes.RoyalBlue;
            }
            else
            {
                isReturn = true;

                tbPaid.IsEnabled = false;
                tbCash.IsEnabled = false;
                tbCard.IsEnabled = false;
                tbVoucher.IsEnabled = false;
                tbCardNum.IsEnabled = false;
                tbTaxNum.IsEnabled = false;
                tbDiscountAmt.IsEnabled = false;
                tbDiscountPer.IsEnabled = false;
                btnGift.IsEnabled = false;

                btnCheckout.Content = "退貨";
                btnCheckout.Background = Brushes.IndianRed;
            }
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
                tbCash.Focus();
            }
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
            TextBox tb = (TextBox)sender;
            int index = GetRowIndexRouted(e);
            if (index < ProductList.Rows.Count)
            {
                int.TryParse(ProductList.Rows[index]["Deposit"].ToString(), out int deposit);
                int amount = int.Parse(ProductList.Rows[index]["Amount"].ToString());
                if (deposit > amount)
                {
                    tb.Text = "0";
                    MessageWindow.ShowMessage("寄庫量大於購買數量！", MessageType.ERROR);
                }
            }
        }

        private void Deposit_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Deposit_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void tbCash_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbCard.Focus(); }
        }

        private void tbCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbVoucher.Focus(); }
        }

        private void tbVoucher_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbCardNum.Focus(); }
        }

        private void tbCardNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { tbTaxNum.Focus(); }
        }

        private void tbTaxNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                cbCashier.Focus();
                cbCashier.IsDropDownOpen = true;
            }
        }

        private void cbCashier_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { btnCheckout.Focus(); }
        }

        #endregion

        #region ----- CustomerControl -----

        public void ClearCustomerView()
        {
            tbSearch.Text = "";

            lbName.Content = "";
            lbGender.Content = "";
            lbBirthDay.Content = "";
            lbCellphone.Content = "";
            lbTelephone.Content = "";
            tbAddress.Text = "";
            tbCusNote.Text = "";

            cusID = "0";
            TradeRecordGrid.ItemsSource = null;
            HISRecordGrid.ItemsSource = null;
            DepositColumn.Visibility = Visibility.Hidden;
        }

        private void FillInCustomerData(DataTable result)
        {
            cusID = result.Rows[0]["Cus_ID"].ToString();

            lbName.Content = result.Rows[0]["Cus_Name"].ToString();
            lbGender.Content = result.Rows[0]["Cus_Gender"].ToString();
            if (result.Rows[0]["Cus_Birthday"] == null || result.Rows[0]["Cus_Birthday"].ToString() == "")
            {
                lbBirthDay.Content = "";
            }
            else {
                DateTime dt = (DateTime)result.Rows[0]["Cus_Birthday"];
                CultureInfo culture = new CultureInfo("zh-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                lbBirthDay.Content = dt.ToString("yyy/MM/dd", culture);
            }
            lbCellphone.Content = result.Rows[0]["Cus_Cellphone"].ToString();
            lbTelephone.Content = result.Rows[0]["Cus_Telephone"].ToString();
            tbAddress.Text = result.Rows[0]["Cus_Address"].ToString();
            tbCusNote.Text = result.Rows[0]["Cus_Note"].ToString();

            DepositColumn.Visibility = Visibility.Visible;
        }

        private void GetCustomerTradeRecord()
        {
            //MessageBox.Show(cusID);
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MasterID", DBNull.Value));
            parameters.Add(new SqlParameter("CustomerID", cusID));
            parameters.Add(new SqlParameter("sDate", ""));
            parameters.Add(new SqlParameter("eDate", ""));
            parameters.Add(new SqlParameter("sInvoice", ""));
            parameters.Add(new SqlParameter("eInvoice", ""));
            parameters.Add(new SqlParameter("flag", "2"));
            parameters.Add(new SqlParameter("ShowIrregular", DBNull.Value));
            parameters.Add(new SqlParameter("ShowReturn", DBNull.Value));
            parameters.Add(new SqlParameter("Cashier", -1));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            FormatTradeTime(result);
            TradeRecordGrid.ItemsSource = result.DefaultView;
        }
        private void GetCustomerHISRecord()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CustomerID", cusID));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[HISRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            result.Columns.Add("TransTime_Format", typeof(string));
            foreach (DataRow dr in result.Rows)
            {
                string ogTransTime = dr["AdjustDate"].ToString();
                DateTime dt = DateTime.Parse(ogTransTime);
                CultureInfo culture = new CultureInfo("zh-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                dr["TransTime_Format"] = dt.ToString("yyy/MM/dd", culture);
            }
            HISRecordGrid.ItemsSource = result.DefaultView;
        }
        private void FormatTradeTime(DataTable result)
        {
            result.Columns.Add("TransTime_Format", typeof(string));
            foreach (DataRow dr in result.Rows)
            {
                string ogTransTime = dr["TraMas_ChkoutTime"].ToString();
                DateTime dt = DateTime.Parse(ogTransTime);
                CultureInfo culture = new CultureInfo("zh-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                dr["TransTime_Format"] = dt.ToString("yyy/MM/dd", culture);
            }
        }

        private void acw_RaiseCustomEvent(object sender, CustomEventArgs e)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", int.Parse(e.Message)));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[GetCustomerByID]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (result.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("查無資料！", MessageType.ERROR);
            }
            else
            {
                FillInCustomerData(result);
            }
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (tb.Text.Length == 0)
                {
                    MessageWindow.ShowMessage("查詢位數不足！", MessageType.ERROR);
                   return;
                }

                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                bool isCell = tb.Text.StartsWith("09");
                if (isCell)
                {
                    parameters.Add(new SqlParameter("Cus_Cellphone", tb.Text));
                    parameters.Add(new SqlParameter("Cus_Telephone", DBNull.Value));
                }
                else if (tb.Text.Length >= 7 && tb.Text.Length <= 8)
                {
                    parameters.Add(new SqlParameter("Cus_Cellphone", DBNull.Value));
                    parameters.Add(new SqlParameter("Cus_Telephone", tb.Text));
                }
                else
                {
                    parameters.Add(new SqlParameter("Cus_Cellphone", DBNull.Value));
                    parameters.Add(new SqlParameter("Cus_Telephone", DBNull.Value));
                }
                if (!int.TryParse(tb.Text, out int i))
                {
                    parameters.Add(new SqlParameter("@Cus_Name", tb.Text));
                }
                else {
                    parameters.Add(new SqlParameter("@Cus_Name", DBNull.Value));
                }
                if (tb.Text.Length == 6 )
                {
                    int.TryParse(tb.Text.Substring(0, 2), out int year);
                    int.TryParse(tb.Text.Substring(2, 2), out int month);
                    int.TryParse(tb.Text.Substring(4, 2), out int day);
                    string yearStr = (year + 1911).ToString();
                    string dateStr = yearStr + month.ToString("00") + day.ToString("00");

                    parameters.Add(new SqlParameter("@Cus_Birthday", dateStr));
                }
                else if ((tb.Text.Length == 7 && tb.Text.StartsWith("1"))) {
                    int.TryParse(tb.Text.Substring(0, 3), out int year);
                    int.TryParse(tb.Text.Substring(3, 2), out int month);
                    int.TryParse(tb.Text.Substring(5, 2), out int day);
                    string yearStr = (year + 1911).ToString();
                    string dateStr = yearStr + month.ToString("00") + day.ToString("00");
                    parameters.Add(new SqlParameter("@Cus_Birthday", dateStr));
                }
                else {
                    parameters.Add(new SqlParameter("@Cus_Birthday", DBNull.Value));
                }
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[CustomerQuery]", parameters);
                MainWindow.ServerConnection.CloseConnection();

                if (result.Rows.Count == 0)
                {
                    MessageWindow.ShowMessage("查無資料！", MessageType.ERROR);
                }
                else
                {
                    FillInCustomerData(result);
                    GetCustomerTradeRecord();
                    GetCustomerHISRecord();
                    if (PrescriptionDeclareView.FromPOSCuslblcheck != null)
                    {
                        PrescriptionDeclareView.FromPOSCuslblcheck.Text = tb.Text;
                    }
                }
            }
        }

        private void SearchCustomerFromHIS(string phone)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            bool isCell = phone.StartsWith("09");
            if (isCell)
            {
                parameters.Add(new SqlParameter("Cus_Cellphone", phone));
                parameters.Add(new SqlParameter("Cus_Telephone", DBNull.Value));
            }
            else if (phone.Length >= 7 && phone.Length <= 8)
            {
                parameters.Add(new SqlParameter("Cus_Cellphone", DBNull.Value));
                parameters.Add(new SqlParameter("Cus_Telephone", phone));
            }
            else {
                parameters.Add(new SqlParameter("Cus_Cellphone", DBNull.Value));
                parameters.Add(new SqlParameter("Cus_Telephone", DBNull.Value));
            }
            if (!int.TryParse(phone, out int i))
            {
                parameters.Add(new SqlParameter("@Cus_Name", phone));
            }
            else
            {
                parameters.Add(new SqlParameter("@Cus_Name", DBNull.Value));
            }
            if (phone.Length == 6)
            {
                int.TryParse(phone.Substring(0, 2), out int year);
                int.TryParse(phone.Substring(2, 2), out int month);
                int.TryParse(phone.Substring(4, 2), out int day);
                string yearStr = (year + 1911).ToString();
                string dateStr = yearStr + month.ToString("00") + day.ToString("00");

                parameters.Add(new SqlParameter("@Cus_Birthday", dateStr));
            }
            else if ((phone.Length == 7 && phone.StartsWith("1")))
            {
                int.TryParse(phone.Substring(0, 3), out int year);
                int.TryParse(phone.Substring(3, 2), out int month);
                int.TryParse(phone.Substring(5, 2), out int day);
                string yearStr = (year + 1911).ToString();
                string dateStr = yearStr + month.ToString("00") + day.ToString("00");
                parameters.Add(new SqlParameter("@Cus_Birthday", dateStr));
            }
            else
            {
                parameters.Add(new SqlParameter("@Cus_Birthday", DBNull.Value));
            }
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[CustomerQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            if (result.Rows.Count == 0)
                {
                    
                }
                else
                {
                    FillInCustomerData(result);
                    GetCustomerTradeRecord();
                GetCustomerHISRecord();
                }
            
        }
        public void FillCustomerDirect() {

            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[NewestCustomerQuery]");
            MainWindow.ServerConnection.CloseConnection();

            if (result.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("查無資料！", MessageType.ERROR);
            }
            else
            {
               FillInCustomerData(result);
               GetCustomerTradeRecord();
                GetCustomerHISRecord();
            }
        }
        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            addCustomerWindow = new AddCustomerWindow(customer);
            addCustomerWindow.Closed += new EventHandler(SetContentHandler);

            //AddNewCustomerWindow acw = new AddNewCustomerWindow();
            //acw.RaiseCustomEvent += new EventHandler<CustomEventArgs>(acw_RaiseCustomEvent);
            //acw.ShowDialog();
        }
        private void SetContentHandler(object sender, EventArgs e) {
            addCustomerWindow = null;
            FillCustomerDirect();
            
        }
    
        private void btnClearCustomer_Click(object sender, RoutedEventArgs e)
        {
            ClearCustomerView();
        }

        private void btnDepositManage_Click(object sender, RoutedEventArgs e)
        {
            CustomerDepositManageView cdmv = new CustomerDepositManageView(cusID);
            if (cdmv.CusName != null)
            {
                cdmv.CusName.Content = lbName.Content;
            }
            cdmv.ShowDialog();
        }

        #endregion

        private void tbCUS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbCUS.Text == "1") {
                FillCustomerDirect();
                tbSearch.Text = "";
                tbCUS.Text = "0";
            }
        }

        private void tbFromHIS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbFromHIS.Text.Length>1)
            {
                SearchCustomerFromHIS(tbFromHIS.Text);
                tbSearch.Text = "";
                tbFromHIS.Text = "";
            }
        }

        private void TradeRecordGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TradeRecordGrid.SelectedCells.Count <= 0)
            {
                return;

            }
            else {

                DataRowView row = (DataRowView)TradeRecordGrid.SelectedItems[0];

                DataRow masterRow = row.Row;

                int index = GetRowIndex(e);
                string TradeID = row["TraMas_ID"].ToString();

                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("MasterID", TradeID));
                parameters.Add(new SqlParameter("CustomerID", DBNull.Value));
                parameters.Add(new SqlParameter("sDate", ""));
                parameters.Add(new SqlParameter("eDate", ""));
                parameters.Add(new SqlParameter("sInvoice", ""));
                parameters.Add(new SqlParameter("eInvoice", ""));
                parameters.Add(new SqlParameter("flag", "1"));
                parameters.Add(new SqlParameter("ShowIrregular", DBNull.Value));
                parameters.Add(new SqlParameter("ShowReturn", DBNull.Value));
                parameters.Add(new SqlParameter("Cashier", -1));
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
                MainWindow.ServerConnection.CloseConnection();

                ProductTransactionDetail.ProductTransactionDetail ptd = new ProductTransactionDetail.ProductTransactionDetail(masterRow, result);

                ptd.ShowDialog();
                ptd.Activate();

            }

        }

        private void btnChangeRecord_Click(object sender, RoutedEventArgs e)
        {
            btnChangeRecord.Visibility = Visibility.Collapsed;
            btnChangeHIS.Visibility = Visibility.Visible;
            lbRecord.Content = "消費紀錄";
            HISRecordGrid.Visibility = Visibility.Collapsed;
            TradeRecordGrid.Visibility = Visibility.Visible;

        }

        private void btnChangeHIS_Click(object sender, RoutedEventArgs e)
        {
            btnChangeRecord.Visibility = Visibility.Visible;
            btnChangeHIS.Visibility = Visibility.Collapsed;
            lbRecord.Content = "處方紀錄";
            HISRecordGrid.Visibility = Visibility.Visible;
            TradeRecordGrid.Visibility = Visibility.Collapsed;
        }

        private void HISRecordGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HISRecordGrid.SelectedCells.Count <= 0)
            {
                return;

            }
            else
            {

                DataRowView row = (DataRowView)HISRecordGrid.SelectedItems[0];

                DataRow masterRow = row.Row;

                int index = GetRowIndex(e);
                int TradeID = (int)row["SourceId"];
                string Type = row["Type"].ToString();

                PrescriptionService.ShowPrescriptionEditWindow(TradeID,0);

            }
        }
    }
}
