using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.ProductType;
using System.Data;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductTypeManage
{
    /// <summary>
    /// DeleteTypeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DeleteTypeWindow : Window
    {
        #region ----- Define Variables -----

        public ProductTypeManageMaster SelectedType { get; set; }

        #endregion ----- Define Variables -----

        public DeleteTypeWindow(ProductTypeManageMaster currentType)
        {
            SelectedType = currentType;
            InitializeComponent();
            DataContext = this;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            if ((bool)SmallCategoryRadioButton.IsChecked && SelectedType.CurrentDetailType.ProductCollection.Count > 0)
            {
                MessageWindow.ShowMessage("小類別中還有商品 無法刪除", MessageType.ERROR);
                return;
            }
            else if (!(bool)SmallCategoryRadioButton.IsChecked && SelectedType.TypeDetailCount > 0)
            {
                MessageWindow.ShowMessage("大類別中還有小類別 無法刪除", MessageType.ERROR);
                return;
            }

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductTypeDB.DeleteType(((bool)SmallCategoryRadioButton.IsChecked) ? SelectedType.CurrentDetailType.ID : SelectedType.ID);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable is null || dataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("新增失敗 請稍後再試", NewClass.MessageType.ERROR);
                return;
            }

            Close();
        }
    }
}