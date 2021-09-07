using His_Pos.FunctionWindow;
using His_Pos.NewClass.ProductLocation;
using System.Data;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    /// <summary>
    /// EditTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class EditLocationWindow : Window
    {
        private int ID;

        public EditLocationWindow(int i)
        {
            InitializeComponent();

            ID = i;

            DataContext = this;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductLocationDB.UpdateLocation(ID, ChiName.Text);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable.Rows[0]["RESULT"].ToString() == "DOUABLE")
            {
                MessageWindow.ShowMessage("櫃位名稱不得重複", Class.MessageType.ERROR);
                return;
            }

            if (dataTable is null || dataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("新增失敗 請稍後再試", Class.MessageType.ERROR);
                return;
            }
            MessageWindow.ShowMessage("修改成功", Class.MessageType.ERROR);
            Close();
        }
    }
}