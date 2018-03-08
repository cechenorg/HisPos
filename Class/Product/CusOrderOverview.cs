using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class CusOrderOverview
    {
        public CusOrderOverview(string date, string amount, string profit)
        {
            Date = date;
            Amount = amount;
            Profit = profit;
        }

        public string Date { get; }
        public string Amount { get; }
        public string Profit { get; }
    }
}
