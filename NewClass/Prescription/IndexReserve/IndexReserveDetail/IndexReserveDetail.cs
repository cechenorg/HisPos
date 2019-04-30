using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail
{
   public class IndexReserveDetail : ObservableObject {
        public IndexReserveDetail(DataRow r) {
            Id = r.Field<string>("MedicineID");
            Name = r.Field<string>("Pro_ChineseName");
            Amount = r.Field<double>("TotalAmount"); 
             Stock = r.Field<double>("Inventory");
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double Stock { get; set; }
    }
}
