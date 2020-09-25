using System.Data;
using System.Windows;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.ProductLocation;
using His_Pos.NewClass.ProductType;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    /// <summary>
    /// DeleteTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DeleteLocationWindow : Window
    {
        #region ----- Define Variables -----
        int ID;
        #endregion

        public DeleteLocationWindow(int i)
        {
            //SelectedType = currentType;
            InitializeComponent();
            ID = i;
            DataContext = this;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductLocationDB.DeleteLocation(ID);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable is null || dataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("刪除失敗 請稍後再試", Class.MessageType.ERROR);
                return;
            }

            Close();
        }
    }
}
