using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using MahApps.Metro.Controls;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    /// <summary>
    /// ProductTransactionView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTransactionView : UserControl
    {
        public DataTable ProductList;
        public string AppliedPrice;

        public string preTotal = "0";
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
                    MainWindow.ServerConnection.CloseConnection();

                    TradeAddProductWindow tapw = new TradeAddProductWindow(result);
                    tapw.ShowDialog();

                    if (ProductList.Rows.Count == 0)
                    {
                        ProductList = result.Clone();
                        ProductList.Columns.Add("ID", typeof(int));
                        DataColumn amt = new DataColumn("Amount", typeof(int));
                        amt.DefaultValue = 1;
                        ProductList.Columns.Add(amt);
                        ProductList.Columns.Add("Calc", typeof(double));
                    }

                    DataRow NewProduct = tapw.SelectedProduct;
                    ProductList.ImportRow(NewProduct);
                    ProductDataGrid.ItemsSource = ProductList.DefaultView;
                    MessageBox.Show("123");
                    //SelectCellByIndex(ProductDataGrid, ProductList.Rows.Count - 1, 5);
                    DataGridRow row = ProductDataGrid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                    TextBox ele = ((ContentPresenter)ProductDataGrid.Columns[5].GetCellContent(row)).Content as TextBox;
                    ele.Focus();

                    Calculate_Calc();
                }
                else
                {
                    MessageWindow.ShowMessage("查無此商品", MessageType.WARNING);
                }
            }
        }

        public static void SelectCellByIndex(DataGrid dataGrid, int rowIndex, int columnIndex)
        {
            if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.Cell))
                throw new ArgumentException("The SelectionUnit of the DataGrid must be set to Cell.");

            if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
                throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

            if (columnIndex < 0 || columnIndex > (dataGrid.Columns.Count - 1))
                throw new ArgumentException(string.Format("{0} is an invalid column index.", columnIndex));

            dataGrid.SelectedCells.Clear();

            object item = dataGrid.Items[rowIndex]; //=Product X
            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row == null)
            {
                dataGrid.ScrollIntoView(item);
                row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }
            if (row != null)
            {
                DataGridCell cell = GetCell(dataGrid, row, columnIndex);
                if (cell != null)
                {
                    DataGridCellInfo dataGridCellInfo = new DataGridCellInfo(cell);
                    dataGrid.SelectedCells.Add(dataGridCellInfo);
                    cell.Focus();
                }
            }
        }
        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }
        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void Calculate_Calc()
        {
            if (ProductList != null)
            {
                foreach (DataRow dr in ProductList.Rows)
                {
                    dr["Calc"] = int.Parse(dr[AppliedPrice].ToString()) * int.Parse(dr["Amount"].ToString());
                }
                preTotal = ProductList.Compute("SUM(Calc)", string.Empty).ToString();
                lblPreTotal.Content = preTotal;
                if (ProductList.Rows.Count == 0)
                {
                    preTotal = "0";
                    lblPreTotal.Content = preTotal;
                }
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
            if (preTotal != "0")
                Calculate_Discount("AMT");
        }

        private void tbDiscountPer_LostFocus(object sender, RoutedEventArgs e)
        {
            if (preTotal != "0")
                Calculate_Discount("PER");
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow dgr = null;
            var visParent = VisualTreeHelper.GetParent(e.OriginalSource as FrameworkElement);
            while (dgr == null && visParent != null)
            {
                dgr = visParent as DataGridRow;
                visParent = VisualTreeHelper.GetParent(visParent);
            }
            if (dgr == null) { return; }

            var rowIdx = dgr.GetIndex();
            if (ProductList.Rows.Count > 0)
                ProductList.Rows.Remove(ProductList.Rows[rowIdx]);
            Calculate_Calc();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ConfirmWindow cw = new ConfirmWindow("是否清除頁面資料?", "清除頁面確認");
            if (!(bool)cw.DialogResult)
                return;
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
