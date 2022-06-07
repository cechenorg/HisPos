using His_Pos.FunctionWindow;
using His_Pos.NewClass.ProductLocation;
using System.Data;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    /// <summary>
    /// DeleteTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DeleteLocationWindow : Window
    {
        #region ----- Define Variables -----

        private int ID;

        #endregion ----- Define Variables -----

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