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
            if (indexReserveDetail.IsCommon)
                Name += "(常備)";
            if (!(indexReserveDetail.IsControl is null) && indexReserveDetail.IsControl > 0)
                Name += "(管藥)";
            if (indexReserveDetail.IsFrozen)
                Name += "(冰品)";
            PrepareAmount = indexReserveDetail.Amount - indexReserveDetail.SendAmount;
            Amount = indexReserveDetail.Amount;
            IsControlCommonFrozen = indexReserveDetail.IsFrozen || indexReserveDetail.IsCommon ||
                                    !(indexReserveDetail.IsControl is null) && indexReserveDetail.IsControl > 0;
        }

        public ReserveMedicine(PrescriptionSendData prescriptionSendData)
        {
            ID = prescriptionSendData.MedId;
            Name = Strings.StrConv(prescriptionSendData.MedName, VbStrConv.Narrow);
            if (prescriptionSendData.IsCommon)
                Name += "(常備)";
            if (prescriptionSendData.IsControl)
                Name += "(管藥)";
            if (prescriptionSendData.IsFrozen)
                Name += "(冰品)";
            PrepareAmount = prescriptionSendData.TreatAmount - prescriptionSendData.SendAmount;
            Amount = prescriptionSendData.TreatAmount;
            IsControlCommonFrozen = prescriptionSendData.IsFrozen || prescriptionSendData.IsCommon ||
                                    prescriptionSendData.IsControl;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double PrepareAmount { get; set; }
        public bool IsControlCommonFrozen { get; set; }
    }
}