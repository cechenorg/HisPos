using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Report.TradeProfitReport
{
    public class TradeProfitReport : ObservableObject
    {
        public TradeProfitReport()
        {
        }

        public TradeProfitReport(DataRow r)
        {
            TypeId = r.Field<string>("TypeId");
            TypeName = r.Field<string>("TypeName");
            Count = r.Field<int>("Count");
            NetIncome = r.Field<int>("NetIncome");
            Cost = r.Field<int>("Cost");
            Profit = r.Field<int>("Profit");
            CashAmount = r.Field<int>("CashAmount");
            CardAmount = r.Field<int>("CardAmount");
            DiscountAmt = r.Field<int>("DiscountAmt");
            CardFee = r.Field<decimal>("CardFee");
            CashCoupon = r.Field<int>("CashCoupon");
        }

        private string typeId;

        public string TypeId
        {
            get => typeId;
            set
            {
                Set(() => TypeId, ref typeId, value);
            }
        }

        private string typeName;

        public string TypeName
        {
            get => typeName;
            set
            {
                Set(() => TypeName, ref typeName, value);
            }
        }

        private int count;

        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }

        private int cashCoupon;

        public int CashCoupon
        {
            get => cashCoupon;
            set
            {
                Set(() => CashCoupon, ref cashCoupon, value);
            }
        }

        private int netIncome;

        public int NetIncome
        {
            get => netIncome;
            set
            {
                Set(() => NetIncome, ref netIncome, value);
            }
        }

        private int cost;

        public int Cost
        {
            get => cost;
            set
            {
                Set(() => Cost, ref cost, value);
            }
        }

        private int profit;

        public int Profit
        {
            get => profit;
            set
            {
                Set(() => Profit, ref profit, value);
            }
        }

        private int cashAmount;

        public int CashAmount
        {
            get => cashAmount;
            set
            {
                Set(() => CashAmount, ref cashAmount, value);
            }
        }

        private int cardAmount;

        public int CardAmount
        {
            get => cardAmount;
            set
            {
                Set(() => CardAmount, ref cardAmount, value);
            }
        }

        private int discountAmt;

        public int DiscountAmt
        {
            get => discountAmt;
            set
            {
                Set(() => DiscountAmt, ref discountAmt, value);
            }
        }

        private int totalAmt;

        public int TotalAmt
        {
            get => totalAmt;
            set
            {
                Set(() => TotalAmt, ref totalAmt, value);
            }
        }

        private int totalDeleteAmt;

        public int TotalDeleteAmt
        {
            get => totalDeleteAmt;
            set
            {
                Set(() => TotalDeleteAmt, ref totalDeleteAmt, value);
            }
        }

        private int totalChangeAmt;

        public int TotalChangeAmt
        {
            get => totalChangeAmt;
            set
            {
                Set(() => TotalChangeAmt, ref totalChangeAmt, value);
            }
        }

        private int totalNormalAmt;

        public int TotalNormalAmt
        {
            get => totalNormalAmt;
            set
            {
                Set(() => TotalNormalAmt, ref totalNormalAmt, value);
            }
        }

        private int totalChangeCashAmt;

        public int TotalChangeCashAmt
        {
            get => totalChangeCashAmt;
            set
            {
                Set(() => TotalChangeCashAmt, ref totalChangeCashAmt, value);
            }
        }

        private int totalChangeCardAmt;

        public int TotalChangeCardAmt
        {
            get => totalChangeCardAmt;
            set
            {
                Set(() => TotalChangeCardAmt, ref totalChangeCardAmt, value);
            }
        }

        private int totalChangeDiscountAmt;

        public int TotalChangeDiscountAmt
        {
            get => totalChangeDiscountAmt;
            set
            {
                Set(() => TotalChangeDiscountAmt, ref totalChangeDiscountAmt, value);
            }
        }

        private int totalChangeCashCouponAmt;

        public int TotalChangeCashCouponAmt
        {
            get => totalChangeCashCouponAmt;
            set
            {
                Set(() => TotalChangeCashCouponAmt, ref totalChangeCashCouponAmt, value);
            }
        }

        private int totalDeleteCashAmt;

        public int TotalDeleteCashAmt
        {
            get => totalDeleteCashAmt;
            set
            {
                Set(() => TotalDeleteCashAmt, ref totalDeleteCashAmt, value);
            }
        }

        private int totalDeleteCardAmt;

        public int TotalDeleteCardAmt
        {
            get => totalDeleteCardAmt;
            set
            {
                Set(() => TotalDeleteCardAmt, ref totalDeleteCardAmt, value);
            }
        }

        private int totalDeleteCostAmt;

        public int TotalDeleteCostAmt
        {
            get => totalDeleteCostAmt;
            set
            {
                Set(() => TotalDeleteCostAmt, ref totalDeleteCostAmt, value);
            }
        }

        private int totalChangeCostAmt;

        public int TotalChangeCostAmt
        {
            get => totalChangeCostAmt;
            set
            {
                Set(() => TotalChangeCostAmt, ref totalChangeCostAmt, value);
            }
        }

        private int totalDeleteDiscountAmt;

        public int TotalDeleteDiscountAmt
        {
            get => totalDeleteDiscountAmt;
            set
            {
                Set(() => TotalDeleteDiscountAmt, ref totalDeleteDiscountAmt, value);
            }
        }

        private int totalDeleteCashCouponAmt;

        public int TotalDeleteCashCouponAmt
        {
            get => totalDeleteCashCouponAmt;
            set
            {
                Set(() => TotalDeleteCashCouponAmt, ref totalDeleteCashCouponAmt, value);
            }
        }

        private int totalDiscountTotalAmt;

        public int TotalDiscountTotalAmt
        {
            get => totalDiscountTotalAmt;
            set
            {
                Set(() => TotalDiscountTotalAmt, ref totalDiscountTotalAmt, value);
            }
        }

        private int totalCardTotalAmt;

        public int TotalCardTotalAmt
        {
            get => totalCardTotalAmt;
            set
            {
                Set(() => TotalCardTotalAmt, ref totalCardTotalAmt, value);
            }
        }

        private int totalCashTotalAmt;

        public int TotalCashTotalAmt
        {
            get => totalCashTotalAmt;
            set
            {
                Set(() => TotalCashTotalAmt, ref totalCashTotalAmt, value);
            }
        }

        private int totalCostTotalAmt;

        public int TotalCostTotalAmt
        {
            get => totalCostTotalAmt;
            set
            {
                Set(() => TotalCostTotalAmt, ref totalCostTotalAmt, value);
            }
        }

        private int totalCostTotal;

        public int TotalCostTotal
        {
            get => totalCostTotal;
            set
            {
                Set(() => TotalCostTotal, ref totalCostTotal, value);
            }
        }

        private int totalCashCouponTotalAmt;

        public int TotalCashCouponTotalAmt
        {
            get => totalCashCouponTotalAmt;
            set
            {
                Set(() => TotalCashCouponTotalAmt, ref totalCashCouponTotalAmt, value);
            }
        }

        private decimal cardFee;

        public decimal CardFee
        {
            get => cardFee;
            set
            {
                Set(() => CardFee, ref cardFee, value);
            }
        }

        public void CountEditPoint(DataRow editDataRow)
        {
            Count += editDataRow.Field<int>("Count");
            NetIncome += editDataRow.Field<int>("NetIncome");
            Cost += editDataRow.Field<int>("Cost");
            Profit += editDataRow.Field<int>("Profit");
        }
    }
}