using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Service;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransactionRecord
{
    /// <summary>
    /// ProductTransactionRecordView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTransactionRecordView : UserControl
    {
        public ProductTransactionRecordView()
        {
            InitializeComponent();
        }

        private void GetData() 
        {
            //DateTime? sDate = (DateTime?)StartDate.Value;
            //DateTime? eDate = (DateTime?)EndDate.Value;
            string sDate = "2020-08-10";
            string eDate = "2020-08-10";

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MasterID", DBNull.Value));
            parameters.Add(new SqlParameter("sDate", sDate));
            parameters.Add(new SqlParameter("eDate", eDate));
            parameters.Add(new SqlParameter("flag", 0));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            RecordGrid.ItemsSource = result.DefaultView;
        }

        private void ShowSelectedPrescriptionEditWindow(object sender, MouseButtonEventArgs e)
        {

        }

        private void StartDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
                EndDate.Focus();
                EndDate.SelectionStart = 0;
            }
        }

        private void EndDate_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
            {
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
            }
        }

        private void btnQuery_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GetData();
        }
    }
}
