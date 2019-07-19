using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using Microsoft.VisualBasic;

namespace His_Pos.NewClass.Medicine.ReserveMedicine
{
    public class ReserveMedicine
    {
        public ReserveMedicine(IndexReserveDetail indexReserveDetail)
        {
            ID = indexReserveDetail.ID;
            Name = Strings.StrConv(indexReserveDetail.FullName, VbStrConv.Narrow);  
            Amount = indexReserveDetail.Amount - indexReserveDetail.SendAmount;
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
      
    }
}
