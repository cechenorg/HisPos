using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using His_Pos.Class.Declare;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.Medicine;

namespace His_Pos.NewClass.Product.CustomerHistoryProduct
{
    public class CustomerHistoryProducts:Collection<CustomerHistoryProduct>
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
        internal void GetCustomerHistoryProducts(int id,HistoryType type)
        {
            switch (type)
            {
                case HistoryType.AdjustRecord:
                case HistoryType.RegisterRecord:
                    GetDataByPrescriptionId(id);
                    break;
                case HistoryType.ReservedPrescription:
                    GetDataByReserveId(id.ToString());
                    break;
            }
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
        public void GetDataByReserveId(string resId)
        {
            DataTable table = MedicineDb.GetDataByReserveId(resId);
            foreach (DataRow r in table.Rows)
            {
                var pro = new CustomerHistoryProduct(r);
                Add(pro);
            }
        }
    }
}
