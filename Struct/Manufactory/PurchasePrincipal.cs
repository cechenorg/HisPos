using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Struct.Manufactory
{
    public struct PurchasePrincipal
    {
        public PurchasePrincipal(DataRow dataRow)
        {
            Id = dataRow["PRINCIPAL_ID"].ToString();
            Name = dataRow["PRINCIPAL_NAME"].ToString();
            Phone = dataRow["PRINCIPAL_TEL"].ToString();
        }
        
        public string Id { get; }
        public string Name { get; set; }
        public string Phone { get; set; }

    }
}
