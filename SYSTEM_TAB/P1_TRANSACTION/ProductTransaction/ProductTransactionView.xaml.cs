using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.Service;

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

        public ProductTransactionView()
        {
            InitializeComponent();
            ProductList = new DataTable();
            ProductDataGrid.ItemsSource = ProductList.DefaultView;
        }

        private void ProductIDTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int currentRowIndex = ProductDataGrid.Items.IndexOf(ProductDataGrid.CurrentItem);
            
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (ProductList.Rows.Count == currentRowIndex)
                {
                    if (!tb.Text.Equals(string.Empty))
                    {
                        AddProductByInputAction(tb.Text);
                        foreach (DataRow dr in ProductList.Rows)
                        {
                            dr["ID"] = ProductList.Rows.IndexOf(dr) + 1;
                        }
                        tb.Text = "";
                    }
                }
            }
        }

        private void AddProductByInputAction(string searchString)
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

                    if (result.Rows.Count == 1)
                    {
                        DataRow NewProduct = result.Rows[0];
                        ProductList.ImportRow(NewProduct);
                        ProductDataGrid.ItemsSource = ProductList.DefaultView;                        
                    }
                    else if (result.Rows.Count > 1)
                    {
                        TradeAddProductWindow tapw = new TradeAddProductWindow(result);
                        tapw.ShowDialog();
                        DataRow NewProduct = tapw.SelectedProduct;
                        ProductList.ImportRow(NewProduct);
                        ProductDataGrid.ItemsSource = ProductList.DefaultView;
                    }

                    Dispatcher.InvokeAsync(() => {
                        var ProductIDList = new List<TextBox>();
                        NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox",
                            ref ProductIDList);
                        ProductIDList[ProductIDList.Count - 1].Focus();
                    }, DispatcherPriority.ApplicationIdle);

                    Calculate_Total();
                }
                else { MessageWindow.ShowMessage("查無此商品", MessageType.WARNING); }
            }
        }

        private void Calculate_Total()
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
            Calculate_Discount("AMT");
            discountAmount = int.Parse(tbDiscountAmt.Text);
            realTotal = preTotal - discountAmount;
            lblRealTotal.Content = realTotal;
        }

        private void Calculate_Discount(string type)
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
        }

        private string GetPayMethod() 
        {
            return "";
        }

        #region ----- Events -----

        private void ProductIDTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text.Length == 13)
            {
                Key key = Key.Enter;
                IInputElement target = Keyboard.FocusedElement;
                RoutedEvent routedEvent = Keyboard.KeyDownEvent;
                target.RaiseEvent(new KeyEventArgs(
                    Keyboard.PrimaryDevice,
                    Keyboard.PrimaryDevice.ActiveSource, 0, key)
                { RoutedEvent = routedEvent });
            }
        }

        private void Amount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "") { tb.Text = "0"; }
            Calculate_Total();
        }

        private void tbDiscountAmt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (preTotal != 0) { Calculate_Discount("AMT"); }
        }

        private void tbDiscountPer_LostFocus(object sender, RoutedEventArgs e)
        {
            if (preTotal != 0) { Calculate_Discount("PER"); }
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow dgr = null;
            DependencyObject visParent = VisualTreeHelper.GetParent(e.OriginalSource as FrameworkElement);
            while (dgr == null && visParent != null)
            {
                dgr = visParent as DataGridRow;
                visParent = VisualTreeHelper.GetParent(visParent);
            }
            if (dgr == null) { return; }
            int rowIdx = dgr.GetIndex();
            if (ProductList.Rows.Count > 0 && rowIdx < ProductList.Rows.Count) 
            {
                ProductList.Rows.Remove(ProductList.Rows[rowIdx]);
            }                
            Calculate_Total();
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
            if (Price != null) { Price.Binding = nb; }
            Calculate_Total();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ConfirmWindow cw = new ConfirmWindow("是否清除頁面資料?", "清除頁面確認");
            if (!(bool)cw.DialogResult) { return; }
            else 
            {
                ProductList.Clear();
                tbDiscountAmt.Text = "0";
                tbNote.Text = "";
                tbCashAmt.Text = "";
                tbTaxNum.Text = "";
                tbCardAmt.Text = "";
                tbCardNum.Text = "";
                tbInvoiceNum.Text = "";
                Calculate_Total();
            }
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            /*string cusID = "";
            DateTime chkoutTime = DateTime.Now;
            string payMethod = getPayMethod();
            // preTotal
            // discountAmount
            // realTotal
            int cashAmt = int.Parse(tbCashAmt.Text);
            int cardAmt = int.Parse(tbCardAmt.Text);
            string cardNum = tbCardNum.Text;
            string invoiceNum = tbInvoiceNum.Text;
            string taxNum = tbTaxNum.Text;
            string cashier = "";
            string note = tbNote.Text;
            DataTable detailDT = new DataTable();*/

            /*MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CustomerID", cusID));
            parameters.Add(new SqlParameter("ChkoutTime", chkoutTime));
            parameters.Add(new SqlParameter("PayMethod", payMethod));
            parameters.Add(new SqlParameter("PreTotal", preTotal));
            parameters.Add(new SqlParameter("DiscountAmt", discountAmount));
            parameters.Add(new SqlParameter("RealTotal", realTotal));
            parameters.Add(new SqlParameter("CardNumber", cardNum));
            parameters.Add(new SqlParameter("InvoiceNumber", invoiceNum));
            parameters.Add(new SqlParameter("TaxNumber", taxNum));
            parameters.Add(new SqlParameter("Cashier", cashier));
            parameters.Add(new SqlParameter("Note", note));
            parameters.Add(new SqlParameter("CashAmount", cashAmt));
            parameters.Add(new SqlParameter("CardAmount", cardAmt));
            parameters.Add(new SqlParameter("DETAILS", detailDT));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordInsert]", parameters);
            MainWindow.ServerConnection.CloseConnection();*/
        }

        #endregion
    }
}
