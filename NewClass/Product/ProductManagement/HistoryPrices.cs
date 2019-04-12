using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class HistoryPrices : Collection<HistoryPrice>
    {
        private HistoryPrices(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new HistoryPrice(row));
            }
        }

        internal static HistoryPrices GetHistoryPrices(string medicineID)
        {
            return new HistoryPrices(ProductDetailDB.GetMedicineHistoryPrices(medicineID));
        }
    }
}
