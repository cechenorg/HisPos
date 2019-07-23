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
            PrepareAmount = indexReserveDetail.Amount - indexReserveDetail.SendAmount;
            Amount = indexReserveDetail.Amount;
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double PrepareAmount { get; set; }
        
    }
}
