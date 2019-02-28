using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Service;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryManageDetail : Manufactory, ICloneable
    {
        #region ----- Define Variables -----
        private ManufactoryPrincipal currentPrincipal;

        public string Address { get; set; }
        public string Fax { get; set; }
        public string EIN { get; set; }
        public string Email { get; set; }
        public string ControlMedicineID { get; set; }
        public string Note { get; set; }
        public ManufactoryPrincipals Principals { get; set; }
        public ManufactoryTradeRecords TradeRecords { get; set; }
        public ManufactoryPrincipal CurrentPrincipal { get; set; }
        #endregion

        private ManufactoryManageDetail() { }

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
        public bool DeleteManufactory()
        {
            DataTable dataTable = ManufactoryDB.DeleteManufactory(ID);

            if (dataTable.Rows.Count == 0)
                return false;
            else
                return dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
        }
        public void DeleteManufactoryPrincipal()
        {
            Principals.Remove(CurrentPrincipal);
        }
        public object Clone()
        {
            ManufactoryManageDetail newManufactoryManageDetail = new ManufactoryManageDetail();

            newManufactoryManageDetail.ID = ID;
            newManufactoryManageDetail.Name = Name;
            newManufactoryManageDetail.NickName = NickName;
            newManufactoryManageDetail.Telephone = Telephone;
            newManufactoryManageDetail.Address = Address;
            newManufactoryManageDetail.Fax = Fax;
            newManufactoryManageDetail.EIN = EIN;
            newManufactoryManageDetail.ControlMedicineID = ControlMedicineID;
            newManufactoryManageDetail.Note = Note;

            newManufactoryManageDetail.Principals = Principals.Clone() as ManufactoryPrincipals;

            return newManufactoryManageDetail;
        }
        internal void ResetData(ManufactoryManageDetail currentManufactoryBackUp)
        {
            ID = currentManufactoryBackUp.ID;
            Name = currentManufactoryBackUp.Name;
            NickName = currentManufactoryBackUp.NickName;
            Telephone = currentManufactoryBackUp.Telephone;
            Address = currentManufactoryBackUp.Address;
            Fax = currentManufactoryBackUp.Fax;
            EIN = currentManufactoryBackUp.EIN;
            ControlMedicineID = currentManufactoryBackUp.ControlMedicineID;
            Note = currentManufactoryBackUp.Note;

            Principals = currentManufactoryBackUp.Principals.Clone() as ManufactoryPrincipals;
        }
        internal bool UpdateManufactoryDetail()
        {
            DataTable dataTable = ManufactoryDB.UpdateManufactoryDetail(this);

            if (dataTable.Rows.Count == 0)
                return false;
            else
                return dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
        }

        internal void AddManufactoryPrincipal()
        {
            Principals.AddNewPrincipal();
            CurrentPrincipal = Principals.Last();
        }
        #endregion
    }
}
