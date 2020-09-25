using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.ProductLocation;
using His_Pos.NewClass.ProductType;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    /// <summary>
    /// AddTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InsertLocationDetailWindow : Window
    {
        int ID;

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
        #endregion

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

                Close();
            }
        }
        #endregion
    }
}
