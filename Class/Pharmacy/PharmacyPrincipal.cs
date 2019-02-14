using His_Pos.Interface;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.Class.Pharmacy
{
    public class PharmacyPrincipal : IPrincipal, ICloneable
    {
        public PharmacyPrincipal()
        {
            Id = "";
            Name = "";
            NickName = "";
            Telephone = "";
            Fax = "";
            Email = "";
            Line = "";
        }

        public PharmacyPrincipal(DataRow row)
        {
            Id = row["MAN_ID"].ToString();
            Name = row["MAN_NAME"].ToString();
            NickName = row["MAN_NICKNAME"].ToString();
            Telephone = row["MAN_TEL"].ToString();
            Fax = row["MAN_FAX"].ToString();
            Email = row["MAN_EMAIL"].ToString();
            Line = row["MAN_LINE"].ToString();
            IsEnable = Boolean.Parse(row["MAN_ENABLE"].ToString());
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Line { get; set; }
        public bool IsEnable { get; set; } = true;

        public ObservableCollection<PharmacyGetOverview> PharmacyGetOverviews { get; set; }

        public object Clone()
        {
            PharmacyPrincipal newPharmacyPrincipal = new PharmacyPrincipal
            {
                Id = Id,
                Name = Name,
                Telephone = Telephone,
                Fax = Fax,
                NickName = NickName,
                Email = Email,
                Line = Line,
                IsEnable = IsEnable
            };

            return newPharmacyPrincipal;
        }
    }
}
