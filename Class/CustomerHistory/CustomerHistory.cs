using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using His_Pos.AbstractClass;
using His_Pos.Class.Product;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.CustomerHistory
{
    public class CustomerHistory
    {
        public ObservableCollection<CustomerHistoryMaster> CustomerHistoryMasterCollection { get; }
        private DataTable CustomerHistoryDetails;

        public CustomerHistory(DataTable customerHistoryMasters, DataTable customerHistoryDetails)
        {
            CustomerHistoryMasterCollection = new ObservableCollection<CustomerHistoryMaster>();

            foreach (DataRow row in customerHistoryMasters.Rows)
            {
                CustomerHistoryMasterCollection.Add(new CustomerHistoryMaster((SystemType)row["TYPE"], row["DATE"].ToString(), row["HISTORY_ID"].ToString(), row["HISTORY_DATA"].ToString()));
            }

            CustomerHistoryDetails = customerHistoryDetails;
        }

        public ObservableCollection<CustomerHistoryDetail> getCustomerHistoryDetails(SystemType type, string CustomerHistoryDetailId)
        {
            var table = CustomerHistoryDetails.Select("SYSTEMTYPE = '" + (int)type + "' AND CUSHISTORYDETAILID = '" + CustomerHistoryDetailId + "'");
            var customerHistoryDetailCollection = new ObservableCollection<CustomerHistoryDetail>();

            foreach (var row in table)
            {
                customerHistoryDetailCollection.Add(new CustomerHistoryDetail(row));
            }

            return customerHistoryDetailCollection;
        }
    }
}