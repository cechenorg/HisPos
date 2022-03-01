using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Report.RewardReport
{
    public class RewardReport : ObservableObject
    {
        public RewardReport()
        {
        }

        public RewardReport(DataRow r)
        {
            RewardAmount = r.Field<double>("RewardAmount");
            Free = r.Field<double>("Free");
        }

        private double rewardAmount;

        public double RewardAmount
        {
            get => rewardAmount;
            set
            {
                Set(() => RewardAmount, ref rewardAmount, value);
            }
        }

        private double rewardAmountSum;

        public double RewardAmountSum
        {
            get => rewardAmountSum;
            set
            {
                Set(() => RewardAmountSum, ref rewardAmountSum, value);
            }
        }

        private double free;

        public double Free
        {
            get => free;
            set
            {
                Set(() => Free, ref free, value);
            }
        }
    }
}