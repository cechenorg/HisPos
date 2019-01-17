using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine
{
    public static class MedicineDb
    {
        public static DataRow GetMedicinesBySearchId(string medicineID)
        {
            var table = new DataTable();
            return table.Rows[0];
        }
    }
}
