using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Product
{
    public class PrescriptionSendData : ObservableObject
    {
        public PrescriptionSendData()
        {
        }
        public PrescriptionSendData(Medicine.Medicine m) {
            MedId = m.ID;
            MedName = m.ChineseName;
            Stock = m.Inventory;
            TreatAmount = m.Amount;
            if (Stock <= 0 || Stock > TreatAmount)
                SendAmount = TreatAmount;
            else if (Stock > 0 && Stock < TreatAmount)
                SendAmount = TreatAmount - Stock;
             
        }
        public string MedId { get; set; }
        public string MedName { get; set; }
        public double Stock { get; set; }
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

    }
}
