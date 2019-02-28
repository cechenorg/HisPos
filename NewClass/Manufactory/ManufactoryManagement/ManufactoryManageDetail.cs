using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryManageDetail : Manufactory
    {
        #region ----- Define Variables -----
        public string Address { get; set; }
        public string Fax { get; set; }
        public string EIN { get; set; }
        public string Email { get; set; }
        public string ControlMedicineID { get; set; }
        public string Note { get; set; }
        public ManufactoryPrincipals Principals { get; set; }
        public ManufactoryTradeRecords TradeRecords { get; set; }
        #endregion

        public ManufactoryManageDetail(DataRow row) : base(row)
        {
            Address = row.Field<string>("Man_Address");
            Fax = row.Field<string>("Man_Fax");
            EIN = row.Field<string>("Man_EIN");
            Email = row.Field<string>("Man_Email");
            ControlMedicineID = row.Field<string>("Man_ControlMedicineID");
            Note = row.Field<string>("Man_Note");
        }

        #region ----- Define Functions -----
        public void GetManufactoryDetailData()
        {
            Principals = ManufactoryPrincipals.GetManufactoryPrincipals(ID);
            TradeRecords = ManufactoryTradeRecords.GetManufactoryTradeRecords(ID);
        }
        #endregion
    }
}
