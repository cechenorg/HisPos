using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement.MedBagDetail
{
    public class MedBagDetailStructs : Collection<MedBagDetailStruct>
    {
        private MedBagDetailStructs(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new MedBagDetailStruct(row));
            }
        }
        
        #region ----- Define Functions -----
        internal static MedBagDetailStructs GetMedBagDetailByID(string proID, string wareID)
        {
            return new MedBagDetailStructs(ProductDetailDB.GetMedBagDetailByID(proID, wareID));
        }

        internal static MedBagDetailStructs GetOTCMedBagDetailByID(string proID, string wareID)
        {
            return new MedBagDetailStructs(ProductDetailDB.GetOTCMedBagDetailByID(proID, wareID));
        }
        #endregion
    }
}
