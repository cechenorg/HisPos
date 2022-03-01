using GalaSoft.MvvmLight;
using His_Pos.NewClass.Medicine.Base;

namespace His_Pos.NewClass.Product.PrescriptionSendData
{
    public class PrescriptionSendData : ObservableObject
    {
        public PrescriptionSendData()
        {
        }

        public PrescriptionSendData(Medicine.Base.Medicine m)
        {
            MedId = m.ID;
            MedName = m.FullName;
            TreatAmount = m.Amount;
            SendAmount = 0;
            InvID = m.InventoryID;
            CanUseAmount = 0;
            if (m is MedicineNHI nhiMed)
            {
                IsControl = nhiMed.ControlLevel != null;
                IsCommon = nhiMed.IsCommon;
                IsFrozen = nhiMed.Frozen;
            }
        }

        public string MedId { get; set; }
        public string MedName { get; set; }
        public int InvID { get; set; }
        private double canUseAmount;

        public double CanUseAmount
        {
            get => canUseAmount;
            set
            {
                Set(() => CanUseAmount, ref canUseAmount, value);
            }
        }

        public double TreatAmount { get; set; }
        private double sendAmount;

        public double SendAmount
        {
            get => sendAmount;
            set
            {
                Set(() => SendAmount, ref sendAmount, value);
            }
        }

        private double prepareAmount;

        public double PrepareAmount
        {
            get => prepareAmount;
            set
            {
                Set(() => PrepareAmount, ref prepareAmount, value);
            }
        }

        public bool IsFrozen { get; set; }
        public bool IsControl { get; set; }
        public bool IsCommon { get; set; }
    }
}