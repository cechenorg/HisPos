using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductRegisterPrescriptions : Collection<ProductRegisterPrescription>
    {
        private ProductRegisterPrescriptions(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductRegisterPrescription(row));
            }
        }

        internal static ProductRegisterPrescriptions GetRegisterPrescriptionsByID(string proID, string wareID)
        {
            return new ProductRegisterPrescriptions(ProductDetailDB.GetRegisterPrescriptionsByID(proID, wareID));
        }
    }
}