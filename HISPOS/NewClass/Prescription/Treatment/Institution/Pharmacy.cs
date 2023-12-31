﻿using Dapper;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Pharmacy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    [ZeroFormattable]
    public class Pharmacy : ObservableObject, ICloneable
    {
        public Pharmacy()
        {
            MedicalPersonnels = new Employees();
        }

        public Pharmacy(DataRow r)
        {
            ID = r.Field<string>("CurPha_ID");
            Name = r.Field<string>("CurPha_Name");
            Address = r.Field<string>("CurPha_Address");
            Tel = r.Field<string>("CurPha_Telephone");
            ReaderCom = (Properties.Settings.Default.ReaderComPort == "") ? 0 : int.Parse(Properties.Settings.Default.ReaderComPort);
            VpnIp = r.Field<string>("CurPha_VPN");
            NewInstitution = r.Field<bool>("CurPha_NewInstitution");
            GroupServerName = r.Field<string>("GroupServerName");
            TAXNUM = r.Field<string>("PHAMAS_TAXNUM");
            VerifyKey = r.Field<string>("PHAMAS_VerifyKey");
            MedicalPersonnels = new Employees();
            AllEmployees = new Employees();

            ClosingDate = r["CurPha_ClosingDate"] != DBNull.Value ? Convert.ToDateTime(r["CurPha_ClosingDate"]) : new DateTime();
            PrescriptionCloseDate = r["CurPha_PrescriptionClosingDate"] != DBNull.Value ? Convert.ToDateTime(r["CurPha_PrescriptionClosingDate"]) : new DateTime();
        }

        private string id;

        [Index(0)]
        public virtual string ID
        {
            get { return id; }
            set { Set(() => ID, ref id, value); }
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

        private bool newInstitution;

        [Index(6)]
        public virtual bool NewInstitution
        {
            get => newInstitution;
            set
            {
                Set(() => NewInstitution, ref newInstitution, value);
            }
        }

        private DateTime? startDate;

        [Index(7)]
        public virtual DateTime? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private string tAXNUM;
        [Index(8)]
        public virtual string TAXNUM
        {
            get => tAXNUM;
            set
            {
                Set(() => TAXNUM, ref tAXNUM, value);
            }
        }
        [IgnoreFormat]
        public Employee MedicalPersonnel { get; set; }

        [IgnoreFormat]
        public Employees MedicalPersonnels { get; set; }

        [IgnoreFormat]
        public Employees AllEmployees { get; set; }

        [IgnoreFormat]
        public string GroupServerName { get; set; }

        [IgnoreFormat]
        public string HISPOS_ServerName { get; set; } = "HIS_POS_Server";


        [IgnoreFormat]
        public IEnumerable<NewClass.Pharmacy.PharmacyInfo> GroupPharmacyinfoList { get; set; }

        [IgnoreFormat]
        public string VerifyKey { get; set; }

        [IgnoreFormat]
        public DateTime ClosingDate { get; set; }
        [IgnoreFormat]
        public DateTime PrescriptionCloseDate { get; set; }
        #region Function

        public static Pharmacy GetCurrentPharmacy()
        {
            DataTable tableCurrentPharmacy = PharmacyDb.GetCurrentPharmacy();
            Pharmacy pharmacy = new Pharmacy(tableCurrentPharmacy.Rows[0]); 
            pharmacy.MedicalPersonnels.InitPharmacists();

            pharmacy.AllEmployees.Init();

            return pharmacy;
        }

        public Employee GetPharmacist()
        {
            if (ViewModelMainWindow.CurrentUser.IsLocalPharmist())
                return MedicalPersonnels.Single(m => m.ID.Equals(ViewModelMainWindow.CurrentUser.ID));
            var medicalPersonnels = MedicalPersonnels.GetLocalPharmacist();
            return medicalPersonnels[0];
        }

        public bool SetPharmacy()
        {
            return PharmacyDb.SetPharmacy(this).Rows[0].Field<string>("result") == "Success" ? true : false;
        }

        public void InsertPharmacy()
        {
            PharmacyDb.InsertPharmacy(this);
        }

        #endregion Function

        public Employees GetPharmacists(DateTime date)
        {
            var pharmacists = new Employees();
            foreach (var e in MedicalPersonnels)
            {
                if (e.CheckLeave(date))
                {
                    if (e.IsLocal)
                        pharmacists.Add(e);
                    else if (e.ID.Equals(ViewModelMainWindow.CurrentUser.ID))
                        pharmacists.Add(e);
                }
            }
            return pharmacists;
        }

        public object Clone()
        {
            var clone = new Pharmacy
            {
                ID = ID,
                Name = Name,
                Address = Address,
                Tel = Tel,
                ReaderCom = ReaderCom,
                VpnIp = VpnIp,
                NewInstitution = NewInstitution,
                GroupServerName = GroupServerName,
                TAXNUM= TAXNUM
            };
            return clone;
        }

        internal static DataTable GetParameters(string sysParName)
        {
            //DateTime result = new DateTime();
            //try
            //{
            //    SQLServerConnection.DapperQuery((conn) =>
            //    {
            //        result = conn.QueryFirst<DateTime>(string.Format("Select Isnull(CurPha_ClosingDate, GETDATE()) As CurPha_ClosingDate From [{0}].[SystemInfo].[CurrentPharmacy]",
            //            Properties.Settings.Default.SystemSerialNumber),
            //             commandType: CommandType.Text);
            //    });
            //}
            //catch { }
            DataTable table = PharmacyDb.GetSystemParameters(sysParName);
            return table;
        }
    }
}