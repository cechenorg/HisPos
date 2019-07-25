using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Trade
{
    public class Transaction
    {
        #region ----- Define Variables -----
        public TransactionTypeEnum Type { get; set; }
        public int ID { get; set; }
        public int TotalPrice { get; set; }
        public int DiscountPrice { get; set; }
        public float DiscountPercent { get; set; }
        public int DiscountPercentPrice { get; set; }
        public int DiscountPoint { get; set; }
        public int DiscountPointPrice { get; set; }
        public int Cash { get; set; }
        public int OrderCash { get; set; }
        public int CardPrice { get; set; }
        public string CardNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string TaxIDNumber { get; set; }

        public int FinalTotalPrice { get; set; }
        public int PayPrice { get; set; }
        public int LeftPrice { get; set; }
        #endregion

        #region ----- Define Functions -----
        internal void Clear()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
