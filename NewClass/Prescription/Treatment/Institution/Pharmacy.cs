﻿using System.Data;
using System.Linq;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.MedicalPerson;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.Institution {
    [ZeroFormattable]
    public class Pharmacy : ObservableObject
    {
        public Pharmacy() {
        }

        public Pharmacy(DataRow r) {
            ID = r.Field<string>("CurPha_ID");
            Name = r.Field<string>("CurPha_Name");
            Address = r.Field<string>("CurPha_Address");
            Tel = r.Field<string>("CurPha_Telephone");
            ReaderCom = r.Field<byte>("CurPha_ReaderCom");
            VpnIp = r.Field<string>("CurPha_VPN");
            NewReader = r.Field<bool>("CurPha_ReaderIsNew");
        }
        private string id;
        [Index(0)]
        public virtual string ID {
            get { return id; }
            set { Set(() => ID,ref id,value); }
        }
        [Index(1)]
        public virtual string Name { get; set; }
        [Index(2)]
        public virtual string Address { get; set; }
        [Index(3)]
        public virtual string Tel { get; set; }
        [Index(4)]
        public virtual int ReaderCom { get; set; }
        [Index(5)]
        public virtual string VpnIp { get; set; }
        [Index(6)]
        public virtual bool NewReader { get; set; }
        [IgnoreFormat]
        public MedicalPersonnel MedicalPersonnel { get; set; }
        [IgnoreFormat]
        public MedicalPersonnels MedicalPersonnels { get; set; }

        #region Function
        public static Pharmacy GetCurrentPharmacy() { 
            DataTable tableCurrentPharmacy = PharmacyDb.GetCurrentPharmacy();
            Pharmacy pharmacy = new Pharmacy(tableCurrentPharmacy.Rows[0]);
            pharmacy.MedicalPersonnels = new MedicalPersonnels(); 
            return pharmacy;
        }
        public MedicalPersonnel GetPharmacist()
        {
            if (ViewModelMainWindow.CurrentUser.WorkPositionName.Equals("藥師"))
                return MedicalPersonnels.Single(m => m.ID.Equals(ViewModelMainWindow.CurrentUser.ID));
            return MedicalPersonnels[0];
        }
        public void SetPharmacy() {
            PharmacyDb.SetPharmacy(this);
        }
        public void InsertPharmacy() {
            PharmacyDb.InsertPharmacy(this);
        }
        #endregion
    }
}
