using His_Pos.FunctionWindow;
using His_Pos.NewClass.ProductType;
using System.Data;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage
{
    /// <summary>
    /// AddTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddTypeWindow : Window
    {
        #region ----- Define Variables -----

        public ProductTypeManageMasters TypeManageCollection { get; set; }
        public ProductTypeManageMaster SelectedType { get; set; }

        #endregion ----- Define Variables -----

        public AddTypeWindow(ProductTypeManageMasters typeManageCollection)
        {
            TypeManageCollection = typeManageCollection;
            InitializeComponent();
            UpdateUi();

            DataContext = this;
        }

        #region ----- Define Functions -----

        private void UpdateUi()
        {
            if (SelectionHint is null) return;

            if ((bool)SmallTypeRadioButton.IsChecked)
            {
                SelectionHint.Content = "選擇所屬大類別";
                BigTypeCombo.Visibility = Visibility.Visible;
            }
            else
            {
                SelectionHint.Content = "輸入大類別資訊";
                BigTypeCombo.Visibility = Visibility.Collapsed;
            }
        }

        private bool CheckEmptyData()
        {
            string error = "";

            if ((bool)SmallTypeRadioButton.IsChecked && SelectedType is null)
                error += "未選擇所屬大類別!\n";
            else if ((bool)SmallTypeRadioButton.IsChecked && SelectedType.ID == 0)
                error += "藥品無法新增小類別!\n";

            if (ChiName.Text.Equals(""))
                error += "未填寫中文名稱!\n";

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
                DataTable dataTable = ProductTypeDB.AddNewProductType(ChiName.Text, (SelectedType is null) ? -1 : SelectedType.ID);
                MainWindow.ServerConnection.CloseConnection();

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