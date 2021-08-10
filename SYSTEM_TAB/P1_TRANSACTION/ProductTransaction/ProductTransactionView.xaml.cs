using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddCustomerWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Medicine.NotEnoughMedicine;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Product;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSearchWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.CustomerDataControl;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.FunctionWindow.NotEnoughOTCPurchaseWindow;
using His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.InvoiceControl;
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
        public string TbText;
        private DataTable ProductList;
        private string AppliedPrice;
        private int preTotal = 0;
        private int discountAmount = 0;
        private int realTotal = 0;
        private double totalProfit = 0;
        private string cusID = "0";
        private string PrepayBalance = "0";
        private bool isGift = false;
        private string PrepayProID = "PREPAY";

        private CheckoutWindowView chkWindow;

        public AddCustomerWindow addCustomerWindow;
        public int ID;
        public CustomerSearchCondition Con;

        private static readonly Regex _regex = new Regex("^[0-9]+$");

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public Pharmacy MyPharmacy;
        public NewClass.Prescription.Prescription CurrentPrescription { get; set; }

        public ProductTransactionView()
        {
            InitializeComponent();

            InvoiceNumLable = tbInvoiceNum;
            Cuslblcheck = tbCUS;
            FromHISCuslblcheck = tbFromHIS;
            ProductList = new DataTable();
            ProductDataGrid.ItemsSource = ProductList.DefaultView;

            if (Properties.Settings.Default.InvoiceCheck == "1")
            {
                tbInvoiceNum.Content = Properties.Settings.Default.InvoiceNumberEng.ToString() + Properties.Settings.Default.InvoiceNumber.ToString();
            }
            else
            {
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

        private void GetNotEnoughOTCs()
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
                    var foundRow = check.Select("OTC_Code= '" + dr["Pro_ID"].ToString() + "'");
                    var foundRowBar = check.Select("OTC_Barcode= '" + dr["Pro_ID"].ToString() + "'");
                    if (foundRow.Length > 0 || foundRowBar.Length > 0)
                    {
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

        // 新增品項
        private void AddProductByInputAction(string searchString, int rowIndex)
        {
            if (string.IsNullOrEmpty(searchString)) return;
            if (searchString.Length == 0)
            {
                if (rowIndex < ProductList.Rows.Count)
                {
                    ProductList.Rows.RemoveAt(rowIndex);
                    CalculateTotal();
                }
            }

            if (ProductList.Rows.Count > 0 && ProductList.Rows[0]["Pro_ID"].ToString() == PrepayProID) 
            {
                MessageWindow.ShowMessage("預付訂金須單獨結帳", MessageType.ERROR);
                return;
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
                if (rowIndex < ProductList.Rows.Count && rowIndex >= 0)
                {
                    ProductList.Rows.RemoveAt(rowIndex);
                    CalculateTotal();
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

                        if (result.Rows[0]["Pro_ID"].ToString() == PrepayProID) 
                        {
                            result.Rows[0]["Pro_TypeID"] = 0;
                        }

                        int amt = int.Parse(result.Rows[0]["Available_Amount"].ToString());
                        if (amt < 1 && result.Rows[0]["Pro_ID"].ToString() != PrepayProID)
                        {
                            MessageWindow.ShowMessage("該品項可用量不足", MessageType.WARNING);
                            return;
                        }
                        if (rowIndex < ProductList.Rows.Count && rowIndex >= 0)
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
                        if (NewProduct != null)
                        {
                            newRow.ItemArray = NewProduct.ItemArray;
                            int amt = int.Parse(NewProduct["Available_Amount"].ToString());
                            if (amt < 1 && result.Rows[0]["Pro_ID"].ToString() != PrepayProID)
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

                    //FocusLastRow();
                    SetPrice();

                    if (isGift)
                    {
                        ProductList.Rows[rowIndex]["CurrentPrice"] = 0;
                        ProductList.Rows[rowIndex]["IsGift"] = 1;
                        isGift = false;
                        btnGift.IsEnabled = true;
                    }
                    CalculateTotal();
                }
                else { MessageWindow.ShowMessage("查無此商品", MessageType.WARNING); }
            }
        }

        private void SetPrice()
        {
            if (ProductList != null)
            {
                int rc = 0;
                int count = ProductList.Rows.Count;
                foreach (DataRow dr in ProductList.Rows)
                {
                    if (rc == count-1) 
                    {
                        bool tp = int.TryParse(dr["IsGift"].ToString(), out int ig);
                        //if (!tp || ig != 1 && dr["CurrentPrice"].ToString() == "0")
                        if (!tp || ig != 1)
                        {
                            dr["CurrentPrice"] = dr[AppliedPrice];
                        }
                    }
                    rc++;
                }
            }
        }

        private void CalculateTotal()
        {
            if (ProductList == null) { return; }
            if (ProductList.Rows.Count > 0)
            {
                foreach (DataRow dr in ProductList.Rows)
                {
                    if (dr["CurrentPrice"].ToString() != "" && dr["Amount"].ToString() != "")
                    {
                        dr["Calc"] = int.Parse(dr["CurrentPrice"].ToString()) * int.Parse(dr["Amount"].ToString());
                        double profit = (double.Parse(dr["CurrentPrice"].ToString()) -
                        double.Parse(dr["AVGVALUE"].ToString())) * int.Parse(dr["Amount"].ToString());
                        dr["Profit"] = string.Format("{0:F2}", profit);
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
            CalculateDiscount();
        }

        private async void CalculateDiscount()
        {
            discountAmount = int.Parse(tbDiscountAmt.Text);
            realTotal = preTotal - discountAmount;
            lblRealTotal.Content = realTotal;
            TaxNum.Content = ((int)(realTotal * 0.05)).ToString();
            NOTaxNum.Content = (realTotal - ((int)(realTotal * 0.05))).ToString();

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
                CalculateTotal();
                await Task.Delay(20);
                tbDiscountAmt.Focus();
            }
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
            AppliedPrice = "Pro_RetailPrice";

            CalculateTotal();
            PriceCombo.SelectedIndex = 0;

            chkWindow = null;
        }

        private void CheckoutSubmit()
        {
            if (chkWindow == null) { return; }
            GetNotEnoughOTCs();

            ConfirmWindow confirmWindow = new ConfirmWindow("是否送出結帳資料?", "結帳確認", true);
            if (!(bool)confirmWindow.DialogResult) { return; }

            try
            {
                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("CustomerID", cusID));
                parameters.Add(new SqlParameter("ChkoutTime", DateTime.Now));
                parameters.Add(new SqlParameter("PayMethod", chkWindow.PayMethod));
                parameters.Add(new SqlParameter("CashAmount", chkWindow.Cash));
                parameters.Add(new SqlParameter("VoucherAmount", chkWindow.Voucher));
                parameters.Add(new SqlParameter("CashCouponAmount", chkWindow.CashCoupon));
                parameters.Add(new SqlParameter("CardAmount", chkWindow.Card));
                parameters.Add(new SqlParameter("PrepayAmount", chkWindow.Prepay));
                parameters.Add(new SqlParameter("PreTotal", preTotal));
                parameters.Add(new SqlParameter("DiscountAmt", discountAmount));
                parameters.Add(new SqlParameter("RealTotal", realTotal));
                parameters.Add(new SqlParameter("CardNumber", chkWindow.CardNumber));
                if (Properties.Settings.Default.InvoiceCheck == "0")
                {
                    parameters.Add(new SqlParameter("InvoiceNumber", ""));
                }
                else {
                    parameters.Add(new SqlParameter("InvoiceNumber", Properties.Settings.Default.InvoiceNumberEng+Properties.Settings.Default.InvoiceNumber));
                }
                parameters.Add(new SqlParameter("TaxNumber", chkWindow.TaxNumber));
                parameters.Add(new SqlParameter("Cashier", chkWindow.Employee));
                parameters.Add(new SqlParameter("Note", tbNote.Text));
                parameters.Add(new SqlParameter("DETAILS", TransferDetailTable()));
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordInsert]", parameters);
                MainWindow.ServerConnection.CloseConnection();

                if (result.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                {
                    DepositInsert();
                    if (Properties.Settings.Default.InvoiceCheck == "1" && ProductList.Rows[0]["Pro_ID"].ToString() != PrepayProID)
                    {
                        if (Properties.Settings.Default.InvoiceNumber.Length != 8)
                        {
                            MessageWindow.ShowMessage("請重新設定發票！", MessageType.ERROR);
                            return;
                        }
                        tbInvoiceNum.Content = Properties.Settings.Default.InvoiceNumberEng.ToString()+Properties.Settings.Default.InvoiceNumber.ToString();
                        InvoicePrint();
                        InvoiceControlViewModel vm = new InvoiceControlViewModel();
                        vm.InvoiceNumPlusOneAction();
                        tbInvoiceNum.Content = Properties.Settings.Default.InvoiceNumberEng.ToString() + Properties.Settings.Default.InvoiceNumber.ToString();
                    }
                    ClearPage();
                    MessageWindow.ShowMessage("資料傳送成功！", MessageType.SUCCESS);
                }
                else { MessageWindow.ShowMessage(result.Rows[0].Field<string>("RESULT"), MessageType.ERROR); }
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
            }
        }

        private void InvoicePrint() //9.16發票
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

            strArr = big5.GetBytes(MyPharmacy.Name.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("地址:" + MyPharmacy.Address.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("電話:" + MyPharmacy.Tel.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("賣方:" + MyPharmacy.TAXNUM.ToString());
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("買方:" + chkWindow.TaxNumber);
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            port.Write("" + cr + lf);

            int j = ProductList.Rows.Count;
            int priceSum = 0;
            for (int i = 0; i < j; i++)
            {
                if (ProductList.Rows[i]["Pro_TypeID"].ToString() == "1") {
                    strArr = big5.GetBytes("配藥".PadRight(13, ' ').Substring(0, 13));
                    port.Write(strArr, 0, strArr.Length);
                    port.Write("" + cr + lf);
                }
                else if (ProductList.Rows[i]["Pro_ID"].ToString() == PrepayProID) { } //預收訂金
                else {
                    strArr = big5.GetBytes(ProductList.Rows[i]["Pro_ChineseName"].ToString().PadRight(13, ' ').Substring(0, 13));
                    port.Write(strArr, 0, strArr.Length);
                    port.Write("" + cr + lf);
                }
                
                strArr = big5.GetBytes(" *" + ProductList.Rows[i]["Amount"].ToString() + "= " + ProductList.Rows[i]["Calc"].ToString().PadLeft(4, ' ') + "TX");
                port.Write(strArr, 0, strArr.Length);
                priceSum += (int)ProductList.Rows[i]["Calc"];
                port.Write("" + cr + lf);

                if (i != 0 && (i % 7) == 0 && i != j)
                {
                    strArr = big5.GetBytes("下頁");
                    port.Write(strArr, 0, strArr.Length);
                    port.Write("" + Convert.ToChar(12));
                    port.Write(esc + "d" + Convert.ToChar(4));
                    InvoiceControlViewModel vm = new InvoiceControlViewModel();
                    vm.InvoiceNumPlusOneAction();
                    tbInvoiceNum.Content = Properties.Settings.Default.InvoiceNumberEng.ToString() + Properties.Settings.Default.InvoiceNumber.ToString();
                }
            }
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("折價:            "
                + ("-" + discountAmount.ToString()).ToString().PadLeft(5, ' ') + "TX");
            port.Write(strArr, 0, strArr.Length); 
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("實收金額:        $" + chkWindow.Paid);
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("應找金額:        $" + chkWindow.Change);
            port.Write(strArr, 0, strArr.Length);
            port.Write("" + cr + lf);
            strArr = big5.GetBytes("合計:            $" + realTotal.ToString());
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

        private void CheckoutActions() 
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
                MessageWindow.ShowMessage("尚無結帳商品！", MessageType.ERROR);
                FocusLastRow();
                return;
            }
            if (Properties.Settings.Default.InvoiceNumber.Length != 8 && Properties.Settings.Default.InvoiceCheck=="1")
            {
                MessageWindow.ShowMessage("請確認發票設定！", MessageType.ERROR);
                return;
            }
            if (Math.Ceiling((double)ProductList.Rows.Count / 7) >= 1 && Properties.Settings.Default.InvoiceCheck == "1")
            {
                int num = int.Parse(Properties.Settings.Default.InvoiceNumber);
                int Snum = int.Parse(Properties.Settings.Default.InvoiceNumberStart);
                int Count = int.Parse(Properties.Settings.Default.InvoiceNumberCount);
                if ((Count - (num - Snum)) < Math.Ceiling((double)ProductList.Rows.Count / 7))
                {
                    MessageWindow.ShowMessage("發票剩餘張數不夠 請檢查設定！", MessageType.ERROR);
                    return;
                }
            }

            int rowCount = ProductList.Rows.Count;
            int amtCount = int.Parse(ProductList.Compute("Sum(Amount)", string.Empty).ToString());
            bool hasCustomer = int.Parse(cusID) > 0 ? true : false;
            bool isPrepay = ProductList.Rows[0]["Pro_ID"].ToString() == PrepayProID;
            chkWindow = new CheckoutWindowView(realTotal, rowCount, amtCount, PrepayBalance, hasCustomer, isPrepay);
            chkWindow.ShowDialog();
            if ((bool)chkWindow.DialogResult)
            {
                CheckoutSubmit();
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
                    e.Handled = true;
                }
                else
                {
                    AddProductByInputAction(tb.Text, currentRowIndex);
                    foreach (DataRow dr in ProductList.Rows)
                    {
                        dr["ID"] = ProductList.Rows.IndexOf(dr) + 1;
                    }
                    // Focus On Price
                    Dispatcher.InvokeAsync(() =>
                    {
                        var ProductIDList = new List<TextBox>();
                        NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox",
                            ref ProductIDList);
                        ProductIDList[ProductIDList.Count - 1].Focus();
                        //var ProductIDList = new List<TextBox>();
                        //NewFunction.FindChildGroup(ProductDataGrid, "Price",
                        //    ref ProductIDList);
                        //ProductIDList[currentRowIndex].Focus();
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
                //Send(Key.Enter);
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

        #endregion ProductID

        #region Amount

        private void Amount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (IsTextAllowed(tb.Text)) { tb.Text = ""; }
            if (tb.Text == "") { tb.Text = "0"; }
            CalculateTotal();
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int index = GetRowIndexRouted(e);
            if (index > ProductList.Rows.Count - 1) { return; }
            TextBox tb = (TextBox)sender;
            int.TryParse(tb.Text, out int amt);
            if (amt < 0) { tb.Text = "0"; }
            int.TryParse(ProductList.Rows[index]["Available_Amount"].ToString(), out int stock);
            if (amt > stock && ProductList.Rows[index]["Pro_ID"].ToString() != PrepayProID)
            {
                MessageWindow.ShowMessage("輸入量大於可用量！", MessageType.WARNING);
                tb.Text = stock.ToString();
            }
            CalculateTotal();
        }

        private void Amount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Dispatcher.InvokeAsync(() =>
                {
                    var ProductIDList = new List<TextBox>();
                    NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox",
                        ref ProductIDList);
                    ProductIDList[ProductIDList.Count - 1].Focus();
                }, DispatcherPriority.ApplicationIdle);
            }
                
        }

        #endregion Amount

        #region Price

        private void Price_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (IsTextAllowed(tb.Text)) { tb.Text = ""; }
            if (tb.Text == "") { tb.Text = "0"; }
            CalculateTotal();
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

            CalculateTotal();
            foreach (DataRow dr in ProductList.Rows)
            {
                double profit = (double.Parse(dr["CurrentPrice"].ToString()) -
                    double.Parse(dr["AVGVALUE"].ToString()));
                dr["PriceTooltip"] = string.Format("{0:F2}", dr["AVGVALUE"]) + " / " +
                    string.Format("{0:F2}", profit);
            }
        }

        private void Price_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                int currentRowIndex = ProductDataGrid.Items.IndexOf(ProductDataGrid.CurrentItem);
                Dispatcher.InvokeAsync(() =>
                {
                    var ProductIDList = new List<TextBox>();
                    NewFunction.FindChildGroup(ProductDataGrid, "Amount",
                        ref ProductIDList);
                    ProductIDList[currentRowIndex].Focus();
                }, DispatcherPriority.ApplicationIdle);
            }
        }

        #endregion Price

        #region Discount

        private void tbDiscountAmt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (IsTextAllowed(tb.Text)) { tb.Text = ""; }
            if (tbDiscountAmt.Text == "")
            {
                tbDiscountAmt.Text = "0";
                CalculateTotal();
                return;
            }
            if (preTotal != 0 && int.Parse(tbDiscountAmt.Text) >= 0) { CalculateTotal(); }
            else { tbDiscountAmt.Text = "0"; }
        }

        private void tbDiscountAmt_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!IsTextAllowed(tb.Text)) { CalculateTotal(); }
        }

        #endregion Discount

        #region Buttons

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ConfirmWindow cw = new ConfirmWindow("是否清除頁面資料?", "清除頁面確認");
            if (!(bool)cw.DialogResult) { return; }
            else { ClearPage(); }
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            CheckoutActions();
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

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (ProductList.Rows.Count > 0 && index < ProductList.Rows.Count)
            {
                ProductList.Rows.Remove(ProductList.Rows[index]);
            }
            CalculateTotal();
        }

        #endregion Buttons

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
            CalculateTotal();
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

        #endregion ----- Events -----

        #region ----- CustomerControl -----

        public void ClearCustomerView()
        {
            tbSearch.Text = "";

            lbName.Content = "";
            lbGender.Content = "";
            lbBirthDay.Content = "";
            lbCellphone.Content = "";
            lbSecondphone.Content = "";
            lbTelephone.Content = "";
            tbAddress.Text = "";
            tbCusNote.Text = "";

            cusID = "0";
            PrepayBalance = "0";
            TradeRecordGrid.ItemsSource = null;
            HISRecordGrid.ItemsSource = null;
            DepositColumn.Visibility = Visibility.Hidden;
            btnPrepay.IsEnabled = false;
            AppliedPrice = "Pro_RetailPrice";
            SetPrice();
            CalculateTotal();
        }

        private void FillInCustomerData(DataTable result)
        {
            cusID = result.Rows[0]["Cus_ID"].ToString();
            string res = result.Rows[0]["Prepay_Balance"].ToString();
            PrepayBalance = string.IsNullOrEmpty(res) ? "0" : res;
            lbName.Content = result.Rows[0]["Cus_Name"].ToString();
            lbGender.Content = result.Rows[0]["Cus_Gender"].ToString();
            if (result.Rows[0]["Cus_Birthday"] == null || result.Rows[0]["Cus_Birthday"].ToString() == "")
            {
                lbBirthDay.Content = "";
            }
            else
            {
                DateTime dt = (DateTime)result.Rows[0]["Cus_Birthday"];
                CultureInfo culture = new CultureInfo("zh-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                lbBirthDay.Content = dt.ToString("yyy/MM/dd", culture);
            }
            lbCellphone.Content = result.Rows[0]["Cus_Cellphone"].ToString();
            lbSecondphone.Content= result.Rows[0]["Cus_Secondphone"].ToString();
            lbTelephone.Content = result.Rows[0]["Cus_Telephone"].ToString();
            tbAddress.Text = result.Rows[0]["Cus_Address"].ToString();
            tbCusNote.Text = result.Rows[0]["Cus_Note"].ToString();

            AppliedPrice = "Pro_MemberPrice";
            SetPrice();
            CalculateTotal();
            DepositColumn.Visibility = Visibility.Visible;
            btnPrepay.IsEnabled = true;
        }

        private void GetCustomerTradeRecord()
        {
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
            parameters.Add(new SqlParameter("ProID", DBNull.Value));
            parameters.Add(new SqlParameter("ProName", DBNull.Value));
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
                    ConfirmWindow cw = new ConfirmWindow("是否新增客戶?", "新增客戶確認");
                    if (!(bool)cw.DialogResult) { return; }
                    else
                    {
                        NewClass.Person.Customer.Customer customer = new NewClass.Person.Customer.Customer();

                        if (TbText != null) { }
                        {
                            customer.CellPhone = tbSearch.Text;
                        }

                        addCustomerWindow = new AddCustomerWindow(customer);
                        addCustomerWindow.Closed += new EventHandler(SetContentHandler);
                    }

                }
                else if (result.Rows.Count > 1)
                {
                    CustomerSearchWindow customerSearch;
                    if (Con == CustomerSearchCondition.Birthday)
                    {
                        Messenger.Default.Register<NotificationMessage<NewClass.Person.Customer.Customer>>(this, GetSelectedCustomer);
                        var twCulture = new System.Globalization.CultureInfo("zh-TW", true);
                        twCulture.DateTimeFormat.Calendar = new System.Globalization.TaiwanCalendar();

                        var dateString = tb.Text.Trim();
                        dateString = dateString.PadLeft(8, '0');
                        var date = DateTime.ParseExact(dateString, "yMMdd", twCulture);

                        customerSearch = new CustomerSearchWindow(date);
                        Messenger.Default.Unregister<NotificationMessage<NewClass.Person.Customer.Customer>>(this);
                    }
                    else
                    {
                        Messenger.Default.Register<NotificationMessage<NewClass.Person.Customer.Customer>>(this, GetSelectedCustomer);
                        customerSearch = new CustomerSearchWindow(Con, 0, tb.Text.Trim());
                        Messenger.Default.Unregister<NotificationMessage<NewClass.Person.Customer.Customer>>(this);
                    }

                    if (ID != 0)
                    {
                        MainWindow.ServerConnection.OpenConnection();
                        List<SqlParameter> parameters1 = new List<SqlParameter>();
                        parameters1.Add(new SqlParameter("ID", ID));
                        DataTable result1 = MainWindow.ServerConnection.ExecuteProc("[POS].[CustomerQueryByID]", parameters1);
                        MainWindow.ServerConnection.CloseConnection();
                        FillInCustomerData(result1);
                        GetCustomerTradeRecord();
                        GetCustomerHISRecord();
                        if (PrescriptionDeclareView.FromPOSCuslblcheck != null)
                        {
                            PrescriptionDeclareView.FromPOSCuslblcheck.Text = tb.Text;
                        }
                    }
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

                ID = CurrentPrescription.Patient.ID;
               
            }
        }

        private void SearchCustomerFromHIS(string phone)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", phone));
            
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[CustomerQueryByID]", parameters);
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

        public void FillCustomerDirect()
        {
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
            NewClass.Person.Customer.Customer customer = new NewClass.Person.Customer.Customer();
          
            if (TbText != null) { }
            {
                customer.CellPhone = tbSearch.Text;
            }

            addCustomerWindow = new AddCustomerWindow(customer);
            addCustomerWindow.Closed += new EventHandler(SetContentHandler);

            //AddNewCustomerWindow acw = new AddNewCustomerWindow();
            //acw.RaiseCustomEvent += new EventHandler<CustomEventArgs>(acw_RaiseCustomEvent);
            //acw.ShowDialog();
        }

        private void SetContentHandler(object sender, EventArgs e)
        {
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

        #endregion ----- CustomerControl -----

        private void tbCUS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbCUS.Text == "1")
            {
                FillCustomerDirect();
                tbSearch.Text = "";
                tbCUS.Text = "0";
            }
        }

        private void tbFromHIS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbFromHIS.Text.Length > 1)
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
            else
            {
                DataRowView row = (DataRowView)TradeRecordGrid.SelectedItems[0];
                DataRow masterRow = row.Row;

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
                parameters.Add(new SqlParameter("ProID", DBNull.Value));
                parameters.Add(new SqlParameter("ProName", DBNull.Value));
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
                int TradeID = (int)row["SourceId"];
                PrescriptionService.ShowPrescriptionEditWindow(TradeID, 0);
            }
        }

        private void tbDiscountAmt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Dispatcher.BeginInvoke(new Action(() => tb.SelectAll()));
        }

        private void tbDiscountAmt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnCheckout.Focus();
                e.Handled = true;
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) 
            {
                CheckoutActions();
                e.Handled = true;
            }
        }

        private void lbName_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (cusID is null) return;

            CustomerManageViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).CustomerManageView;

            Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, cusID, ""));
        }

        private void ProductDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Key == Key.Enter)
            {
                e.Handled = true;
                var TabKey = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, Key.Tab);
                TabKey.RoutedEvent = Keyboard.KeyDownEvent;
                InputManager.Current.ProcessInput(TabKey);
            }*/
        }

        private void btnPrepay_Click(object sender, RoutedEventArgs e)
        {
            int currentRowIndex = ProductDataGrid.Items.IndexOf(ProductDataGrid.CurrentItem);
            if (isGift) 
            {
                MessageWindow.ShowMessage("預付訂金不得當作贈品", MessageType.ERROR);
                isGift = false;
            }
            if (ProductList.Rows.Count == 0)
            {
                AddProductByInputAction(PrepayProID, currentRowIndex);
                foreach (DataRow dr in ProductList.Rows)
                {
                    dr["ID"] = ProductList.Rows.IndexOf(dr) + 1;
                }
                // Focus On Price
                Dispatcher.InvokeAsync(() =>
                {
                    var ProductIDList = new List<TextBox>();
                    NewFunction.FindChildGroup(ProductDataGrid, "Amount",
                        ref ProductIDList);
                    TextBox tb = ProductIDList[ProductIDList.Count - 2];
                    tb.Focus();
                    tb.SelectAll();
                }, DispatcherPriority.ApplicationIdle);
            }
            else 
            {
                MessageWindow.ShowMessage("預付訂金須單獨結帳", MessageType.ERROR);
            }
        }
    }
}