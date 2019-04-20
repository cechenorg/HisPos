using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder.Report
{
    public class ManufactoryOrderDetails : Collection<ManufactoryOrderDetail>
    {
        public ManufactoryOrderDetails(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ManufactoryOrderDetail(row));
            }
        }

        internal void ReCalculateTax(bool includeTax)
        {
            foreach (var detail in Items)
            {
                detail.CalculateTax(includeTax);
            }
        }

        internal static ManufactoryOrderDetails GetOrderDetails(string manufactoryID)
        {
            throw new NotImplementedException();
        }
    }
}
