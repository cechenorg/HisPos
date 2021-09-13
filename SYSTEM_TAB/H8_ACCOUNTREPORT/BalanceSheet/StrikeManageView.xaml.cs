using System.Data;
using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet
{
    /// <summary>
    /// BalanceSheetView.xaml 的互動邏輯
    /// </summary>
    public partial class StrikeManageView : UserControl
    {
        private DataTable debitAccList = new DataTable();
        private DataTable creditAccList = new DataTable();

        public StrikeManageView()
        {
            InitializeComponent();
            debitAccList.Columns.Add("AccID", typeof(string));
            debitAccList.Columns.Add("AccName", typeof(string));
            creditAccList.Columns.Add("AccID", typeof(string));
            creditAccList.Columns.Add("AccName", typeof(string));
        }

        private void InitView()
        {
            GetAccountList();
        }

        private void GetAccountList()
        {
        }
    }
}