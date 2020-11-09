using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitReport
{
    public class TradeProfitReport : ObservableObject
    {
        public TradeProfitReport()
        {
        }
        public TradeProfitReport(DataRow r) {
            TypeId = r.Field<string>("TypeId");
            TypeName = r.Field<string>("TypeName");
            Count = r.Field<int>("Count");
            NetIncome = r.Field<int>("NetIncome");
            Cost = r.Field<int>("Cost");
            Profit = r.Field<int>("Profit");
            CashAmount = r.Field<int>("CashAmount");
            CardAmount = r.Field<int>("CardAmount");
            DiscountAmt = r.Field<int>("DiscountAmt");
        }
        private string typeId;
        public string TypeId {
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


        public void CountEditPoint(DataRow editDataRow)
        {
            Count += editDataRow.Field<int>("Count");
            NetIncome+= editDataRow.Field<int>("NetIncome");
            Cost+= editDataRow.Field<int>("Cost");
            Profit += editDataRow.Field<int>("Profit");
        }
    }
}
