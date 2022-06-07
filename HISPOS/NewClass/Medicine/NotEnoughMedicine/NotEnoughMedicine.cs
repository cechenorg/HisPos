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

        public bool IsCommon { get; }
        public bool Frozen { get; }
        public int? ControlLevel { get; }
        public double AveragePrice { get; }

        public NotEnoughMedicine(string id, string name, double amount, bool isCommon, bool isFrozen, int? controlLevel, double price, double notEnough)
        {
            ID = id;
            Name = name;
            Amount = amount;
            IsCommon = isCommon;
            Frozen = isFrozen;
            ControlLevel = controlLevel;
            AveragePrice = price;
            NotEnoughAmount = notEnough;
        }
    }
}