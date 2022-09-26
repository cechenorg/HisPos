
using System;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryManageDetail : Manufactory, ICloneable
    {
        #region ----- Define Variables -----

        private ManufactoryPrincipal currentPrincipal;
        private string responsibleName;
        private string responsibleTelephone;
        private string responsibleLINE;
        private string address;
        private string fax;
        private string ein;
        private string email;
        private string medicalID;
        private string controlMedicineID;
        private string note;

        private bool isDataChanged;

        public bool IsDataChanged
        {
            get { return isDataChanged; }
            set
            {
                Set(() => IsDataChanged, ref isDataChanged, value);
            }
        }

        public string ResponsibleName
        {
            get { return responsibleName; }
            set { Set(() => ResponsibleName, ref responsibleName, value); }
        }

        public string ResponsibleTelephone
        {
            get { return responsibleTelephone; }
            set { Set(() => ResponsibleTelephone, ref responsibleTelephone, value); }
        }

        public string ResponsibleLINE
        {
            get { return responsibleLINE; }
            set { Set(() => ResponsibleLINE, ref responsibleLINE, value); }
        }

        public string Address
        {
            get { return address; }
            set { Set(() => Address, ref address, value); }
        }

        public string Fax
        {
            get { return fax; }
            set { Set(() => Fax, ref fax, value); }
        }

        public string EIN
        {
            get { return ein; }
            set { Set(() => EIN, ref ein, value); }
        }

        public string Email
        {
            get { return email; }
            set { Set(() => Email, ref email, value); }
        }

        public string MedicalID
        {
            get { return medicalID; }
            set { Set(() => MedicalID, ref medicalID, value); }
        }

        public string ControlMedicineID
        {
            get { return controlMedicineID; }
            set { Set(() => ControlMedicineID, ref controlMedicineID, value); }
        }

        public string Note
        {
            get { return note; }
            set { Set(() => Note, ref note, value); }
        }

        public ManufactoryPrincipals Principals { get; set; }
        public ManufactoryTradeRecords TradeRecords { get; set; }

        public ManufactoryPrincipal CurrentPrincipal
        {
            get { return currentPrincipal; }
            set
            {
                bool oldIsDataChanged = IsDataChanged;

                Set(() => CurrentPrincipal, ref currentPrincipal, value);
                RaisePropertyChanged(nameof(HasPrincipal));

                if (!oldIsDataChanged)
                    IsDataChanged = false;
            }
        }

        public bool HasPrincipal => !(CurrentPrincipal is null);

        #endregion ----- Define Variables -----

        private ManufactoryManageDetail()
        {
        }

        public ManufactoryManageDetail(DataRow row) : base(row)
        {
            ResponsibleName = row.Field<string>("Man_ResponsibleName");
            ResponsibleTelephone = row.Field<string>("Man_ResponsibleTelephone");
            ResponsibleLINE = row.Field<string>("Man_ResponsibleLINE");

            Address = row.Field<string>("Man_Address");
            Fax = row.Field<string>("Man_Fax");
            EIN = row.Field<string>("Man_EIN");
            Email = row.Field<string>("Man_Email");
            MedicalID = row.Field<string>("Man_MedicalID");
            ControlMedicineID = row.Field<string>("Man_ControlMedicineID");
            Note = row.Field<string>("Man_Note");
        }

        #region ----- Define Functions -----
         
        public void ResetData(ManufactoryManageDetail currentManufactoryBackUp)
        {
            ID = currentManufactoryBackUp.ID;
            Name = currentManufactoryBackUp.Name;
            NickName = currentManufactoryBackUp.NickName;
            Telephone = currentManufactoryBackUp.Telephone;
            ResponsibleName = currentManufactoryBackUp.ResponsibleName;
            ResponsibleTelephone = currentManufactoryBackUp.ResponsibleTelephone;
            ResponsibleLINE = currentManufactoryBackUp.ResponsibleLINE;
            Address = currentManufactoryBackUp.Address;
            Fax = currentManufactoryBackUp.Fax;
            Email = currentManufactoryBackUp.Email;
            EIN = currentManufactoryBackUp.EIN;
            MedicalID = currentManufactoryBackUp.MedicalID;
            ControlMedicineID = currentManufactoryBackUp.ControlMedicineID;
            Note = currentManufactoryBackUp.Note;

            if (Principals != null) 
            {
                Principals.ResetData(currentManufactoryBackUp.Principals);
                
                if (Principals.Count > 0)
                    CurrentPrincipal = Principals.First();
                else
                    CurrentPrincipal = null;
            }
        }       

        #region ///// Principal Functions /////

        public void AddManufactoryPrincipal()
        {
            Principals.AddNewPrincipal();
            CurrentPrincipal = Principals.Last();
        }

        public void DeleteManufactoryPrincipal()
        {
            Principals.Remove(CurrentPrincipal);

            if (Principals.Count > 0)
                CurrentPrincipal = Principals.Last();
            else
                CurrentPrincipal = null;
        }

        #endregion ///// Principal Functions /////

        public object Clone()
        {
            ManufactoryManageDetail newManufactoryManageDetail = new ManufactoryManageDetail();

            newManufactoryManageDetail.ID = ID;
            newManufactoryManageDetail.Name = Name;
            newManufactoryManageDetail.NickName = NickName;
            newManufactoryManageDetail.Telephone = Telephone;
            newManufactoryManageDetail.ResponsibleName = ResponsibleName;
            newManufactoryManageDetail.ResponsibleTelephone = ResponsibleTelephone;
            newManufactoryManageDetail.ResponsibleLINE = ResponsibleLINE;
            newManufactoryManageDetail.Address = Address;
            newManufactoryManageDetail.Fax = Fax;
            newManufactoryManageDetail.Email = Email;
            newManufactoryManageDetail.EIN = EIN;
            newManufactoryManageDetail.MedicalID = MedicalID;
            newManufactoryManageDetail.ControlMedicineID = ControlMedicineID;
            newManufactoryManageDetail.Note = Note;

            if(Principals != null)
                newManufactoryManageDetail.Principals = Principals.Clone() as ManufactoryPrincipals;

            return newManufactoryManageDetail;
        }

        #endregion ----- Define Functions -----
    }
}