using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.ProductLocation;
using His_Pos.NewClass.ProductType;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    /// <summary>
    /// EditTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class EditLocationWindow : Window
    {

        int ID;
        public EditLocationWindow(int i)
        {
            InitializeComponent();

            ID = i;

            DataContext = this;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductLocationDB.UpdateLocation(ID,ChiName.Text);
            MainWindow.ServerConnection.CloseConnection();

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
