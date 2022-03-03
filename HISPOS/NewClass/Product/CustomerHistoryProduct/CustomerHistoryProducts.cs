using His_Pos.NewClass;
using His_Pos.NewClass.Medicine;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.CustomerHistoryProduct
{
    public class CustomerHistoryProducts : ObservableCollection<CustomerHistoryProduct>
    {
        public CustomerHistoryProducts()
        {
        }

        public CustomerHistoryProducts(DataTable dataTable)
        {
            foreach (DataRow r in dataTable.Rows)
            {
                Add(new CustomerHistoryProduct(r));
            }
        }

        internal void GetCustomerHistoryProducts(int id, HistoryType type)
        {
            GetDataByPrescriptionId(id);
        }

        public void GetDataByPrescriptionId(int preId)
        {
            DataTable table = MedicineDb.GetDataByPrescriptionId(preId);
            foreach (DataRow r in table.Rows)
            {
                var pro = new CustomerHistoryProduct(r);
                Add(pro);
            }
        }
    }
}