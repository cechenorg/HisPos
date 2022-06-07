using System.Collections.ObjectModel;
using System.Data;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proID"></param>
        /// <param name="wareID"></param>
        /// <param name="undo">用來判斷是採購還是藥袋</param>
        /// <returns></returns>
        internal static OnTheWayDetailStructs GetOnTheWayDetailByID(string proID, string wareID,int undo)
        {
            return new OnTheWayDetailStructs(ProductDetailDB.GetOnTheWayDetailByID(proID, wareID, undo));
        }

        #endregion ----- Define Functions -----
    }
}