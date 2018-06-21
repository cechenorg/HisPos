using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Manufactory
{
    public class ManufactoryPrincipal
    {
        public ManufactoryPrincipal(DataRow row)
        {
            Id = row["MAN_ID"].ToString();
            Name = row["MAN_NAME"].ToString();
            NickName = row["MAN_NICKNAME"].ToString();
            Telphone = row["MAN_TEL"].ToString();
            Fax = row["MAN_FAX"].ToString();
            Email = row["MAN_EMAIL"].ToString();
            PayType = "";
            ResponsibleDepartment = row["MAN_RESPONSIBLEDEP"].ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Telphone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string PayType { get; set; }
        public string ResponsibleDepartment { get; set; }
    }
}
