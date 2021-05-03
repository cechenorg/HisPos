using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView.OrderDetailControl
{
    /// <summary>
    /// PurchaseDetailControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseDetailControl : UserControl
    {
        DataTable Send;
        public PurchaseDetailControl()
        {
            InitializeComponent();

        }



       
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            //StoreOrderHistorys ss = new StoreOrderHistorys();
            //if (HISTORYID == null) { }
            //else
            //{
            //    ss.getData(HISTORYID.Content.ToString());

            //}
            //MainWindow.ServerConnection.CloseConnection();

            //HISTORYDG.ItemsSource = ss;

        }

        //private void ContentControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    List<SqlParameter> parameters = new List<SqlParameter>();

        //    if (HistoryID != null)
        //    {
        //        parameters.Add(new SqlParameter("ID", HistoryID.Content.ToString()));
        //        MessageBox.Show(HistoryID.Content.ToString());
        //        DataTable historiesTable = MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderHistory]", parameters);
        //        Send = historiesTable;
        //        HISTORYDG.ItemsSource = historiesTable.DefaultView;
        //    }
        //}

        //private void HISTORYDG_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{

        //    MessageBox.Show(HISTORYDG.SelectedValue.ToString());


        //    int index = GetRowIndex(e);
        //    if (index < Send.Rows.Count && index >= 0)
        //    {
        //        string proID = Send.Rows[index]["ID"].ToString();
               

        //        ProductPurchaseRecordViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).ProductPurchaseRecord;

        //        Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, proID,""));

        //    }
        //}
        private int GetRowIndex(MouseButtonEventArgs e)
        {
            DataGridRow dgr = null;
            DependencyObject visParent = VisualTreeHelper.GetParent(e.OriginalSource as FrameworkElement);
            while (dgr == null && visParent != null)
            {
                dgr = visParent as DataGridRow;
                visParent = VisualTreeHelper.GetParent(visParent);
            }
            if (dgr == null) { return -1; }
            int rowIdx = dgr.GetIndex();
            return rowIdx;
        }
    }
   
}