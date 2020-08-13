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

        public void CountEditPoint(DataRow editDataRow)
        {
            Count += editDataRow.Field<int>("Count");
            NetIncome+= editDataRow.Field<int>("NetIncome");
            Cost+= editDataRow.Field<int>("Cost");
            Profit += editDataRow.Field<int>("Profit");
        }
    }
}
