using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement.OnTheWayDetail
{
    public class OnTheWayDetailStructs : Collection<OnTheWayDetailStruct>
    {
        private OnTheWayDetailStructs(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new OnTheWayDetailStruct(row));
            }
        }

        #region ----- Define Functions -----
        internal static OnTheWayDetailStructs GetOnTheWayDetailByID(string proID, string wareID)
        {
            return new OnTheWayDetailStructs(ProductDetailDB.GetOnTheWayDetailByID(proID, wareID));
        }
        #endregion
    }
}
