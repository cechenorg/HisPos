using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace His_Pos.Class.Manufactory
{
    public class ManufactoryPrincipal : ICloneable
    {
        public ManufactoryPrincipal()
        {
            Id = "";
            Name = "";
            NickName = "";
            Telphone = "";
            Fax = "";
            Email = "";
            Line = "";
            PayCondition = "D";
            PayType = "M";
            ResponsibleDepartment = "";
        }

        public ManufactoryPrincipal(DataRow row)
        {
            Id = row["MAN_ID"].ToString();
            Name = row["MAN_NAME"].ToString();
            NickName = row["MAN_NICKNAME"].ToString();
            Telphone = row["MAN_TEL"].ToString();
            Fax = row["MAN_FAX"].ToString();
            Email = row["MAN_EMAIL"].ToString();
            Line = row["MAN_LINE"].ToString();
            PayCondition = row["MAN_PAYCONDITION"].ToString();
            PayType = row["MAN_PAYTYPE"].ToString();
            ResponsibleDepartment = row["MAN_RESPONSIBLEDEP"].ToString();
            IsEnable = Boolean.Parse(row["MAN_ENABLE"].ToString()); 
        }
        
        public string Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Telphone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string PayType { get; set; }
        public string Line { get; set; }
        public string ResponsibleDepartment { get; set; }
        public string PayCondition { get; set; }
        public double GetTotal { get; set; }
        public bool IsEnable { get; set; } = true;
        
        public ObservableCollection<ManufactoryPayOverview> ManufactoryPayOverviews { get; set; }

        public object Clone()
        {
            ManufactoryPrincipal newManufactoryPrincipal = new ManufactoryPrincipal
            {
                Id = Id,
                Name = Name,
                Telphone = Telphone,
                Fax = Fax,
                NickName = NickName,
                Email = Email,
                Line = Line,
                PayType = PayType,
                ResponsibleDepartment = ResponsibleDepartment,
                ManufactoryPayOverviews = ManufactoryPayOverviews,
                PayCondition = PayCondition,
                IsEnable = IsEnable
            };

            return newManufactoryPrincipal;
        }
    }
}
