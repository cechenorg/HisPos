using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Product.PrescriptionSendData
{
    public class PrescriptionSendData : ObservableObject
    {
        public PrescriptionSendData()
        {
        }
        public PrescriptionSendData(Medicine.Base.Medicine m) {
            MedId = m.ID;
            MedName = m.FullName; 
            TreatAmount = m.Amount; 
            SendAmount = m.Amount;
            OldSendAmount = m.SendAmount;
        }
        public string MedId { get; set; }
        public string MedName { get; set; } 
        private double ontheFrame;
        public double OntheFrame
        {
            get => ontheFrame;
            set
            {
                Set(() => OntheFrame, ref ontheFrame, value);
            }
        }
        private double ontheWay;
        public double OntheWay
        {
            get => ontheWay;
            set
            {
                Set(() => OntheWay, ref ontheWay, value);
            }
        } 
        public double TreatAmount { get; set; }
        public double OldSendAmount { get; set; }
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
    }
}
