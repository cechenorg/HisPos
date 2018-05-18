using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class StockTakingOTC : AbstractClass.Product, IStockTaking, IDeletable, INotifyPropertyChanged
    {
        public StockTakingOTC(DataRow dataRow) : base(dataRow)
        {
            Location = dataRow["PRO_LOCATION"].ToString();
            Category = dataRow["PROTYP_CHINAME"].ToString();
            LastCheckDate = dataRow["PROCHE_DATE"].ToString();
            BatchNumber = dataRow["STOORDDET_BATCHNUMBER"].ToString();
            Inventory = Double.Parse(dataRow["PRO_INVENTORY"].ToString());
            SafeAmount = Double.Parse(dataRow["PRO_SAFEQTY"].ToString());
            ValidDate = dataRow["STOORDDET_VALIDDATE"].ToString();
            Status = dataRow["PRO_STATUS"].ToString().Equals("1");
        }
     
        public string Category { get; set; }
        public double Inventory { get; set; }
        public double SafeAmount { get; set; }
        public string ValidDate { get; set; }
        public string LastCheckDate { get; set; }
        public string Location { get; set; }
        public string BatchNumber { get; set; }
        public bool Status { get; set; }
        private string source;
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }
    }
}
