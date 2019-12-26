using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.BalanceSheet
{
    public class BalanceSheetData
    {
        #region ----- Define Variables -----
        public string Name { get; set; }
        public string Type { get; set; }
        public double Value { get; set; }
        #endregion

        public BalanceSheetData(DataRow row)
        {
            Name = row.Field<string>("HEADER");
            Type = row.Field<string>("TYPE");
            Value = (double)row.Field<decimal>("VALUE");
        }
    }
}
