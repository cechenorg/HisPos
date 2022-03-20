using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.ProductLocation;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    /// <summary>
    /// AddTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InsertLocationDetailWindow : Window
    {
        private int ID;

        public InsertLocationDetailWindow(int i)
        {
            InitializeComponent();
            ID = i;
            DataContext = this;
        }

        #region ----- Define Functions -----

        private bool CheckEmptyData()
        {
            string error = "";

            if (ProID.Text.Equals(""))
                error += "未填寫名稱!\n";

            if (error.Length != 0)
            {
                MessageWindow.ShowMessage(error, Class.MessageType.ERROR);

                return false;
            }

            return true;
        }

        #endregion ----- Define Functions -----

        #region ----- Define Events -----

        private void ConfrimClick(object sender, RoutedEventArgs e)
        {
            if (CheckEmptyData())
            {
                MainWindow.ServerConnection.OpenConnection();
                DataTable dataTable = ProductLocationDB.InsertProductLocationDetails(ID, ProID.Text);
                MainWindow.ServerConnection.CloseConnection();

                if (dataTable is null || dataTable.Rows.Count == 0)
                {
                    MessageWindow.ShowMessage("新增失敗 請稍後再試", Class.MessageType.ERROR);
                    return;
                }
                {
                    MessageWindow.ShowMessage("新增成功", Class.MessageType.SUCCESS);
                }

                Close();
            }
        }

        #endregion ----- Define Events -----

        private void ProID_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddProductByInputAction(ProID.Text);
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
                    ProID.Text = "";
                    return;
                }
            }
            else
            {
                if (searchString.Length < 2)
                {
                    MessageWindow.ShowMessage("搜尋字串長度不得小於2", MessageType.WARNING);
                    ProID.Text = "";
                    return;
                }
            }
            MainWindow.ServerConnection.OpenConnection();
            int productCount = ProductStructs.GetProductStructCountBySearchString(searchString, AddProductEnum.Trade);
            MainWindow.ServerConnection.CloseConnection();

            if (productCount == 0)
            {
                MessageWindow.ShowMessage("查無商品", MessageType.WARNING);
                ProID.Text = "";
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
                        ProID.Text = result.Rows[0]["Pro_ID"].ToString();
                    }
                    else if (result.Rows.Count > 1)
                    {
                        TradeAddProductWindow tapw = new TradeAddProductWindow(result);
                        tapw.ShowDialog();
                        DataRow NewProduct = tapw.SelectedProduct;
                        int amt = 0;
                        if (NewProduct != null)
                        {
                            ProID.Text = NewProduct["Pro_ID"].ToString();
                        }
                    }
                }
                else
                {
                    MessageWindow.ShowMessage("查無此商品", MessageType.WARNING);
                    ProID.Text = "";
                    return;
                }
            }
        }
    }
}