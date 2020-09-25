using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.ProductLocation;
using His_Pos.NewClass.ProductType;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    /// <summary>
    /// AddTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddLocationWindow : Window
    {

        public AddLocationWindow()
        {
           
            InitializeComponent();
            UpdateUi();

            DataContext = this;
        }

        #region ----- Define Functions -----
        private void UpdateUi()
        {
        }
        private bool CheckEmptyData()
        {
            string error = "";

            if (ChiName.Text.Equals(""))
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
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateUi();
        }
        private void ConfrimClick(object sender, RoutedEventArgs e)
        {
            if (CheckEmptyData())
            {
                MainWindow.ServerConnection.OpenConnection();
                DataTable dataTable = ProductLocationDB.AddNewProductLocation(ChiName.Text);
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
