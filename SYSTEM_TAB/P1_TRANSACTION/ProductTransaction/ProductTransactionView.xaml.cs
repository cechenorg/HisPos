using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    /// <summary>
    /// ProductTransactionView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTransactionView : UserControl
    {
        public DataTable ProductList;
        public string AppliedPrice;

        public string preTotal;
        public string discountAmount;
        public string discountPercent;
        public string realTotal;

        public ProductTransactionView()
        {
            InitializeComponent();
            ProductList = new DataTable();
            ProductDataGrid.ItemsSource = ProductList.DefaultView;            
        }

        private void ProductIDTextbox_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox is null) return;
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (!textBox.Text.Equals(string.Empty)) 
                {
                    AddProductByInputAction(textBox.Text);
                    foreach (DataRow dr in ProductList.Rows) 
                    {
                        dr["ID"] = ProductList.Rows.IndexOf(dr)+1;
                    }
                    textBox.Text = "";
                }

                /*if (ProductDataGrid.CurrentCell.Item.ToString().Equals("{NewItemPlaceholder}") && !textBox.Text.Equals(string.Empty))
                {
                    int oldCount = ProductDataGrid.Items.Count;

                    AddProductByInputAction(textBox.Text);

                    textBox.Text = "";

                    if (ProductDataGrid.Items.Count != oldCount)
                        ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[ProductDataGrid.Items.Count - 2], ProductDataGrid.Columns[3]);
                }
                else if (ProductDataGrid.CurrentCell.Item is Product)
                {
                    if (!(ProductDataGrid.CurrentCell.Item as Product).ID.Equals(textBox.Text))
                        AddProductByInputAction(textBox.Text);

                    List<TextBox> textBoxs = new List<TextBox>();
                    NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox", ref textBoxs);

                    int index = textBoxs.IndexOf(sender as TextBox);

                    if (!(ProductDataGrid.Items[index] as Product).ID.Equals(textBox.Text))
                        textBox.Text = (ProductDataGrid.Items[index] as Product).ID;

                    ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[index], ProductDataGrid.Columns[3]);
                }

                ProductDataGrid.SelectedItem = ProductDataGrid.CurrentCell.Item;

                var focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                if (focusedCell != null) 
                {
                    UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    if (firstChild is TextBox)
                        firstChild.Focus();
                }*/      
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
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(searchString, AddProductEnum.Trade);
            MainWindow.ServerConnection.CloseConnection();
            if (productCount == 0)
                MessageWindow.ShowMessage("查無商品", MessageType.WARNING);
            else
            {
                if (productCount > 0)
                {
                    int WareID = 0;
                    MainWindow.ServerConnection.OpenConnection();
                    var parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("SEARCH_STRING", searchString));
                    parameters.Add(new SqlParameter("WAREHOUSE_ID", WareID));
                    var result = MainWindow.ServerConnection.ExecuteProc("[Get].[SearchProductsByID]", parameters);
                    string res = string.Join(Environment.NewLine, result.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
                    //MessageWindow.ShowMessage(result.Rows.Count.ToString(), MessageType.WARNING);
                    MainWindow.ServerConnection.CloseConnection();

                    if (ProductList.Rows.Count == 0) 
                    {
                        ProductList = result.Clone();
                        ProductList.Columns.Add("ID", typeof(int));
                        ProductList.Columns.Add("Amount", typeof(int));
                        ProductList.Columns.Add("Calc", typeof(double));
                    }
                        

                    TradeAddProductWindow tapw = new TradeAddProductWindow(result);
                    tapw.ShowDialog();
                    DataRow NewProduct = tapw.SelectedProduct;
                    ProductList.ImportRow(NewProduct);
                    ProductDataGrid.ItemsSource = ProductList.DefaultView;
                }
                else
                {
                    MessageWindow.ShowMessage("查無此商品", MessageType.WARNING);
                }
            }
        }

        private void Calculate_Calc()
        {
            if (ProductDataGrid.Items.Count > 0)
            {
                foreach (DataRow dr in ProductList.Rows)
                {
                    dr["Calc"] = int.Parse(dr[AppliedPrice].ToString()) * int.Parse(dr["Amount"].ToString());
                }
                preTotal = ProductList.Compute("SUM(Calc)", string.Empty).ToString();
                lblPreTotal.Content = preTotal;
            }
        }

        private void Calculate_Discount(string type)
        {
            double pt = double.Parse(preTotal);
            if (type == "AMT" && tbDiscountAmt.Text != "")
            {
                double amt = double.Parse(tbDiscountAmt.Text);
                tbDiscountPer.Text = ((pt - amt) / pt * 100).ToString().Replace("0", "");
            }
            else if (type == "PER" && tbDiscountPer.Text != "")
            {
                double per = double.Parse(tbDiscountPer.Text);
                if (per > 10)
                    tbDiscountAmt.Text = (pt - pt * per / 100).ToString();
                else
                    tbDiscountAmt.Text = (pt - pt * per / 10).ToString();
            }
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
            Price.Binding = nb;
            Calculate_Calc();
        }
        
        private void Amount_LostFocus(object sender, RoutedEventArgs e)
        {
            Calculate_Calc();
        }

        private void tbDiscountAmt_LostFocus(object sender, RoutedEventArgs e)
        {
            Calculate_Discount("AMT");
        }

        private void tbDiscountPer_LostFocus(object sender, RoutedEventArgs e)
        {
            Calculate_Discount("PER");
        }
    }
}
