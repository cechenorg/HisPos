using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Service;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

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
            if (StartDate.Text.Contains("-") || EndDate.Text.Contains("-")) { return; }
            string sDate = ConvertMaskedDate(StartDate.Text);
            string eDate = ConvertMaskedDate(EndDate.Text);
            
            System.Windows.MessageBox.Show(sDate);

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MasterID", DBNull.Value));
            parameters.Add(new SqlParameter("sDate", sDate));
            parameters.Add(new SqlParameter("eDate", eDate));
            parameters.Add(new SqlParameter("flag", "0"));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            FormatData(result);
            RecordGrid.ItemsSource = result.DefaultView;
        }

        private void FormatData(DataTable result) 
        {
            result.Columns.Add("TransTime_Format", typeof(string));
            foreach (DataRow dr in result.Rows) 
            {                
                string ogTransTime = dr["TraMas_ChkoutTime"].ToString();
                DateTime dTime = DateTime.Parse(ogTransTime);
                string formatTransTime = dTime.ToString("yyyy-MM-dd HH:mm");
                dr["TransTime_Format"] = formatTransTime;
            }
        }

        private string ConvertMaskedDate(string dateString) 
        {
            string[] strArr = dateString.Split('/');
            string year = (int.Parse(strArr[0]) + 1911).ToString();
            string month = string.Format("{0:D2}", strArr[1]);
            string date = string.Format("{0:D2}", strArr[2]);
            return year + "-" + month + "-" + date;
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
