using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace InfraStructure.SQLService.SQLServer.UserDefinedDataType
{
    internal class MedicineListTable
    {
        public MedicineListTable(){}
        public string MedicineID { get; set; }
         
        public static SqlMapper.ICustomQueryParameter SetMedicines(IEnumerable<MedicineListTable> MedicineIds)
        {
            var dt = new DataTable();
            dt.Columns.Add("MedicineID");
            foreach (var med in MedicineIds)
            {
                dt.Rows.Add(med.MedicineID);
            }

            return dt.AsTableValuedParameter("[HIS].[MedicineListTable]");
        } 
    }
    
}
