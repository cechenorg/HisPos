using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.ControlMedicineDetail
{
    public class ControlMedicineDetail : ObservableObject
    {
        public ControlMedicineDetail() { }
        public ControlMedicineDetail(DataRow r, double stock)
        {
            Date = r.Field<DateTime>("Date");
            TypeName = r.Field<string>("InvRec_Type");
            InputAmount = r.Field<double>("InputAmount");
            OutputAmount = r.Field<double>("OutputAmount");
            BatchNumber = r.Field<string>("InvRec_BatchNumber");
            Description = r.Field<string>("Description");
            FinalStock = stock + InputAmount + OutputAmount;
        }
        public DateTime Date { get; set; }
        public string TypeName { get; set; }
        public double InputAmount { get; set; }
        public double OutputAmount { get; set; }
        public string BatchNumber { get; set; }
        public double FinalStock { get; set; }
        public string Description { get; set; }
    }
}
