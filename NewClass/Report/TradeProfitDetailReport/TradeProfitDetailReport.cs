using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport
{
    public class TradeProfitDetailReport : ObservableObject
    {
        public TradeProfitDetailReport()
        {
        }

        public TradeProfitDetailReport(DataRow r)
        {
            Id = r.Field<int>("TraMas_ID");
            Name = r.Field<string>("PosCus_Name");
            RealTotal = r.Field<int>("TraMas_RealTotal");
            ValueDifference = Math.Round(r.Field<decimal>("ValueDifference"), 2);
            Profit = r.Field<int>("Profit");
            CashAmount = r.Field<int>("CashAmount");
            CardAmount = r.Field<int>("CardAmount");
            DiscountAmt = r.Field<int>("DiscountAmt");
            TypeId = r.Field<string>("TypeId");
            CardFee = r.Field<decimal>("CardFee");
            CashCoupon = r.Field<int>("CashCoupon");
        }

        private int id;
        private string name;
        private int realTotal;
        private decimal valueDifference;
        private int profit;
        private int cashAmount;
        private int cardAmount;
        private int discountAmt;
        private string typeId;
        private int count;
        private decimal cardFee;
        private int totalCost;
        private int cashCoupon;
        private int totalChange;
        private int totalProfit;
        private int discountAmtMinus;
        public string TypeId
        {
            get => typeId;
            set
            {
                Set(() => TypeId, ref typeId, value);
            }
        }

        public int Id
        {
            get => id;
            set
            {
                Set(() => Id, ref id, value);
            }
        }

        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }

        public string Name
        {
            get => name;
            set
            {
                Set(() => Name, ref name, value);
            }
        }

        public int RealTotal
        {
            get => realTotal;
            set
            {
                Set(() => RealTotal, ref realTotal, value);
            }
        }

        public decimal ValueDifference
        {
            get => valueDifference;
            set
            {
                Set(() => ValueDifference, ref valueDifference, value);
            }
        }

        public int Profit
        {
            get => profit;
            set
            {
                Set(() => Profit, ref profit, value);
            }
        }

        public int CashAmount
        {
            get => cashAmount;
            set
            {
                Set(() => CashAmount, ref cashAmount, value);
            }
        }

        public int CardAmount
        {
            get => cardAmount;
            set
            {
                Set(() => CardAmount, ref cardAmount, value);
            }
        }

        public int DiscountAmt
        {
            get => discountAmt;
            set
            {
                Set(() => DiscountAmt, ref discountAmt, value);
            }
        }
        public int DiscountAmtMinus
        {
            get => discountAmtMinus;
            set
            {
                Set(() => DiscountAmtMinus, ref discountAmtMinus, value);
            }
        }

        public int CashCoupon
        {
            get => cashCoupon;
            set
            {
                Set(() => CashCoupon, ref cashCoupon, value);
            }
        }

        public decimal CardFee
        {
            get => cardFee;
            set
            {
                Set(() => CardFee, ref cardFee, value);
            }
        }
        public int TotalCost
        {
            get => totalCost;
            set
            {
                Set(() => TotalCost, ref totalCost, value);
            }
        }
        public int TotalChange
        {
            get => totalChange;
            set
            {
                Set(() => TotalChange, ref totalChange, value);
            }
        }
        public int TotalProfit
        {
            get => totalProfit;
            set
            {
                Set(() => TotalProfit, ref totalProfit, value);
            }
        }
    }
}