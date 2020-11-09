using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Report.CashDetailReport.CashDetailRecordReport;
using His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot;
using His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingDetailRecordReport;
using His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport.EntryDetailControl
{
    /// <summary>
    /// PrescriptionDetailControl.xaml 的互動邏輯
    /// </summary>
    public partial class TradeProfitDetailControl : UserControl
    {
        public TradeProfitDetailControl()
        {
            InitializeComponent();
            
        }

        private void ShowProductDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row?.Item is null) return;

            ProductDetailWindow.ShowProductDetailWindow();

           // Messenger.Default.Send(new NotificationMessage<string>(this, ((TradeProfitDetailRecordReport)row.Item).MasterID, "ShowProductDetail"));
        }

        private void Recordbtn_Click(object sender, RoutedEventArgs e)
        {
            Recordbtn.Visibility = Visibility.Collapsed;
            Empbtn.Visibility = Visibility.Visible;
            dgEmp.Visibility = Visibility.Collapsed;
            dgEmpRecord.Visibility = Visibility.Collapsed;
            lblEmp.Visibility = Visibility.Collapsed;
            dgRecord.Visibility = Visibility.Visible;
            dgRecordRecord.Visibility = Visibility.Visible;
            lblRecord.Visibility = Visibility.Visible;
            spRecord.Visibility = Visibility.Visible;
            spEmp.Visibility = Visibility.Collapsed;
            SumEmp.Visibility = Visibility.Collapsed;
            SumRecord.Visibility = Visibility.Visible;
        }

        private void Empbtn_Click(object sender, RoutedEventArgs e)
        {
            Recordbtn.Visibility = Visibility.Visible;
            Empbtn.Visibility = Visibility.Collapsed;
            dgEmp.Visibility = Visibility.Visible;
            dgEmpRecord.Visibility = Visibility.Visible;
            lblEmp.Visibility = Visibility.Visible;
            dgRecord.Visibility = Visibility.Collapsed;
            dgRecordRecord.Visibility = Visibility.Collapsed;
            lblRecord.Visibility = Visibility.Collapsed;
            spRecord.Visibility = Visibility.Collapsed;
            spEmp.Visibility = Visibility.Visible;
            SumEmp.Visibility = Visibility.Visible;
            SumRecord.Visibility = Visibility.Collapsed;


        }
    }
}
