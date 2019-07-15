using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public class ReserveMedicine
    {
        public ReserveMedicine(IndexReserveDetail indexReserveDetail)
        {
            ID = indexReserveDetail.ID;
            Name = indexReserveDetail.FullName; 
            Amount = indexReserveDetail.Amount;
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
      
    }
}
