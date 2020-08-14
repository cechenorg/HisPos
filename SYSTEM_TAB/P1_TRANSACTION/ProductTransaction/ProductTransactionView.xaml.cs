using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    /// <summary>
    /// ProductTransactionView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTransactionView : UserControl
    {
        public DataTable ProductList;
        public string AppliedPrice;

        public int preTotal = 0;
        public int discountAmount = 0;
        public int realTotal = 0;

        private static readonly Regex _regex = new Regex("^[0-9]+$");
        private static bool IsTextAllowed(string text) { return !_regex.IsMatch(text); }

        public ProductTransactionView()
        {
            InitializeComponent();
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

        private void AddProductByInputAction(string searchString, int rowIndex)
        {
            if (string.IsNullOrEmpty(searchString)) return;
            if (searchString.Length < 5)
            {
                MessageWindow.ShowMessage("搜尋字長度不得小於5", MessageType.WARNING);
                return;
            }
            foreach (DataRow dr in ProductList.Rows) 
            {
                if (dr["Pro_ID"].ToString() == searchString) 
                {
                    dr["Amount"] = int.Parse(dr["Amount"].ToString()) + 1;
                    return;
                }
            }

            MainWindow.ServerConnection.OpenConnection();
            int productCount = ProductStructs.GetProductStructCountBySearchString(searchString, AddProductEnum.Trade);
            MainWindow.ServerConnection.CloseConnection();

            if (productCount == 0) { MessageWindow.ShowMessage("查無商品", MessageType.WARNING); }
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
                        ProductList.Columns.Add("ID", typeof(int));
                        DataColumn amt = new DataColumn("Amount", typeof(int));
                        amt.DefaultValue = 1;
                        ProductList.Columns.Add(amt);
                        ProductList.Columns.Add("Calc", typeof(double));
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
                    Dispatcher.InvokeAsync(() => {
                        var ProductIDList = new List<TextBox>();
                        NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox",
                            ref ProductIDList);
                        ProductIDList[ProductIDList.Count - 1].Focus();
                    }, DispatcherPriority.ApplicationIdle);

                    CalculateTotal("AMT");
                }
                else { MessageWindow.ShowMessage("查無此商品", MessageType.WARNING); }
            }
        }

        private void CalculateTotal(string type)
        {
            if (ProductList == null) { return; }
            if (ProductList.Rows.Count > 0)
            {
                foreach (DataRow dr in ProductList.Rows)
                {
                    dr["Calc"] = int.Parse(dr[AppliedPrice].ToString()) * int.Parse(dr["Amount"].ToString());
                }
                preTotal = int.Parse(ProductList.Compute("SUM(Calc)", string.Empty).ToString());
                lblPreTotal.Content = preTotal;
            }
            if (ProductList.Rows.Count == 0)
            {
                preTotal = 0;
                lblPreTotal.Content = preTotal;
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
            foreach (DataRow dr in ProductList.Rows)
            {
                dt.Rows.Add(
                    dr["ID"],
                    dr["Pro_ID"],
                    dr["Amount"],
                    AppliedPrice,
                    dr[AppliedPrice],
                    dr["Calc"]);
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
            AppliedPrice = "Pro_RetailPrice";
            CalculateTotal("AMT");
        }

        #region ----- Events -----

        private void ProductDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var ProductIDList = new List<TextBox>();
            NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox",
                ref ProductIDList);
            ProductIDList[ProductIDList.Count - 1].Focus();
        }

        private void ProductIDTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int currentRowIndex = ProductDataGrid.Items.IndexOf(ProductDataGrid.CurrentItem);

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (!tb.Text.Equals(string.Empty))
                {
                    AddProductByInputAction(tb.Text, currentRowIndex);
                    foreach (DataRow dr in ProductList.Rows)
                    {
                        dr["ID"] = ProductList.Rows.IndexOf(dr) + 1;
                    }
                    if (currentRowIndex == ProductList.Rows.Count - 1) { tb.Text = ""; }
                }
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

        private void Amount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!IsTextAllowed(tb.Text)) { tb.Text = ""; }
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

        private void tbDiscountAmt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!IsTextAllowed(tb.Text)) { tb.Text = ""; }
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
            if (!IsTextAllowed(tb.Text)) { tb.Text = ""; }
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
            Binding nb = new Binding();
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
            nb.Path = new PropertyPath(AppliedPrice);
            if (Price!=null) { Price.Binding = nb; } 
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

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotal("AMT");
        }

        private void tbPaid_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculateChange();
        }

        private void tbPaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { CalculateChange(); }
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
            ConfirmWindow confirmWindow = new ConfirmWindow("是否送出結帳資料?", "結帳確認");
            if (!(bool)confirmWindow.DialogResult) { return; }

            string cusID = "0";
            string payMethod = (bool)rbCash.IsChecked ? "現金" : "信用卡";

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CustomerID", cusID));
            parameters.Add(new SqlParameter("ChkoutTime", DateTime.Now));
            parameters.Add(new SqlParameter("PayMethod", payMethod));
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

        private void lblProductName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = GetRowIndex(e);
            if (index < ProductList.Rows.Count)
            {
                string proID = ProductList.Rows[index]["Pro_ID"].ToString();
                ProductDetailWindow.ShowProductDetailWindow();
                Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { proID, "0" }, "ShowProductDetail"));
            }
        }

        #endregion

        
    }
}
