using His_Pos.FunctionWindow;
using His_Pos.NewClass.ProductLocation;
using System.Data;
using System.Windows;

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

        #endregion ----- Define Functions -----

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

                if (dataTable.Rows[0]["RESULT"].ToString()=="DOUABLE")
                {
                    MessageWindow.ShowMessage("櫃位名稱不得重複", Class.MessageType.ERROR);
                    return;
                }


                if (dataTable is null || dataTable.Rows.Count == 0)
                {
                    MessageWindow.ShowMessage("新增失敗 請稍後再試", Class.MessageType.ERROR);
                    return;
                }

                Close();
            }
        }

        #endregion ----- Define Events -----
    }
}