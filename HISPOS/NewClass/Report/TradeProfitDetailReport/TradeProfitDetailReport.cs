using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;

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
            PrePay = r.Field<int>("PrePay");
            IsEnable = r.Field<Byte>("TraMas_IsEnable") == 1;
        }

        private bool isEnable;
        private int id;
        private string name;
        private int realTotal;
        private decimal valueDifference;
        private int profit;
        private int cashAmount;
        private int cardAmount;
        private int prePay;
        private int discountAmt;
        private string typeId;
        private int disableCount; //退貨
        private int count;
        private int totalCount;
        private decimal cardFee;
        private int totalCost;
        private int cashCoupon;
        private int totalChange;
        private int totalProfit;
        private int discountAmtMinus;

        public bool IsEnable
        {
            get => isEnable;
            set
            {
                Set(() => IsEnable, ref isEnable, value);
            }
        }

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


        public int TotalCount
        {
            get => totalCount;
            set
            {
                Set(() => TotalCount, ref totalCount, value);
            }
        }

        public int DisableCount
        {
            get => disableCount;
            set
            {
                Set(() => DisableCount, ref disableCount, value);
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

        // 預付訂金
        public int PrePay
        {
            get => prePay;
            set
            {
                Set(() => PrePay, ref prePay, value);
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

        public void SumOTCReport(IEnumerable<TradeProfitDetailReport> tradeProfitDetails)
        {
            CardAmount = tradeProfitDetails.Sum(s => s.CardAmount);
            CashAmount = tradeProfitDetails.Sum(s => s.CashAmount);
            DiscountAmt = tradeProfitDetails.Sum(s => s.DiscountAmt);
            CashCoupon = tradeProfitDetails.Sum(s => s.CashCoupon);
            PrePay = tradeProfitDetails.Sum(s => s.PrePay); //0806
            Profit = tradeProfitDetails.Sum(s => s.Profit); 
            ValueDifference = tradeProfitDetails.Sum(s => s.ValueDifference);
            CardFee = tradeProfitDetails.Sum(s => s.CardFee);
            DisableCount = tradeProfitDetails.Count(_ => _.IsEnable == false);
            Count = tradeProfitDetails.Count(_ => _.IsEnable);
            TotalCount = DisableCount + Count;
            TotalCost = (int)tradeProfitDetails.Sum(s => s.ValueDifference);
            DiscountAmtMinus = DiscountAmt * -1;
             
            RealTotal = tradeProfitDetails.Sum(s => s.CardAmount) +
                        tradeProfitDetails.Sum(s => s.CashAmount) +
                        tradeProfitDetails.Sum(s => s.DiscountAmt) +
                        tradeProfitDetails.Sum(s => s.CashCoupon) +
                        tradeProfitDetails.Sum(s => s.PrePay); 
             
        }

       
    }
}