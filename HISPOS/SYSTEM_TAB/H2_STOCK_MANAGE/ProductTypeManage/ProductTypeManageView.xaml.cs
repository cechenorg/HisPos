using ClosedXML.Excel;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.ProductLocation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage
{
    /// <summary>
    /// ProductTypeManageView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTypeManageView : System.Windows.Controls.UserControl
    {
        private DataTable master;
        private DataTable detail;
        private int SelectedValue;

        public ProductTypeManageView()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            //AddLocationWindow addTypeWindow = new AddLocationWindow();
            //addTypeWindow.ShowDialog();
            InitLocationLoad();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            //EditLocationWindow editTypeWindow = new EditLocationWindow((int)ProductLocationDataGrid.SelectedValue);
            DataRowView row = (DataRowView)ProductLocationDataGrid.SelectedItem;
            //editTypeWindow.ChiName.Text = row["ProLoc_Name"].ToString();
            //editTypeWindow.ShowDialog();
            InitLocationLoad();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var deleteMsg = "確定刪除選取櫃位?";
            var delete = new ConfirmWindow(deleteMsg, "刪除確認");
            if ((bool)delete.DialogResult)
            {
                MainWindow.ServerConnection.OpenConnection();
                DataTable dataTable = ProductLocationDB.DeleteLocation((int)ProductLocationDataGrid.SelectedValue);
                MainWindow.ServerConnection.CloseConnection();

                if (dataTable is null || dataTable.Rows.Count == 0)
                {
                    MessageWindow.ShowMessage("刪除失敗 請稍後再試", NewClass.MessageType.ERROR);
                    return;
                }
            }

            InitLocationLoad();
        }

        private void InitLocation()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductLocationDB.GetProductLocationMasters();
            MainWindow.ServerConnection.CloseConnection();
            master = dataTable;
            ProductLocationDataGrid.ItemsSource = dataTable.DefaultView;
            if (SelectedValue > 0) { ProductLocationDataGrid.SelectedValue = SelectedValue; }
        }

        private void InitLocationLoad()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductLocationDB.GetProductLocationMasters();
            MainWindow.ServerConnection.CloseConnection();
            master = dataTable;
            ProductLocationDataGrid.ItemsSource = dataTable.DefaultView;
            ProductLocationDataGrid.SelectedIndex = 0;
        }

        private void InitLocationDetail()
        {
            if (ProductLocationDataGrid.SelectedValue == null)
            {
                return;
            }
            else
            {
                MainWindow.ServerConnection.OpenConnection();
                DataTable dataTable = ProductLocationDB.GetProductLocationDetails(SelectedValue);
                MainWindow.ServerConnection.CloseConnection();
                detail = dataTable;
                ProductLocationDetailDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void ProductLocationDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            InsertButton.Visibility = Visibility.Visible;
            PrintButton.Visibility = Visibility.Visible;
            lbInsertID.Visibility = Visibility.Visible;
            InsertID.Visibility = Visibility.Visible;
            DeleteDetailButton.Visibility = Visibility.Visible;
            if (ProductLocationDataGrid.SelectedValue == null)
            {
                Newbtn.Visibility = Visibility.Visible;
                Editbtn.Visibility = Visibility.Visible;
                Deletebtn.Visibility = Visibility.Visible;
            }
            else if ((int)ProductLocationDataGrid.SelectedValue == 9999)
            {
                Newbtn.IsEnabled = false;
                Editbtn.IsEnabled = false;
                Deletebtn.IsEnabled = false;
                InsertButton.Visibility = Visibility.Hidden;
                PrintButton.Visibility = Visibility.Visible;
                lbInsertID.Visibility = Visibility.Hidden;
                InsertID.Visibility = Visibility.Hidden;
                DeleteDetailButton.Visibility = Visibility.Hidden;
                SelectedValue = (int)ProductLocationDataGrid.SelectedValue;
            }
            else
            {
                Newbtn.IsEnabled = true;
                Editbtn.IsEnabled = true;
                Deletebtn.IsEnabled = true;
                SelectedValue = (int)ProductLocationDataGrid.SelectedValue;
            }
            InitLocationDetail();
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            //InsertLocationDetailWindow insertTypeWindow = new InsertLocationDetailWindow((int)ProductLocationDataGrid.SelectedValue);
            //insertTypeWindow.ShowDialog();
            InitLocationDetail();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (master.DefaultView is null) return;
            if (detail.DefaultView is null) return;

            DataRowView row = (DataRowView)ProductLocationDataGrid.SelectedItem;

            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "儲位管理";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;
            fdlg.Filter = "XLSX檔案|*.xlsx";
            fdlg.FileName = DateTime.Now.ToString("yyyy-MM-dd") + "_" + row["ProLoc_Name"].ToString() + "_" + "櫃位管理";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                var ws = wb.Worksheets.Add(row["ProLoc_Name"].ToString() + "儲位管理");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);

                var col1 = ws.Column("A");
                col1.Width = 20;
                var col2 = ws.Column("B");
                col2.Width = 33;
                var col3 = ws.Column("C");
                col3.Width = 10;
                var col4 = ws.Column("D");
                col4.Width = 10;
                var col5 = ws.Column("E");
                col5.Width = 10;

                ws.Cell(1, 1).Value = "櫃位名稱：" + row["ProLoc_Name"].ToString();
                ws.Range(1, 1, 1, 5).Merge().AddToNamed("Titles");
                ws.Cell("A2").Value = "商品代碼";
                ws.Cell("B2").Value = "商品名稱";
                ws.Cell("C2").Value = "庫存數量";
                ws.Cell("D2").Value = "寄庫數量";
                ws.Cell("E2").Value = "盤點數量";
                var rangeWithData = ws.Cell(3, 1).InsertData(detail.AsEnumerable());

                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                wb.SaveAs(fdlg.FileName);
            }
            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = (fdlg.FileName);
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.Verb = "print";
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
            }
        }

        private void InsertID_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddProductByInputAction(InsertID.Text);
            }
        }

        private void AddProductByInputAction(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return;
            if (searchString.Length == 0)
            {
                return;
            }

            if (int.TryParse(searchString, out int n))
            {
                if (searchString.Length < 5)
                {
                    MessageWindow.ShowMessage("商品代碼長度不得小於5", MessageType.WARNING);
                    InsertID.Text = "";
                    return;
                }
            }
            else
            {
                if (searchString.Length < 2)
                {
                    MessageWindow.ShowMessage("搜尋字串長度不得小於2", MessageType.WARNING);
                    InsertID.Text = "";
                    return;
                }
            }
            MainWindow.ServerConnection.OpenConnection();
            int productCount = ProductStructs.GetProductStructCountBySearchString(searchString, AddProductEnum.Trade);
            MainWindow.ServerConnection.CloseConnection();

            if (productCount == 0)
            {
                MessageWindow.ShowMessage("查無商品", MessageType.WARNING);
                InsertID.Text = "";
                return;
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

                    if (result.Rows.Count == 1)
                    {
                        InsertID.Text = result.Rows[0]["Pro_ID"].ToString();
                    }
                    else if (result.Rows.Count > 1)
                    {
                        TradeAddProductWindow tapw = new TradeAddProductWindow(result);
                        tapw.ShowDialog();
                        DataRow NewProduct = tapw.SelectedProduct;
                        int amt = 0;
                        if (NewProduct != null)
                        {
                            InsertID.Text = NewProduct["Pro_ID"].ToString();
                        }
                    }
                }
                else
                {
                    MessageWindow.ShowMessage("查無此商品", MessageType.WARNING);
                    InsertID.Text = "";
                    return;
                }
            }
        }

        private void InsertButton_Click_1(object sender, RoutedEventArgs e)
        {
            AddProductByInputAction(InsertID.Text);
            if (CheckEmptyData())
            {
                MainWindow.ServerConnection.OpenConnection();
                DataTable dataTable = ProductLocationDB.InsertProductLocationDetails((int)ProductLocationDataGrid.SelectedValue, InsertID.Text);
                MainWindow.ServerConnection.CloseConnection();

                if (dataTable is null || dataTable.Rows.Count == 0)
                {
                    MessageWindow.ShowMessage("查無商品 請重新輸入", NewClass.MessageType.ERROR);
                    InsertID.Text = "";
                    return;
                }
                {
                    if (dataTable.Rows[0]["RESULT"].ToString() == "DOUBLE")
                    {
                        MessageWindow.ShowMessage("商品已在櫃位內 請先執行刪除", NewClass.MessageType.ERROR);
                        return;
                    }
                    MessageWindow.ShowMessage("新增成功", NewClass.MessageType.SUCCESS);
                    InsertID.Text = "";
                    InitLocationDetail();
                    InitLocation();
                }
            }
        }

        private bool CheckEmptyData()
        {
            string error = "";

            if (InsertID.Text.Equals(""))
                error += "未填寫名稱!\n";

            if (error.Length != 0)
            {
                MessageWindow.ShowMessage(error, NewClass.MessageType.ERROR);

                return false;
            }

            return true;
        }

        private void DeleteDetailButton_Click(object sender, RoutedEventArgs e)
        {
            string Pro_id;
            Pro_id = ProductLocationDetailDataGrid.SelectedValue.ToString();
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Pro_ID", Pro_id));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[ProductLocationDetailsDelete]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            if (result is null || result.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("刪除時發生錯誤 請再試一次", NewClass.MessageType.ERROR);
                return;
            }
            {
                MessageWindow.ShowMessage("刪除成功", NewClass.MessageType.SUCCESS);
                InitLocationDetail();
                InitLocation();
            }
        }
    }
}