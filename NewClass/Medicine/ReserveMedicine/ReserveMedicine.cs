using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using His_Pos.NewClass.Product.PrescriptionSendData;
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
        public ReserveMedicine(PrescriptionSendData prescriptionSendData)
        {
            ID = prescriptionSendData.MedId;
            Name = Strings.StrConv(prescriptionSendData.MedName, VbStrConv.Narrow);
            PrepareAmount = prescriptionSendData.TreatAmount - prescriptionSendData.SendAmount;
            Amount = prescriptionSendData.TreatAmount;
        }
        
        public string ID { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double PrepareAmount { get; set; }
        
    }
}
