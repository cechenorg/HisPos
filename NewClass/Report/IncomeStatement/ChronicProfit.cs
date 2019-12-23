using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class ChronicProfit : ObservableObject
    {
        public ChronicProfit(decimal profit)
        {
            Profit = profit;
        }
        private decimal profit;

        public decimal Profit
        {
            get => profit;
            set
            {
                Set(() => Profit, ref profit, value);
            }
        }
    }
}
