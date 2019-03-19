using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.ControlMedicineDetail
{
    public class ControlMedicineDetail : ObservableObject {
        public ControlMedicineDetail(DataRow r) {
            Date = r.Field<DateTime>("InvRec_Time");
            TypeName = r.Field<string>("InvRec_Type");
            Amount = r.Field<double>("Amount");
            BatchNumber = r.Field<string>("InvRec_BatchNumber");
            FinalStock = r.Field<double>("InvRec_NewStock");
            Description = r.Field<string>("Description");
        }
        public DateTime Date { get; set; }
        public string TypeName { get; set; }
        public double Amount { get; set; }
        public string BatchNumber { get; set; }
        public double FinalStock { get; set; }
        public string Description { get; set; }
    }
}
