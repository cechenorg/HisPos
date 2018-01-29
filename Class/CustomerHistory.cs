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
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class
{
    public class CustomerHistory
    {
        public CustomerHistory(int type, string date, string historyId, string hisTitle)
        {
            Type = type;
            Date = date;
            HistoryId = historyId;
            HistoryTitle = hisTitle;
            GetCollection();
        }

        private void GetCollection()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            DataTable table;
            if (Type == 0)
            {
                TypeIcon = new BitmapImage(new Uri(@"..\Images\PosDot.png", UriKind.Relative));
                table = GetPosHistories(dd, parameters);
            }
            else
            {
                TypeIcon = new BitmapImage(new Uri(@"..\Images\HisDot.png", UriKind.Relative));
                table = GetHisHistories(dd, parameters);
            }
            GetHistoryItems(table);
        }

        private void GetHistoryItems(DataTable table)
        {
            foreach (DataRow d in table.Rows)
            {
                if (Type == 0)
                {
                    var product = GetProduct(d);
                    CustomHistories.Add(product);
                }
                else
                {
                    var medicine = GetMedicine(d);
                    CustomHistories.Add(medicine);
                }
            }
        }

        private DataTable GetPosHistories(DbConnection dd,List<SqlParameter> parameters)
        {
            var sqlParameter = new SqlParameter("CUSORD_ID", int.Parse(HistoryId));
            parameters.Add(sqlParameter);
            return dd.ExecuteProc("[HIS_POS_DB].[GET].[CUSPOSHISTORY]", parameters);
        }

        private DataTable GetHisHistories(DbConnection dd, List<SqlParameter> parameters)
        {
            var sqlParameter = new SqlParameter("HISDECMAS_ID", HistoryId);
            parameters.Add(sqlParameter);
            return dd.ExecuteProc("[HIS_POS_DB].[GET].[CUSHISHISTORY]", parameters);
        }

        private Otc GetProduct(DataRow d)
        {
            var otc = new Otc
            {
                Name = d["PRO_NAME"].ToString(),
                Price = double.Parse(d["CUSORDDET_PRICE"].ToString()),
                Total = (short) d["CUSORDDET_QTY"],
                TotalPrice = double.Parse(d["CUSORDDET_SUBTOTAL"].ToString())
            };
            return otc;
        }

        private Medicine GetMedicine(DataRow d)
        {
            var medicine = new Medicine
            {
                Name = d["MED_NAME"].ToString(),
                MedicalCategory =
                {
                    Dosage = d["HISFEQ_ID"].ToString(),
                    Usage = d["HISWAY_ID"].ToString(),
                    Days = (short) d["HISDECDET_DRUGDAY"]
                }
            };
            return medicine;
        }

        public BitmapImage TypeIcon { get; set; }
        public int Type { get; }
        public string Date { get; }
        public string HistoryId { get; }
        public string HistoryTitle { get; set; }

        public ObservableCollection<object> CustomHistories = new ObservableCollection<object>();
    }
}
