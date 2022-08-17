using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Medicine.NotEnoughMedicine
{
    public class NotEnoughMedicine : ObservableObject
    {
        public string ID { get; }
        public string Name { get; }
        private double amount;

        public double Amount
        {
            get => amount;
            set
            {
                Set(() => Amount, ref amount, value);
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set { Set(() => IsSelected, ref isSelected, value); }
        }

        private double notEnoughAmount;

        public double NotEnoughAmount
        {
            get => notEnoughAmount;
            set
            {
                Set(() => NotEnoughAmount, ref notEnoughAmount, value);
            }
        }

        private double canUseAmount;

        public double CanUseAmount
        {
            get => canUseAmount;
            set
            {
                Set(() => CanUseAmount, ref canUseAmount, value);
            }
        }

        private double treatAmount;

        public double TreatAmount
        {
            get => treatAmount;
            set
            {
                Set(() => TreatAmount, ref treatAmount, value);
            }
        }

        private int singdeInv;

        public int SingdeInv
        {
            get => singdeInv;
            set
            {
                Set(() => SingdeInv, ref singdeInv, value);
            }
        }


        public bool IsSingdeMedEnough
        {
            get => SingdeInv >= Amount;
            
        }

        public double PrepareAmount { get; }

        public bool IsCommon { get; }
        public bool Frozen { get; }
        public int? ControlLevel { get; }
        public double AveragePrice { get; }

        public bool SingdeInvNotEnough { get; }

        public NotEnoughMedicine(string id, string name, double amount, bool isCommon, bool isFrozen, int? controlLevel, double price, double notEnough, double canUseAmount=0, double treatAmount=0, int singdeInv=0)
        {
            ID = id;
            Name = name;
            CanUseAmount = canUseAmount;
            NotEnoughAmount = notEnough;
            TreatAmount = treatAmount;
            Amount = amount;
            PrepareAmount = treatAmount - amount;
            SingdeInv = singdeInv;
            SingdeInvNotEnough = false;
            if (singdeInv < amount)
                SingdeInvNotEnough = true;
            AveragePrice = price;
            IsCommon = isCommon;
            Frozen = isFrozen;
            ControlLevel = controlLevel;
        }
    }
}