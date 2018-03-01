using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
using His_Pos.Class;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// InventoryManagementView.xaml 的互動邏輯
    /// </summary>
    public partial class InventoryManagementView : UserControl
    {
        private readonly ObservableCollection<Inventory> _inventoryList = new ObservableCollection<Inventory>();
        public InventoryManagementView()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            DataBinding(start.Text,end.Text, Name.Text,ID.Text);
        }

        private void DataBinding(string start,string end,string name,string id) {
            var conn = new DbConnection(Settings.Default.SQL_global);
            DateTimeExtensions dateTimeExtensions = new DateTimeExtensions();
            var parameters = new List<SqlParameter>();
            if (start != string.Empty)
                    parameters.Add(new SqlParameter("SDATE", dateTimeExtensions.UsToTaiwan(start)));
            else
                parameters.Add(new SqlParameter("SDATE", DBNull.Value));
            if (end != string.Empty)
                parameters.Add(new SqlParameter("EDATE", dateTimeExtensions.UsToTaiwan(end)));
            else
                parameters.Add(new SqlParameter("EDATE", DBNull.Value));
            if (name != string.Empty)
                parameters.Add(new SqlParameter("NAME", name));
            else
                parameters.Add(new SqlParameter("NAME", DBNull.Value));
            if (id != string.Empty)
                parameters.Add(new SqlParameter("ID", id));
            else
                parameters.Add(new SqlParameter("ID", DBNull.Value));

            var table = conn.ExecuteProc("[HIS_POS_DB].[GET].[INVENTORYDATA]", parameters);
            foreach (DataRow row in table.Rows)
            {
                var inventory = new Inventory();
                SetValue(ref inventory, row);
                _inventoryList.Add(inventory);
            }
            DataGrid.ItemsSource = _inventoryList;
        }
        private void SetValue(ref Inventory inventory,DataRow row)
        {
            inventory.ProductId = row["PRO_ID"].ToString();
            inventory.ProductName = row["PRO_NAME"].ToString();
            inventory.ProductSafeAmount = row["PRO_SAFEQTY"].ToString();
            inventory.ProductTotalAmount = row["PRO_INVENTORY"].ToString();
            inventory.ProductCurrentTime = row["PURCHASE_DATE"].ToString();
        }

    }
}
