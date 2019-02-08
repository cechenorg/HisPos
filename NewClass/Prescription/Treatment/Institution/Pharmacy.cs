﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.MedicalPerson;

namespace His_Pos.NewClass.Prescription.Treatment.Institution {
    public class Pharmacy : ObservableObject
    {
        public Pharmacy() {
        }

        public Pharmacy(DataRow r) {
            Id = r.Field<string>("CurPha_ID");
            Name = r.Field<string>("CurPha_Name");
            Address = r.Field<string>("CurPha_Address");
            Tel = r.Field<string>("CurPha_Telephone");
            ReaderCom = r.Field<byte>("CurPha_ReaderCom");
            VpnIp = r.Field<string>("CurPha_VPN");
            NewReader = r.Field<bool>("CurPha_ReaderIsNew");
        }
        private string id;
        public string Id {
            get { return id; }
            set { Set(() => Id,ref id,value); }
        }
        public string Name { get; set; } 
        public string Address { get; set; }
        public string Tel { get; set; }
        public int ReaderCom { get; set; }
        public string VpnIp { get; set; }
        public bool NewReader { get; set; }
        public MedicalPersonnel MedicalPersonnel { get; set; }
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
                return MedicalPersonnels.Single(m => m.Id.Equals(ViewModelMainWindow.CurrentUser.ID));
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
